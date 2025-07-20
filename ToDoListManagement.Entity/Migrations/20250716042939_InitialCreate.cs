using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ToDoListManagement.Entity.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ErrorLogs",
                columns: table => new
                {
                    ErrorLogId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    StackTrace = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ErrorLogs", x => x.ErrorLogId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    PasswordHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ReportingPersonId = table.Column<int>(type: "integer", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Users_Users_ReportingPersonId",
                        column: x => x.ReportingPersonId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Leaves",
                columns: table => new
                {
                    LeaveId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RequestedUserId = table.Column<int>(type: "integer", nullable: true),
                    ApprovalUserId = table.Column<int>(type: "integer", nullable: true),
                    Reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ApprovedOn = table.Column<DateOnly>(type: "date", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    AlternatePhoneNumber = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    CommentByApproval = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsAvailableOnPhone = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leaves", x => x.LeaveId);
                    table.ForeignKey(
                        name: "FK_Leaves_Users_ApprovalUserId",
                        column: x => x.ApprovalUserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_Leaves_Users_RequestedUserId",
                        column: x => x.RequestedUserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    PermissionId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    PermissionName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CanView = table.Column<bool>(type: "boolean", nullable: false),
                    CanAddEdit = table.Column<bool>(type: "boolean", nullable: false),
                    CanDelete = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.PermissionId);
                    table.ForeignKey(
                        name: "FK_Permissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Permissions_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_Permissions_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    ProjectId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    AssignedToProgramManager = table.Column<int>(type: "integer", nullable: true),
                    AssignedToScrumMaster = table.Column<int>(type: "integer", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.ProjectId);
                    table.ForeignKey(
                        name: "FK_Projects_Users_AssignedToProgramManager",
                        column: x => x.AssignedToProgramManager,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_Projects_Users_AssignedToScrumMaster",
                        column: x => x.AssignedToScrumMaster,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_Projects_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "TeamUserMappings",
                columns: table => new
                {
                    TeamUserMappingId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TeamManagerId = table.Column<int>(type: "integer", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateOnly>(type: "date", nullable: true),
                    UpdatedAt = table.Column<DateOnly>(type: "date", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamUserMappings", x => x.TeamUserMappingId);
                    table.ForeignKey(
                        name: "FK_TeamUserMappings_Users_TeamManagerId",
                        column: x => x.TeamManagerId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_TeamUserMappings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "ProjectUsers",
                columns: table => new
                {
                    ProjectUserId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectId = table.Column<int>(type: "integer", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectUsers", x => x.ProjectUserId);
                    table.ForeignKey(
                        name: "FK_ProjectUsers_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "ProjectId");
                    table.ForeignKey(
                        name: "FK_ProjectUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Sprints",
                columns: table => new
                {
                    SprintId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SprintName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    ScrumMasterId = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sprints", x => x.SprintId);
                    table.ForeignKey(
                        name: "FK_Sprints_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sprints_Users_ScrumMasterId",
                        column: x => x.ScrumMasterId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ToDoLists",
                columns: table => new
                {
                    TaskId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectId = table.Column<int>(type: "integer", nullable: true),
                    SprintId = table.Column<int>(type: "integer", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    AssignedTo = table.Column<int>(type: "integer", nullable: true),
                    Title = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Priority = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToDoLists", x => x.TaskId);
                    table.ForeignKey(
                        name: "FK_ToDoLists_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "ProjectId");
                    table.ForeignKey(
                        name: "FK_ToDoLists_Sprints_SprintId",
                        column: x => x.SprintId,
                        principalTable: "Sprints",
                        principalColumn: "SprintId");
                    table.ForeignKey(
                        name: "FK_ToDoLists_Users_AssignedTo",
                        column: x => x.AssignedTo,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_ToDoLists_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "TaskAttachments",
                columns: table => new
                {
                    AttachmentId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TaskId = table.Column<int>(type: "integer", nullable: true),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    FilePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsImage = table.Column<bool>(type: "boolean", nullable: true),
                    UploadedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UploadedBy = table.Column<int>(type: "integer", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskAttachments", x => x.AttachmentId);
                    table.ForeignKey(
                        name: "FK_TaskAttachments_ToDoLists_TaskId",
                        column: x => x.TaskId,
                        principalTable: "ToDoLists",
                        principalColumn: "TaskId");
                    table.ForeignKey(
                        name: "FK_TaskAttachments_Users_DeletedBy",
                        column: x => x.DeletedBy,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_TaskAttachments_Users_UploadedBy",
                        column: x => x.UploadedBy,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "CreatedAt", "CreatedBy", "IsDeleted", "RoleName", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 7, 16, 4, 29, 39, 196, DateTimeKind.Utc).AddTicks(9251), null, false, "Admin", null, null },
                    { 2, new DateTime(2025, 7, 16, 4, 29, 39, 196, DateTimeKind.Utc).AddTicks(9313), null, false, "Program Manager", null, null },
                    { 3, new DateTime(2025, 7, 16, 4, 29, 39, 196, DateTimeKind.Utc).AddTicks(9341), null, false, "Member", null, null },
                    { 4, new DateTime(2025, 7, 16, 4, 29, 39, 196, DateTimeKind.Utc).AddTicks(9370), null, false, "Scrum Master", null, null },
                    { 5, new DateTime(2025, 7, 16, 4, 29, 39, 196, DateTimeKind.Utc).AddTicks(9397), null, false, "Project Manager", null, null },
                    { 6, new DateTime(2025, 7, 16, 4, 29, 39, 196, DateTimeKind.Utc).AddTicks(9428), null, false, "Associative Project Manager", null, null },
                    { 7, new DateTime(2025, 7, 16, 4, 29, 39, 196, DateTimeKind.Utc).AddTicks(9455), null, false, "Senior Team Leader", null, null },
                    { 8, new DateTime(2025, 7, 16, 4, 29, 39, 196, DateTimeKind.Utc).AddTicks(9482), null, false, "Team Leader", null, null },
                    { 9, new DateTime(2025, 7, 16, 4, 29, 39, 196, DateTimeKind.Utc).AddTicks(9509), null, false, "Senior Software Engineer", null, null },
                    { 10, new DateTime(2025, 7, 16, 4, 29, 39, 196, DateTimeKind.Utc).AddTicks(9552), null, false, "Software Engineer", null, null },
                    { 11, new DateTime(2025, 7, 16, 4, 29, 39, 196, DateTimeKind.Utc).AddTicks(9580), null, false, "Trainee Software Engineer", null, null }
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "PermissionId", "CanAddEdit", "CanDelete", "CanView", "CreatedAt", "CreatedBy", "IsDeleted", "PermissionName", "RoleId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, true, true, true, new DateTime(2025, 7, 16, 4, 29, 39, 196, DateTimeKind.Utc).AddTicks(9623), null, false, "Projects", 1, null, null },
                    { 2, true, true, true, new DateTime(2025, 7, 16, 4, 29, 39, 196, DateTimeKind.Utc).AddTicks(9659), null, false, "Employees", 1, null, null },
                    { 3, true, true, true, new DateTime(2025, 7, 16, 4, 29, 39, 196, DateTimeKind.Utc).AddTicks(9687), null, false, "Task Board", 1, null, null },
                    { 4, true, true, true, new DateTime(2025, 7, 16, 4, 29, 39, 196, DateTimeKind.Utc).AddTicks(9714), null, false, "Role And Permissions", 1, null, null },
                    { 5, true, true, true, new DateTime(2025, 7, 16, 4, 29, 39, 196, DateTimeKind.Utc).AddTicks(9741), null, false, "Self Leave", 1, null, null },
                    { 6, true, true, true, new DateTime(2025, 7, 16, 4, 29, 39, 196, DateTimeKind.Utc).AddTicks(9769), null, false, "Team Leave", 1, null, null },
                    { 7, true, true, true, new DateTime(2025, 7, 16, 4, 29, 39, 196, DateTimeKind.Utc).AddTicks(9796), null, false, "Manage Team", 1, null, null },
                    { 8, false, false, true, new DateTime(2025, 7, 16, 4, 29, 39, 196, DateTimeKind.Utc).AddTicks(9823), null, false, "Projects", 2, null, null },
                    { 9, true, true, true, new DateTime(2025, 7, 16, 4, 29, 39, 196, DateTimeKind.Utc).AddTicks(9851), null, false, "Employees", 2, null, null },
                    { 10, true, true, true, new DateTime(2025, 7, 16, 4, 29, 39, 196, DateTimeKind.Utc).AddTicks(9879), null, false, "Task Board", 2, null, null },
                    { 11, false, false, false, new DateTime(2025, 7, 16, 4, 29, 39, 196, DateTimeKind.Utc).AddTicks(9906), null, false, "Role And Permissions", 2, null, null },
                    { 12, true, true, true, new DateTime(2025, 7, 16, 4, 29, 39, 196, DateTimeKind.Utc).AddTicks(9932), null, false, "Self Leave", 2, null, null },
                    { 13, false, false, false, new DateTime(2025, 7, 16, 4, 29, 39, 196, DateTimeKind.Utc).AddTicks(9959), null, false, "Team Leave", 2, null, null },
                    { 14, true, true, true, new DateTime(2025, 7, 16, 4, 29, 39, 196, DateTimeKind.Utc).AddTicks(9986), null, false, "Manage Team", 2, null, null },
                    { 15, false, false, true, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(13), null, false, "Projects", 3, null, null },
                    { 16, false, false, true, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(42), null, false, "Employees", 3, null, null },
                    { 17, false, false, true, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(69), null, false, "Task Board", 3, null, null },
                    { 18, false, false, false, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(100), null, false, "Role And Permissions", 3, null, null },
                    { 19, true, true, true, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(128), null, false, "Self Leave", 3, null, null },
                    { 20, false, false, false, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(155), null, false, "Team Leave", 3, null, null },
                    { 21, true, true, true, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(183), null, false, "Manage Team", 3, null, null },
                    { 22, false, false, true, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(221), null, false, "Projects", 4, null, null },
                    { 23, false, false, true, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(250), null, false, "Employees", 4, null, null },
                    { 24, false, false, true, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(379), null, false, "Task Board", 4, null, null },
                    { 25, false, false, false, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(406), null, false, "Role And Permissions", 4, null, null },
                    { 26, true, true, true, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(433), null, false, "Self Leave", 4, null, null },
                    { 27, false, false, false, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(460), null, false, "Team Leave", 4, null, null },
                    { 28, true, true, true, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(487), null, false, "Manage Team", 4, null, null },
                    { 29, false, false, true, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(515), null, false, "Projects", 5, null, null },
                    { 30, false, false, true, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(542), null, false, "Employees", 5, null, null },
                    { 31, false, false, true, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(568), null, false, "Task Board", 5, null, null },
                    { 32, false, false, false, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(595), null, false, "Role And Permissions", 5, null, null },
                    { 33, true, true, true, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(623), null, false, "Self Leave", 5, null, null },
                    { 34, false, false, false, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(652), null, false, "Team Leave", 5, null, null },
                    { 35, true, true, true, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(679), null, false, "Manage Team", 5, null, null },
                    { 36, false, false, true, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(706), null, false, "Projects", 6, null, null },
                    { 37, false, false, true, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(732), null, false, "Employees", 6, null, null },
                    { 38, false, false, true, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(758), null, false, "Task Board", 6, null, null },
                    { 39, false, false, false, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(785), null, false, "Role And Permissions", 6, null, null },
                    { 40, true, true, true, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(814), null, false, "Self Leave", 6, null, null },
                    { 41, false, false, false, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(842), null, false, "Team Leave", 6, null, null },
                    { 42, true, true, true, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(871), null, false, "Manage Team", 6, null, null },
                    { 43, false, false, true, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(898), null, false, "Projects", 7, null, null },
                    { 44, false, false, true, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(926), null, false, "Employees", 7, null, null },
                    { 45, false, false, true, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(963), null, false, "Task Board", 7, null, null },
                    { 46, false, false, false, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(991), null, false, "Role And Permissions", 7, null, null },
                    { 47, true, true, true, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(1018), null, false, "Self Leave", 7, null, null },
                    { 48, false, false, false, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(1045), null, false, "Team Leave", 7, null, null },
                    { 49, true, true, true, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(1074), null, false, "Manage Team", 7, null, null },
                    { 50, false, false, true, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(1102), null, false, "Projects", 8, null, null },
                    { 51, false, false, true, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(1128), null, false, "Employees", 8, null, null },
                    { 52, false, false, true, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(1155), null, false, "Task Board", 8, null, null },
                    { 53, false, false, false, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(1182), null, false, "Role And Permissions", 8, null, null },
                    { 54, true, true, true, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(1208), null, false, "Self Leave", 8, null, null },
                    { 55, false, false, false, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(1235), null, false, "Team Leave", 8, null, null },
                    { 56, true, true, true, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(1262), null, false, "Manage Team", 8, null, null },
                    { 57, false, false, true, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(1287), null, false, "Projects", 9, null, null },
                    { 58, false, false, true, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(1315), null, false, "Employees", 9, null, null },
                    { 59, false, false, true, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(1341), null, false, "Task Board", 9, null, null },
                    { 60, false, false, false, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(1367), null, false, "Role And Permissions", 9, null, null },
                    { 61, true, true, true, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(1395), null, false, "Self Leave", 9, null, null },
                    { 62, false, false, false, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(1422), null, false, "Team Leave", 9, null, null },
                    { 63, true, true, true, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(1448), null, false, "Manage Team", 9, null, null },
                    { 64, false, false, true, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(1476), null, false, "Projects", 10, null, null },
                    { 65, false, false, true, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(1503), null, false, "Employees", 10, null, null },
                    { 66, false, false, true, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(1532), null, false, "Task Board", 10, null, null },
                    { 67, false, false, false, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(1569), null, false, "Role And Permissions", 10, null, null },
                    { 68, true, true, true, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(1597), null, false, "Self Leave", 10, null, null },
                    { 69, false, false, false, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(1624), null, false, "Team Leave", 10, null, null },
                    { 70, true, true, true, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(1652), null, false, "Manage Team", 10, null, null },
                    { 71, false, false, true, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(1678), null, false, "Projects", 11, null, null },
                    { 72, false, false, true, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(1705), null, false, "Employees", 11, null, null },
                    { 73, false, false, true, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(1732), null, false, "Task Board", 11, null, null },
                    { 74, false, false, false, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(1760), null, false, "Role And Permissions", 11, null, null },
                    { 75, true, true, true, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(1785), null, false, "Self Leave", 11, null, null },
                    { 76, false, false, false, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(1813), null, false, "Team Leave", 11, null, null },
                    { 77, true, true, true, new DateTime(2025, 7, 16, 4, 29, 39, 197, DateTimeKind.Utc).AddTicks(1841), null, false, "Manage Team", 11, null, null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Email", "IsActive", "IsDeleted", "Name", "PasswordHash", "PhoneNumber", "ReportingPersonId", "RoleId" },
                values: new object[] { 1, "admin@outlook.com", true, false, "Admin", "$2a$11$YCiiJxUwumHUtegC05ahFej29UzVm/s1HRwPriPIuta4b.GWddWuW", "9988556644", null, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_Leaves_ApprovalUserId",
                table: "Leaves",
                column: "ApprovalUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Leaves_RequestedUserId",
                table: "Leaves",
                column: "RequestedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_CreatedBy",
                table: "Permissions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_RoleId",
                table: "Permissions",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_UpdatedBy",
                table: "Permissions",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_AssignedToProgramManager",
                table: "Projects",
                column: "AssignedToProgramManager");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_AssignedToScrumMaster",
                table: "Projects",
                column: "AssignedToScrumMaster");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CreatedBy",
                table: "Projects",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUsers_ProjectId",
                table: "ProjectUsers",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUsers_UserId",
                table: "ProjectUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Sprints_ProjectId",
                table: "Sprints",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Sprints_ScrumMasterId",
                table: "Sprints",
                column: "ScrumMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskAttachments_DeletedBy",
                table: "TaskAttachments",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TaskAttachments_TaskId",
                table: "TaskAttachments",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskAttachments_UploadedBy",
                table: "TaskAttachments",
                column: "UploadedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TeamUserMappings_TeamManagerId",
                table: "TeamUserMappings",
                column: "TeamManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamUserMappings_UserId",
                table: "TeamUserMappings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ToDoLists_AssignedTo",
                table: "ToDoLists",
                column: "AssignedTo");

            migrationBuilder.CreateIndex(
                name: "IX_ToDoLists_CreatedBy",
                table: "ToDoLists",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ToDoLists_ProjectId",
                table: "ToDoLists",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ToDoLists_SprintId",
                table: "ToDoLists",
                column: "SprintId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_ReportingPersonId",
                table: "Users",
                column: "ReportingPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ErrorLogs");

            migrationBuilder.DropTable(
                name: "Leaves");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "ProjectUsers");

            migrationBuilder.DropTable(
                name: "TaskAttachments");

            migrationBuilder.DropTable(
                name: "TeamUserMappings");

            migrationBuilder.DropTable(
                name: "ToDoLists");

            migrationBuilder.DropTable(
                name: "Sprints");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
