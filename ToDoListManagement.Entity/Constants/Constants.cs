namespace ToDoListManagement.Entity.Constants;

public class Constants
{
    #region Error Messages

    public const string ErrorCode404 = "The page you are looking for was not found.";
    public const string ErrorCode500 = "An unexpected server error occurred.";
    public const string ErrorCode403 = "Unauthorized";
    public const string ErrorCode401 = "Unauthenticated";
    public const string ErrorCodeDefault = "An unexpected error occurred.";

    #endregion

    #region Error Status

    public const string ErrorStatus404 = "Oops, the page you're looking for isn't here.";
    public const string ErrorStatus500 = "An unexpected server error occurred.";
    public const string ErrorStatus403 = "You do not have permission to access this resource.";
    public const string ErrorStatus401 = "You are not authenticated to access this resource.";
    public const string ErrorStatusDefault = "An unexpected error occurred.";

    #endregion

    #region ViewModel Error
    
    public const string EmailRequiredError = "Email is required.";
    public const string EmailInvalidError = "Invalid Email.";
    public const string PasswordRequiredError = "Password is required.";
    public const string ValidPasswordMessage = "Password must contain minimum 8 characters and at least one uppercase letter, one lowercase letter, one number, and one special character (.@$!%*?&).";
    public const string NameRequiredError = "Name is required.";
    public const string NameMaxLengthError = "Name cannot be longer than 100 characters.";
    public const string EmailAlreadyExistsError = "Email already exists.";
    public const string ConfirmPasswordRequiredError = "Confirm Password is required.";
    public const string PasswordMismatchError = "Password and Confirm Password do not match.";
    public const string AssignedToPMRequiredError = "Assigned PM is required.";
    public const string AssignedToSMRequiredError = "Assigned SM is required.";
    public const string TitleRequiredError = "Title is required.";
    public const string AssignedToRequiredError = "Assigned To is required.";
    public const string StartDateRequiredError = "Start Date is required.";
    public const string DueDateRequiredError = "Due Date is required.";
    public const string EndDateRequiredError = "End Date is required.";
    public const string PriorityRequiredError = "Priority is required.";
    public const string RoleRequiredError = "Role is required.";
    public const string PhoneNumberRequiredError = "Phone Number is required.";
    public const string PhoneNumberInvalidError = "Invalid Phone Number.";
    public const string StatusRequiredError = "Status is required.";
    public const string RoleNameRequiredError = "Role Name is required.";
    public const string RoleNameAlreadyExistsError = "Role Name already exists.";
    public const string InvalidSessionMessage = "User session is invalid.";
    public const string NameExistsError = "Name already exists.";
    public const string TitleExistsError = "Title already exists.";

    #endregion

    #region Login

    public const string LoginSuccessMessage = "Login successful!";
    public const string LoginErrorMessage = "Invalid Credentials!";

    #endregion

    #region Registration

    public const string RegistrationSuccessMessage = "Registration successful!";
    public const string RegistrationErrorMessage = "Registration failed!";
    
    #endregion

    #region Forgot Password

    public const string EmailNotRegisteredMessage = "Email not registered!";
    public const string InvalidEmailMessage = "Invalid email address.";
    public const string InvalidResetPasswordLinkMessage = "Invalid reset password link.";

    #endregion

    #region Reset Password

    public const string ResetPasswordEmailSubject = "Reset Password";
    public const string ResetPasswordEmailBody = "Click the link below to reset your password: <a href='{ResetLink}' target='_blank'>Reset Password</a>";
    public const string ResetPasswordSuccessMessage = "Password reset successfully!";
    public const string ResetPasswordErrorMessage = "Failed to reset password.";

    #endregion

    #region Profile

    public const string ProfileUpdatedMessage = "Profile updated successfully.";
    public const string ProfileUpdateFailedMessage = "Failed to update the profile.";

    #endregion

    #region Project

    public const string ProjectAddedMessage = "Project added successfully.";
    public const string ProjectAddFailedMessage = "Failed to add the project.";
    public const string ProjectNotFoundMessage = "Project not found.";
    public const string ProjectUpdatedMessage = "Project updated successfully.";
    public const string ProjectUpdateFailedMessage = "Failed to update the project.";
    public const string ProjectDeletedMessage = "Project deleted successfully";
    public const string ProjectDeleteFailedMessage = "Failed to delete the project.";
    public const string MembersAssignedMessage = "Members assigned successfully.";
    public const string MembersAssignedFailedMessage = "Failed to assign members.";
    
    #endregion

    #region Emplyee

