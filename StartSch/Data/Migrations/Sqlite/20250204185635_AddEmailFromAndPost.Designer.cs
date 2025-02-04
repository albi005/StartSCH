﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using StartSch.Data;

#nullable disable

namespace StartSch.Data.Migrations.Sqlite
{
    [DbContext(typeof(SqliteDb))]
    [Migration("20250204185635_AddEmailFromAndPost")]
    partial class AddEmailFromAndPost
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.0");

            modelBuilder.Entity("EventGroup", b =>
                {
                    b.Property<int>("EventsId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("GroupsId")
                        .HasColumnType("INTEGER");

                    b.HasKey("EventsId", "GroupsId");

                    b.HasIndex("GroupsId");

                    b.ToTable("EventGroup");
                });

            modelBuilder.Entity("GroupPost", b =>
                {
                    b.Property<int>("GroupsId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PostsId")
                        .HasColumnType("INTEGER");

                    b.HasKey("GroupsId", "PostsId");

                    b.HasIndex("PostsId");

                    b.ToTable("GroupPost");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.DataProtection.EntityFrameworkCore.DataProtectionKey", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("FriendlyName")
                        .HasColumnType("TEXT");

                    b.Property<string>("Xml")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("DataProtectionKeys");
                });

            modelBuilder.Entity("StartSch.Data.Email", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ContentHtml")
                        .IsRequired()
                        .HasMaxLength(50000)
                        .HasColumnType("TEXT");

                    b.Property<string>("From")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<int>("PostId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Subject")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("PostId");

                    b.ToTable("Emails");
                });

