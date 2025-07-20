using Microsoft.EntityFrameworkCore;
using ToDoListManagement.Entity.Models;

namespace ToDoListManagement.Entity.Data;

public partial class ToDoListDbContext : DbContext
{
    public ToDoListDbContext(DbContextOptions<ToDoListDbContext> options)
        : base(options)
    {

    }

    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectUser> ProjectUsers { get; set; }
    public DbSet<TaskAttachment> TaskAttachments { get; set; }
    public DbSet<ToDoList> ToDoLists { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<ErrorLog> ErrorLogs { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<Sprint> Sprints { get; set; }
    public DbSet<Leave> Leaves { get; set; }
    public DbSet<TeamUserMapping> TeamUserMappings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Role>().HasData(
            new Role
            {
                RoleId = 1,
                RoleName = "Admin",
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Role>().HasData(
            new Role
            {
                RoleId = 2,
                RoleName = "Program Manager",
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Role>().HasData(
            new Role
            {
                RoleId = 3,
                RoleName = "Member",
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Role>().HasData(
            new Role
            {
                RoleId = 4,
                RoleName = "Scrum Master",
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Role>().HasData(
            new Role
            {
                RoleId = 5,
                RoleName = "Project Manager",
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Role>().HasData(
            new Role
            {
                RoleId = 6,
                RoleName = "Associative Project Manager",
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Role>().HasData(
            new Role
            {
                RoleId = 7,
                RoleName = "Senior Team Leader",
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Role>().HasData(
            new Role
            {
                RoleId = 8,
                RoleName = "Team Leader",
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Role>().HasData(
            new Role
            {
                RoleId = 9,
                RoleName = "Senior Software Engineer",
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Role>().HasData(
            new Role
            {
                RoleId = 10,
                RoleName = "Software Engineer",
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Role>().HasData(
            new Role
            {
                RoleId = 11,
                RoleName = "Trainee Software Engineer",
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 1,
                PermissionName = Constants.Constants.ProjectModule,
                RoleId = 1,
                CanView = true,
                CanAddEdit = true,
                CanDelete = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 2,
                PermissionName = Constants.Constants.EmployeeModule,
                RoleId = 1,
                CanView = true,
                CanAddEdit = true,
                CanDelete = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 3,
                PermissionName = Constants.Constants.TaskBoardModule,
                RoleId = 1,
                CanView = true,
                CanAddEdit = true,
                CanDelete = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 4,
                PermissionName = Constants.Constants.RolePermissionModule,
                RoleId = 1,
                CanView = true,
                CanAddEdit = true,
                CanDelete = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 5,
                PermissionName = Constants.Constants.SelfLeaveModule,
                RoleId = 1,
                CanView = true,
                CanAddEdit = true,
                CanDelete = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 6,
                PermissionName = Constants.Constants.TeamLeaveModule,
                RoleId = 1,
                CanView = true,
                CanAddEdit = true,
                CanDelete = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 7,
                PermissionName = Constants.Constants.ManageTeamModule,
                RoleId = 1,
                CanView = true,
                CanAddEdit = true,
                CanDelete = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 8,
                PermissionName = Constants.Constants.ProjectModule,
                RoleId = 2,
                CanView = true,
                CanAddEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 9,
                PermissionName = Constants.Constants.EmployeeModule,
                RoleId = 2,
                CanView = true,
                CanAddEdit = true,
                CanDelete = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 10,
                PermissionName = Constants.Constants.TaskBoardModule,
                RoleId = 2,
                CanView = true,
                CanAddEdit = true,
                CanDelete = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 11,
                PermissionName = Constants.Constants.RolePermissionModule,
                RoleId = 2,
                CanView = false,
                CanAddEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 12,
                PermissionName = Constants.Constants.SelfLeaveModule,
                RoleId = 2,
                CanView = true,
                CanAddEdit = true,
                CanDelete = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 13,
                PermissionName = Constants.Constants.TeamLeaveModule,
                RoleId = 2,
                CanView = false,
                CanAddEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 14,
                PermissionName = Constants.Constants.ManageTeamModule,
                RoleId = 2,
                CanView = true,
                CanAddEdit = true,
                CanDelete = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 15,
                PermissionName = Constants.Constants.ProjectModule,
                RoleId = 3,
                CanView = true,
                CanAddEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 16,
                PermissionName = Constants.Constants.EmployeeModule,
                RoleId = 3,
                CanView = true,
                CanAddEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 17,
                PermissionName = Constants.Constants.TaskBoardModule,
                RoleId = 3,
                CanView = true,
                CanAddEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 18,
                PermissionName = Constants.Constants.RolePermissionModule,
                RoleId = 3,
                CanView = false,
                CanAddEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 19,
                PermissionName = Constants.Constants.SelfLeaveModule,
                RoleId = 3,
                CanView = true,
                CanAddEdit = true,
                CanDelete = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 20,
                PermissionName = Constants.Constants.TeamLeaveModule,
                RoleId = 3,
                CanView = false,
                CanAddEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 21,
                PermissionName = Constants.Constants.ManageTeamModule,
                RoleId = 3,
                CanView = true,
                CanAddEdit = true,
                CanDelete = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 22,
                PermissionName = Constants.Constants.ProjectModule,
                RoleId = 4,
                CanView = true,
                CanAddEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 23,
                PermissionName = Constants.Constants.EmployeeModule,
                RoleId = 4,
                CanView = true,
                CanAddEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 24,
                PermissionName = Constants.Constants.TaskBoardModule,
                RoleId = 4,
                CanView = true,
                CanAddEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 25,
                PermissionName = Constants.Constants.RolePermissionModule,
                RoleId = 4,
                CanView = false,
                CanAddEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 26,
                PermissionName = Constants.Constants.SelfLeaveModule,
                RoleId = 4,
                CanView = true,
                CanAddEdit = true,
                CanDelete = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 27,
                PermissionName = Constants.Constants.TeamLeaveModule,
                RoleId = 4,
                CanView = false,
                CanAddEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 28,
                PermissionName = Constants.Constants.ManageTeamModule,
                RoleId = 4,
                CanView = true,
                CanAddEdit = true,
                CanDelete = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 29,
                PermissionName = Constants.Constants.ProjectModule,
                RoleId = 5,
                CanView = true,
                CanAddEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 30,
                PermissionName = Constants.Constants.EmployeeModule,
                RoleId = 5,
                CanView = true,
                CanAddEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 31,
                PermissionName = Constants.Constants.TaskBoardModule,
                RoleId = 5,
                CanView = true,
                CanAddEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 32,
                PermissionName = Constants.Constants.RolePermissionModule,
                RoleId = 5,
                CanView = false,
                CanAddEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 33,
                PermissionName = Constants.Constants.SelfLeaveModule,
                RoleId = 5,
                CanView = true,
                CanAddEdit = true,
                CanDelete = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 34,
                PermissionName = Constants.Constants.TeamLeaveModule,
                RoleId = 5,
                CanView = false,
                CanAddEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 35,
                PermissionName = Constants.Constants.ManageTeamModule,
                RoleId = 5,
                CanView = true,
                CanAddEdit = true,
                CanDelete = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 36,
                PermissionName = Constants.Constants.ProjectModule,
                RoleId = 6,
                CanView = true,
                CanAddEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 37,
                PermissionName = Constants.Constants.EmployeeModule,
                RoleId = 6,
                CanView = true,
                CanAddEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 38,
                PermissionName = Constants.Constants.TaskBoardModule,
                RoleId = 6,
                CanView = true,
                CanAddEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 39,
                PermissionName = Constants.Constants.RolePermissionModule,
                RoleId = 6,
                CanView = false,
                CanAddEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 40,
                PermissionName = Constants.Constants.SelfLeaveModule,
                RoleId = 6,
                CanView = true,
                CanAddEdit = true,
                CanDelete = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 41,
                PermissionName = Constants.Constants.TeamLeaveModule,
                RoleId = 6,
                CanView = false,
                CanAddEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 42,
                PermissionName = Constants.Constants.ManageTeamModule,
                RoleId = 6,
                CanView = true,
                CanAddEdit = true,
                CanDelete = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 43,
                PermissionName = Constants.Constants.ProjectModule,
                RoleId = 7,
                CanView = true,
                CanAddEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 44,
                PermissionName = Constants.Constants.EmployeeModule,
                RoleId = 7,
                CanView = true,
                CanAddEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 45,
                PermissionName = Constants.Constants.TaskBoardModule,
                RoleId = 7,
                CanView = true,
                CanAddEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 46,
                PermissionName = Constants.Constants.RolePermissionModule,
                RoleId = 7,
                CanView = false,
                CanAddEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 47,
                PermissionName = Constants.Constants.SelfLeaveModule,
                RoleId = 7,
                CanView = true,
                CanAddEdit = true,
                CanDelete = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 48,
                PermissionName = Constants.Constants.TeamLeaveModule,
                RoleId = 7,
                CanView = false,
                CanAddEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 49,
                PermissionName = Constants.Constants.ManageTeamModule,
                RoleId = 7,
                CanView = true,
                CanAddEdit = true,
                CanDelete = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 50,
                PermissionName = Constants.Constants.ProjectModule,
                RoleId = 8,
                CanView = true,
                CanAddEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 51,
                PermissionName = Constants.Constants.EmployeeModule,
                RoleId = 8,
                CanView = true,
                CanAddEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 52,
                PermissionName = Constants.Constants.TaskBoardModule,
                RoleId = 8,
                CanView = true,
                CanAddEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 53,
                PermissionName = Constants.Constants.RolePermissionModule,
                RoleId = 8,
                CanView = false,
                CanAddEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 54,
                PermissionName = Constants.Constants.SelfLeaveModule,
                RoleId = 8,
                CanView = true,
                CanAddEdit = true,
                CanDelete = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 55,
                PermissionName = Constants.Constants.TeamLeaveModule,
                RoleId = 8,
                CanView = false,
                CanAddEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 56,
                PermissionName = Constants.Constants.ManageTeamModule,
                RoleId = 8,
                CanView = true,
                CanAddEdit = true,
                CanDelete = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 57,
                PermissionName = Constants.Constants.ProjectModule,
                RoleId = 9,
                CanView = true,
                CanAddEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 58,
                PermissionName = Constants.Constants.EmployeeModule,
                RoleId = 9,
                CanView = true,
                CanAddEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 59,
                PermissionName = Constants.Constants.TaskBoardModule,
                RoleId = 9,
                CanView = true,
                CanAddEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 60,
                PermissionName = Constants.Constants.RolePermissionModule,
                RoleId = 9,
                CanView = false,
                CanAddEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 61,
                PermissionName = Constants.Constants.SelfLeaveModule,
                RoleId = 9,
                CanView = true,
                CanAddEdit = true,
                CanDelete = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 62,
                PermissionName = Constants.Constants.TeamLeaveModule,
                RoleId = 9,
                CanView = false,
                CanAddEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 63,
                PermissionName = Constants.Constants.ManageTeamModule,
                RoleId = 9,
                CanView = true,
                CanAddEdit = true,
                CanDelete = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 64,
                PermissionName = Constants.Constants.ProjectModule,
                RoleId = 10,
                CanView = true,
                CanAddEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 65,
                PermissionName = Constants.Constants.EmployeeModule,
                RoleId = 10,
                CanView = true,
                CanAddEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 66,
                PermissionName = Constants.Constants.TaskBoardModule,
                RoleId = 10,
                CanView = true,
                CanAddEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 67,
                PermissionName = Constants.Constants.RolePermissionModule,
                RoleId = 10,
                CanView = false,
                CanAddEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 68,
                PermissionName = Constants.Constants.SelfLeaveModule,
                RoleId = 10,
                CanView = true,
                CanAddEdit = true,
                CanDelete = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 69,
                PermissionName = Constants.Constants.TeamLeaveModule,
                RoleId = 10,
                CanView = false,
                CanAddEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 70,
                PermissionName = Constants.Constants.ManageTeamModule,
                RoleId = 10,
                CanView = true,
                CanAddEdit = true,
                CanDelete = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 71,
                PermissionName = Constants.Constants.ProjectModule,
                RoleId = 11,
                CanView = true,
                CanAddEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 72,
                PermissionName = Constants.Constants.EmployeeModule,
                RoleId = 11,
                CanView = true,
                CanAddEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 73,
                PermissionName = Constants.Constants.TaskBoardModule,
                RoleId = 11,
                CanView = true,
                CanAddEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 74,
                PermissionName = Constants.Constants.RolePermissionModule,
                RoleId = 11,
                CanView = false,
                CanAddEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 75,
                PermissionName = Constants.Constants.SelfLeaveModule,
                RoleId = 11,
                CanView = true,
                CanAddEdit = true,
                CanDelete = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 76,
                PermissionName = Constants.Constants.TeamLeaveModule,
                RoleId = 11,
                CanView = false,
                CanAddEdit = false,
                CanDelete = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = 77,
                PermissionName = Constants.Constants.ManageTeamModule,
                RoleId = 11,
                CanView = true,
                CanAddEdit = true,
                CanDelete = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );
        
        modelBuilder.Entity<User>().HasData(
            new User 
            {
                UserId = 1,
                Name = "Admin",
                Email = "admin@outlook.com",
                PasswordHash = "$2a$11$YCiiJxUwumHUtegC05ahFej29UzVm/s1HRwPriPIuta4b.GWddWuW",
                RoleId = 1,
                IsDeleted = false,
                IsActive = true,
                PhoneNumber = "9988556644"
            }
        );

        modelBuilder.Entity<User>()
        .HasIndex(u => u.Email)
        .IsUnique();
    }
}