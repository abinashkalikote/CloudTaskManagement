﻿// <auto-generated />
using System;
using CTM.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace App.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20231010042712_TelegramMessageIdAdd")]
    partial class TelegramMessageIdAdd
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("App.Model.AuditTask", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AuditBy")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RecById")
                        .HasColumnType("int");

                    b.Property<DateTime>("RecDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Remarks")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TaskId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RecById");

                    b.HasIndex("TaskId");

                    b.ToTable("AuditTasks");
                });

            modelBuilder.Entity("App.Model.CloudTask", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClientAddress")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClientName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CloudUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CompleteTime")
                        .HasColumnType("datetime2");

                    b.Property<int?>("CompletedById")
                        .HasColumnType("int");

                    b.Property<string>("IssueOnPreviousSoftware")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LicDate")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PANNo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Priority")
                        .HasColumnType("nvarchar(1)");

                    b.Property<int?>("ProccedById")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ProccedTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("RecAuditLog")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RecById")
                        .HasColumnType("int");

                    b.Property<DateTime>("RecDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("RecStatus")
                        .HasColumnType("nvarchar(1)");

                    b.Property<int>("RecVersion")
                        .HasColumnType("int");

                    b.Property<string>("Remarks")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SoftwareVersionFrom")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SoftwareVersionTo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TSKStatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TaskName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TaskTime")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TaskTypeId")
                        .HasColumnType("int");

                    b.Property<int?>("TelegramMessageId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CompletedById");

                    b.HasIndex("ProccedById");

                    b.HasIndex("RecById");

                    b.HasIndex("TaskTypeId");

                    b.ToTable("CloudTasks");
                });

            modelBuilder.Entity("App.Model.CloudTaskLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CloudTaskId")
                        .HasColumnType("int");

                    b.Property<string>("CloudTaskStatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("RecDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Remarks")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CloudTaskId");

                    b.HasIndex("UserId");

                    b.ToTable("CloudTasksLog");
                });

            modelBuilder.Entity("App.Model.TaskType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("RecById")
                        .HasColumnType("int");

                    b.Property<DateTime>("RecDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("RecStatus")
                        .HasColumnType("nvarchar(1)");

                    b.Property<string>("TaskTypeName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("RecById");

                    b.ToTable("TaskTypes");
                });

            modelBuilder.Entity("App.Model.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IsAdmin")
                        .IsRequired()
                        .HasColumnType("nvarchar(1)");

                    b.Property<string>("IsNewPassword")
                        .IsRequired()
                        .HasColumnType("nvarchar(1)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RecBy")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("RecDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("RecStatus")
                        .HasColumnType("nvarchar(1)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("App.Model.AuditTask", b =>
                {
                    b.HasOne("App.Model.User", "RecBy")
                        .WithMany()
                        .HasForeignKey("RecById")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("App.Model.CloudTask", "Task")
                        .WithMany()
                        .HasForeignKey("TaskId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("RecBy");

                    b.Navigation("Task");
                });

            modelBuilder.Entity("App.Model.CloudTask", b =>
                {
                    b.HasOne("App.Model.User", "CompletedBy")
                        .WithMany()
                        .HasForeignKey("CompletedById")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("App.Model.User", "ProccedBy")
                        .WithMany()
                        .HasForeignKey("ProccedById")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("App.Model.User", "RecBy")
                        .WithMany()
                        .HasForeignKey("RecById")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("App.Model.TaskType", "TaskType")
                        .WithMany()
                        .HasForeignKey("TaskTypeId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("CompletedBy");

                    b.Navigation("ProccedBy");

                    b.Navigation("RecBy");

                    b.Navigation("TaskType");
                });

            modelBuilder.Entity("App.Model.CloudTaskLog", b =>
                {
                    b.HasOne("App.Model.CloudTask", "CloudTask")
                        .WithMany("CloudTaskLogs")
                        .HasForeignKey("CloudTaskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("App.Model.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("CloudTask");

                    b.Navigation("User");
                });

            modelBuilder.Entity("App.Model.TaskType", b =>
                {
                    b.HasOne("App.Model.User", "RecBy")
                        .WithMany()
                        .HasForeignKey("RecById")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("RecBy");
                });

            modelBuilder.Entity("App.Model.CloudTask", b =>
                {
                    b.Navigation("CloudTaskLogs");
                });
#pragma warning restore 612, 618
        }
    }
}
