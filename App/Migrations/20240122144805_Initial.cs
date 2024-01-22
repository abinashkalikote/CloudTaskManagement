using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Web.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "app_client",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Link = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentVersion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsMobileBankingClient = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_app_client", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAdmin = table.Column<string>(type: "nvarchar(1)", nullable: false),
                    IsNewPassword = table.Column<string>(type: "nvarchar(1)", nullable: false),
                    RecStatus = table.Column<string>(type: "nvarchar(1)", nullable: true),
                    RecDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RecBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaskTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecStatus = table.Column<string>(type: "nvarchar(1)", nullable: true),
                    RecDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RecById = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskTypes_Users_RecById",
                        column: x => x.RecById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CloudTasks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TaskTypeId = table.Column<int>(type: "int", nullable: false),
                    ClientId = table.Column<long>(type: "bigint", nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(1)", nullable: true),
                    TaskTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CloudUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IssueOnPreviousSoftware = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoftwareVersionFrom = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoftwareVersionTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LicDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecAuditLog = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecVersion = table.Column<int>(type: "int", nullable: false),
                    RecStatus = table.Column<string>(type: "nvarchar(1)", nullable: true),
                    RecDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TSKStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProccedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompleteTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RecById = table.Column<int>(type: "int", nullable: false),
                    ProccedById = table.Column<int>(type: "int", nullable: true),
                    CompletedById = table.Column<int>(type: "int", nullable: true),
                    TelegramMessageId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CloudTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CloudTasks_TaskTypes_TaskTypeId",
                        column: x => x.TaskTypeId,
                        principalTable: "TaskTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CloudTasks_Users_CompletedById",
                        column: x => x.CompletedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CloudTasks_Users_ProccedById",
                        column: x => x.ProccedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CloudTasks_Users_RecById",
                        column: x => x.RecById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CloudTasks_app_client_ClientId",
                        column: x => x.ClientId,
                        principalTable: "app_client",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CloudTasksLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CloudTaskId1 = table.Column<long>(type: "bigint", nullable: false),
                    CloudTaskId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CloudTaskStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CloudTasksLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CloudTasksLog_CloudTasks_CloudTaskId1",
                        column: x => x.CloudTaskId1,
                        principalTable: "CloudTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CloudTasksLog_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CloudTasks_ClientId",
                table: "CloudTasks",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_CloudTasks_CompletedById",
                table: "CloudTasks",
                column: "CompletedById");

            migrationBuilder.CreateIndex(
                name: "IX_CloudTasks_ProccedById",
                table: "CloudTasks",
                column: "ProccedById");

            migrationBuilder.CreateIndex(
                name: "IX_CloudTasks_RecById",
                table: "CloudTasks",
                column: "RecById");

            migrationBuilder.CreateIndex(
                name: "IX_CloudTasks_TaskTypeId",
                table: "CloudTasks",
                column: "TaskTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CloudTasksLog_CloudTaskId1",
                table: "CloudTasksLog",
                column: "CloudTaskId1");

            migrationBuilder.CreateIndex(
                name: "IX_CloudTasksLog_UserId",
                table: "CloudTasksLog",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskTypes_RecById",
                table: "TaskTypes",
                column: "RecById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CloudTasksLog");

            migrationBuilder.DropTable(
                name: "CloudTasks");

            migrationBuilder.DropTable(
                name: "TaskTypes");

            migrationBuilder.DropTable(
                name: "app_client");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
