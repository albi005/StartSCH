using System.Globalization;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using StartSch.Data;
using StartSch.Services;
using StartSch.Wasm;

namespace StartSch.Modules.SchPincer;

public class SchPincerPollJob(Db db, IMemoryCache cache, NotificationQueueService notificationQueueService) : IPollJobExecutor
{
    public async Task Execute(CancellationToken cancellationToken)
    {
        HtmlDocument doc = new();
        doc.Load(await new HttpClient().GetStreamAsync("https://schpincer.sch.bme.hu", cancellationToken));

        // update Group.PincerName
        List<Group> groups = await db.Groups.ToListAsync(cancellationToken);
        var existingPincerNames = groups
            .Where(g => g.PincerName != null)
            .Select(g => g.PincerName!)
            .ToHashSet();
        var pincerGroupNames = doc.DocumentNode
            .Descendants("div")
            .First(n => n.HasClass("left-menu"))
            .Descendants("span")
            .Select(n => n.InnerText)
            .Where(s => s.Length >= 4)
            .ToList();
        foreach (string pincerName in pincerGroupNames)
        {
            if (existingPincerNames.Contains(pincerName)) continue;

            List<Group> candidates = groups
                .Where(g =>
                    g.PekName?.RoughlyMatches(pincerName) == true
                    || g.PincerName?.RoughlyMatches(pincerName) == true
                )
                .ToList();
            switch (candidates.Count)
            {
                case > 1:
                    throw new($"Multiple candidates for {pincerName}");
                case 1:
                    candidates[0].PincerName = pincerName;
                    continue;
                default:
                    Group group = new() { PincerName = pincerName };
                    db.Groups.Add(group);
                    groups.Add(group);
                    break;
            }
        }

        // parse openings
        //
        // schpincer pseudo query:
        // SELECT circle.displayName, circle.alias, opening.dateStart, opening.feeling, circle.id
        // WHERE opening.dateEnd > now AND opening.dateEnd < now + week
        // ORDER BY opening.dateStart
        List<OpeningInfo> infos = doc.DocumentNode.ChildNodes
            .Descendants("table")
            .First()
            .ChildNodes
            .Where(n => n.Name == "tr")
            .Select(tr =>
                {
                    var tds = tr.ChildNodes.Where(n => n.Name == "td").ToArray();
                    DateTime startHu = DateTime.ParseExact(
                        tds.First(n => n.HasClass("date")).InnerText,
                        "HH:mm (yy-MM-dd)",
                        CultureInfo.InvariantCulture
                    );
                    return new OpeningInfo(
                        tds.First().ChildNodes.FindFirst("a").InnerText,
                        new(startHu, Utils.HungarianTimeZone.GetUtcOffset(startHu)),
                        tds.First(n => n.HasClass("feeling")).InnerText
                    );
                }
            )
            .ToList();

        List<Group> pincerGroups = groups.Where(g => g.PincerName != null).ToList();
        Dictionary<string, Group> pincerNameToGroup = pincerGroups.ToDictionary(g => g.PincerName!);
        Dictionary<Group, (List<OpeningInfo> Infos, List<Opening> Openings)> dict = pincerGroups.ToDictionary(
            g => g,
            _ => (new List<OpeningInfo>(), new List<Opening>())
        );
        foreach (OpeningInfo info in infos)
        {
            Group group = pincerNameToGroup[info.GroupName];
            var entry = dict[group];
            entry.Infos.Add(info);
        }

        var unfinishedOpenings = await db.Openings
            .Include(o => o.Groups)
            .Where(o => !o.EndUtc.HasValue)
            .ToListAsync(cancellationToken);
        foreach (Opening opening in unfinishedOpenings)
        {
            Group group = opening.Groups[0];
            var entry = dict[group];
            entry.Openings.Add(opening);
        }

        List<Opening> requiresNotification = [];

        foreach (var group in dict)
        {
            UpdateOpenings(group.Key, group.Value.Infos, group.Value.Openings.ToHashSet(), requiresNotification, db);
        }

        // fail-safe
        if (requiresNotification.Count > 3)
            requiresNotification.Clear();

        DateTime utcNow = DateTime.UtcNow;
        foreach (Opening opening in requiresNotification)
        {
            Notification notification = new EventNotification() { Event = opening };

            var pushTags = pincerGroups.Select(g => $"push.pincér.nyitások.{g.PincerName!}");
            var pushTargets = TagGroup.GetAllTargets(pushTags);
            var pushUsers = await db.Users
                .Where(u => u.Tags.Any(t => pushTargets.Contains(t.Path)))
                .ToListAsync(cancellationToken);
            notification.Requests.AddRange(
                pushUsers.Select(u =>
                    new PushRequest
                    {
                        CreatedUtc = utcNow,
                        Notification = notification,
                        User = u,
                    })
            );

            db.Notifications.Add(notification);
        }

        await db.SaveChangesAsync(cancellationToken);
        cache.Remove(SchPincerModule.PincerGroupsCacheKey);
        if (requiresNotification.Count != 0) notificationQueueService.Notify();
    }

    // opening update types:
    // - added
    // - cancelled
    // - moved
    // - ended
    // assume only one happens per poll
    private static void UpdateOpenings(
        Group group,
        IEnumerable<OpeningInfo> infos,
        HashSet<Opening> unfinishedOpenings,
        List<Opening> requiresNotification,
        Db db
    )
    {
        DateTime utcNow = DateTime.UtcNow;

        // infos are ordered by start
        foreach (OpeningInfo info in infos)
        {
            Opening? closestOpening = unfinishedOpenings.MinBy(o => (info.Start.UtcDateTime - o.StartUtc).Duration());
            if (closestOpening == null)
            {
                // not yet seen opening, add it to db
                Opening opening = new()
                {
                    Groups = { group },
                    CreatedUtc = utcNow,
                    StartUtc = info.Start.UtcDateTime,
                    Title = info.Title
                };
                db.Openings.Add(opening);
                requiresNotification.Add(opening);
            }
            else
            {
                // assume the closest one to be the same opening
                unfinishedOpenings.Remove(closestOpening); // mark it as existing
                closestOpening.Title = info.Title; // update if changed
                closestOpening.StartUtc = info.Start.UtcDateTime;
            }
        }

        foreach (Opening opening in unfinishedOpenings)
        {
            bool hasStarted = opening.StartUtc <= utcNow;
            if (hasStarted)
            {
                // disappeared because it ended
                opening.EndUtc = utcNow;
            }
            else
            {
                // disappeared without starting, probably cancelled
                db.Openings.Remove(opening);
            }
        }
    }

    record OpeningInfo(string GroupName, DateTimeOffset Start, string Title);
}