    public const string EmployeeAddedMessage = "Employee added successfully.";
    public const string EmployeeAddFailedMessage = "Failed to add the employee.";
    public const string EmployeeUpdatedMessage = "Employee data updated successfully.";
    public const string EmployeeNotFoundError = "Employee not found.";
    public const string EmployeeUpdateFailedMessage = "Failed to update the employee data.";
    public const string EmployeeDeletedMessage = "Employee deleted successfully";
    public const string EmployeeDeleteFailedMessage = "Failed to delete the employee.";

    #endregion

    #region Task

    public const string TaskAddedMessage = "Task added successfully.";
    public const string TaskAddFailedMessage = "Failed to add task.";
    public const string TaskTitleAlreadyExistsError = "Title already exists.";
    public const string TaskStausChangeMessage = "Task status updated successfully.";
    public const string TaskStausChangeFailedMessage = "Failed to update task status.";
    public const string TaskUpdatedMessage = "Task updated successfully.";
    public const string TaskUpdateFailedMessage = "Failed to update the task.";
    public const string TaskDeletedMessage = "Task deleted successfully.";
    public const string TaskDeleteFailedMessage = "Failed to delete task."; 
    public const string TaskAttachmentDeletedMessage = "Task deleted successfully.";
    public const string TaskAttachmentDeleteFailedMessage = "Failed to delete task.";

    #endregion

    #region Sprint

    public const string SprintAddedMessage = "Sprint added successfully.";
    public const string SprintAddFailedMessage = "Failed to add sprint.";
    public const string SprintStartedMessage = "Sprint started successfully.";
    public const string SprintStartFailedMessage = "Failed to start sprint.";
    public const string SprintCompletedMessage = "Sprint completed successfully.";
    public const string SprintCompleteFailedMessage = "Failed to complete sprint.";

    #endregion

    #region Role

    public const string RoleAddedMessage = "Role added successfully.";
    public const string RoleAddFailedMessage = "Failed to add the role.";
    public const string RoleNotFoundMessage = "Role not found.";
    public const string RoleUpdatedMessage = "Role updated successfully.";
    public const string RoleUpdateFailedMessage = "Failed to update the role.";
    public const string RoleDeletedMessage = "Role deleted successfully";
    public const string RoleDeleteFailedMessage = "Failed to delete the role.";
    public const string RoleDeleteAssignedUsersFailedMessage = "User(s) are assigned to this Role.";

    #endregion

    #region Permission

    public static readonly string[] Tabs = ["Projects", "Employees", "Task Board", "Role And Permissions", "Self Leave", "Team Leave", "Manage Team"];
    public const string PermissionUpdatedMessage = "Permissions updated successfully.";
    public const string PermissionUpdateFailedMessage = "Failed to update the permissions.";

    #endregion

    #region Leave

    public const string LeaveAddedMessage = "Leave added successfully.";
    public const string LeaveAddFailedMessage = "Failed to add the leave.";
    public const string LeaveNotFound = "Leave not found.";
    public const string LeaveUpdatedMessage = "Leave updated successfully.";
    public const string LeaveUpdateFailedMessage = "Failed to update the leave.";
    public const string LeaveDeletedMessage = "Leave deleted successfully";
    public const string LeaveDeleteFailedMessage = "Failed to delete the leave.";
    public const string LeaveRequestEmailSubject = "Leave Requested";
    public const string LeaveApprovedEmailSubject = "Leave Approved";
    public const string LeaveRejectedEmailSubject = "Leave Rejected";
    public const string LeaveApprovedMessage = "Your Leave from { StartDate } to { EndDate } has been approved.";
    public const string LeaveRejectedMessage = "Your Leave from { StartDate } to { EndDate } has been rejected.";    

    #endregion

    #region Modules

    public const string CanView = "CanView";
    public const string CanAddEdit = "CanAddEdit";
    public const string CanDelete = "CanDelete";
    public const string ProjectModule = "Projects";
    public const string EmployeeModule = "Employees";
    public const string TaskBoardModule = "Task Board";
    public const string RolePermissionModule = "Role And Permissions";
    public const string SelfLeaveModule = "Self Leave";
    public const string TeamLeaveModule = "Team Leave";
    public const string ManageTeamModule = "Manage Team";

    #endregion

    #region Team Manage

    public const string TeamMembersAddedMessage = "Team members added successfully.";
    public const string TeamMembersAddFailedMessage = "Failed to add the team members.";
    public const string TeamMemberGetFailedMessage = "Failed to get the team member.";
    public const string TeamMembersUpdatedMessage = "Team member data updated successfully.";
    public const string TeamMembersUpdatedFailedMessage = "Failed to update the team member data.";
    public const string TeamMembersRemovedMessage = "Team member removed successfully.";
    public const string TeamMembersRemoveFailedMessage = "Failed to remove the member.";
    
    #endregion
}