            modelBuilder.Entity("StartSch.Data.Event", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedUtc")
                        .HasColumnType("TEXT");

                    b.Property<string>("DescriptionMarkdown")
                        .HasMaxLength(50000)
                        .HasColumnType("TEXT");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasMaxLength(8)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("EndUtc")
                        .HasColumnType("TEXT");

                    b.Property<int?>("ParentId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("StartUtc")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(130)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.ToTable("Events");

                    b.HasDiscriminator().HasValue("Event");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("StartSch.Data.Group", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("PekId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("PekName")
                        .HasMaxLength(40)
                        .HasColumnType("TEXT");

                    b.Property<string>("PincerName")
                        .HasMaxLength(40)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("PekId")
                        .IsUnique();

                    b.HasIndex("PincerName")
                        .IsUnique();

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("StartSch.Data.Post", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ContentMarkdown")
                        .HasMaxLength(50000)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedUtc")
                        .HasColumnType("TEXT");

                    b.Property<int?>("EventId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ExcerptMarkdown")
                        .HasMaxLength(1000)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("PublishedUtc")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(130)
                        .HasColumnType("TEXT");

                    b.Property<string>("Url")
                        .HasMaxLength(500)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("EventId");

                    b.HasIndex("Url")
                        .IsUnique();

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("StartSch.Data.PushMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasMaxLength(2000)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("PushMessages");
                });

            modelBuilder.Entity("StartSch.Data.PushSubscription", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Auth")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("Endpoint")
                        .IsRequired()
                        .HasMaxLength(2000)
                        .HasColumnType("TEXT");

                    b.Property<string>("P256DH")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<Guid>("UserId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Endpoint")
                        .IsUnique();

                    b.HasIndex("UserId");

                    b.ToTable("PushSubscriptions");
                });

            modelBuilder.Entity("StartSch.Data.QueuedMessageRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedUtc")
                        .HasColumnType("TEXT");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasMaxLength(34)
                        .HasColumnType("TEXT");

                    b.Property<Guid>("UserId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("QueuedMessageRequests");

                    b.HasDiscriminator().HasValue("QueuedMessageRequest");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("StartSch.Data.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("StartSch.Data.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("AuthSchEmail")
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.Property<string>("StartSchEmail")
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.Property<bool>("StartSchEmailVerified")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("StartSch.Data.UserTagSelection", b =>
                {
                    b.Property<int>("TagId")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("UserId")
                        .HasColumnType("TEXT");

                    b.HasKey("TagId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("UserTagSelections");
                });

            modelBuilder.Entity("StartSch.Data.Opening", b =>
                {
                    b.HasBaseType("StartSch.Data.Event");

                    b.Property<DateTime?>("OrderingEndUtc")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("OrderingStartUtc")
                        .HasColumnType("TEXT");

                    b.HasDiscriminator().HasValue("Opening");
                });

            modelBuilder.Entity("StartSch.Data.UserEmailRequest", b =>
                {
                    b.HasBaseType("StartSch.Data.QueuedMessageRequest");

                    b.Property<int>("EmailId")
                        .HasColumnType("INTEGER");

                    b.HasIndex("EmailId");

                    b.HasDiscriminator().HasValue("UserEmailRequest");
                });

            modelBuilder.Entity("StartSch.Data.UserPushMessageRequest", b =>
                {
                    b.HasBaseType("StartSch.Data.QueuedMessageRequest");

                    b.Property<int>("PushMessageId")
                        .HasColumnType("INTEGER");

                    b.HasIndex("PushMessageId");

                    b.HasDiscriminator().HasValue("UserPushMessageRequest");
                });

            modelBuilder.Entity("EventGroup", b =>
                {
                    b.HasOne("StartSch.Data.Event", null)
                        .WithMany()
                        .HasForeignKey("EventsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("StartSch.Data.Group", null)
                        .WithMany()
                        .HasForeignKey("GroupsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GroupPost", b =>
                {
                    b.HasOne("StartSch.Data.Group", null)
                        .WithMany()
                        .HasForeignKey("GroupsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("StartSch.Data.Post", null)
                        .WithMany()
                        .HasForeignKey("PostsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("StartSch.Data.Email", b =>
                {
                    b.HasOne("StartSch.Data.Post", "Post")
                        .WithMany()
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Post");
                });

            modelBuilder.Entity("StartSch.Data.Event", b =>
                {
                    b.HasOne("StartSch.Data.Event", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("StartSch.Data.Post", b =>
                {
                    b.HasOne("StartSch.Data.Event", "Event")
                        .WithMany("Posts")
                        .HasForeignKey("EventId");

                    b.Navigation("Event");
                });

            modelBuilder.Entity("StartSch.Data.PushSubscription", b =>
                {
                    b.HasOne("StartSch.Data.User", "User")
                        .WithMany("PushSubscriptions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("StartSch.Data.QueuedMessageRequest", b =>
                {
                    b.HasOne("StartSch.Data.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("StartSch.Data.UserTagSelection", b =>
                {
                    b.HasOne("StartSch.Data.Tag", "Tag")
                        .WithMany()
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("StartSch.Data.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Tag");

                    b.Navigation("User");
                });

            modelBuilder.Entity("StartSch.Data.UserEmailRequest", b =>
                {
                    b.HasOne("StartSch.Data.Email", "Email")
                        .WithMany("Requests")
                        .HasForeignKey("EmailId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Email");
                });

            modelBuilder.Entity("StartSch.Data.UserPushMessageRequest", b =>
                {
                    b.HasOne("StartSch.Data.PushMessage", "PushMessage")
                        .WithMany("Requests")
                        .HasForeignKey("PushMessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PushMessage");
                });

            modelBuilder.Entity("StartSch.Data.Email", b =>
                {
                    b.Navigation("Requests");
                });

            modelBuilder.Entity("StartSch.Data.Event", b =>
                {
                    b.Navigation("Children");

                    b.Navigation("Posts");
                });

            modelBuilder.Entity("StartSch.Data.PushMessage", b =>
                {
                    b.Navigation("Requests");
                });

            modelBuilder.Entity("StartSch.Data.User", b =>
                {
                    b.Navigation("PushSubscriptions");
                });
#pragma warning restore 612, 618
        }
    }
}
