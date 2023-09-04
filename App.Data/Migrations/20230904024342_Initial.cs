using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAdmin = table.Column<bool>(type: "bit", nullable: false),
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TaskTypeId = table.Column<int>(type: "int", nullable: false),
                    ClientName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(1)", nullable: true),
                    TaskTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CloudUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IssueOnPreviousSoftware = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoftwareVersionFrom = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoftwareVersionTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecAuditLog = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecVersion = table.Column<int>(type: "int", nullable: false),
                    RecStatus = table.Column<string>(type: "nvarchar(1)", nullable: true),
                    RecDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RecById = table.Column<int>(type: "int", nullable: false),
                    ProccedById = table.Column<int>(type: "int", nullable: true),
                    CompletedById = table.Column<int>(type: "int", nullable: true)
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
                });

            migrationBuilder.CreateTable(
                name: "AuditTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskId = table.Column<int>(type: "int", nullable: false),
                    AuditBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RecById = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditTasks_CloudTasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "CloudTasks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AuditTasks_Users_RecById",
                        column: x => x.RecById,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditTasks_RecById",
                table: "AuditTasks",
                column: "RecById");

            migrationBuilder.CreateIndex(
                name: "IX_AuditTasks_TaskId",
                table: "AuditTasks",
                column: "TaskId");

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
                name: "IX_TaskTypes_RecById",
                table: "TaskTypes",
                column: "RecById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditTasks");

            migrationBuilder.DropTable(
                name: "CloudTasks");

            migrationBuilder.DropTable(
                name: "TaskTypes");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
