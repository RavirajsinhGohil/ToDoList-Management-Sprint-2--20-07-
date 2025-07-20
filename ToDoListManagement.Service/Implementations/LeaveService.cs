using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ToDoListManagement.Entity.Constants;
using ToDoListManagement.Entity.Helper;
using ToDoListManagement.Entity.Models;
using ToDoListManagement.Entity.ViewModel;
using ToDoListManagement.Repository.Interfaces;
using ToDoListManagement.Service.Interfaces;

namespace ToDoListManagement.Service.Implementations;

public class LeaveService : ILeaveService
{
    private readonly ILeaveRepository _leaveRepository;
    private readonly IEmailService _emailService;
    private readonly IWebHostEnvironment _hostingEnvironment;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAuthRepository _authRepository;
    public LeaveService(ILeaveRepository leaveRepository, IEmailService emailService, IWebHostEnvironment hostingEnvironment, IHttpContextAccessor httpContextAccessor, IAuthRepository authRepository)
    {
        _leaveRepository = leaveRepository;
        _emailService = emailService;
        _hostingEnvironment = hostingEnvironment;
        _httpContextAccessor = httpContextAccessor;
        _authRepository = authRepository;
    }

    public async Task<Pagination<LeaveViewModel>> GetPaginatedLeavesAsync(Pagination<LeaveViewModel> pagination, int? userId)
    {
        Pagination<Leave> leavePagination = new()
        {
            SearchKeyword = pagination.SearchKeyword,
            CurrentPage = pagination.CurrentPage,
            PageSize = pagination.PageSize,
            SortColumn = pagination.SortColumn,
            SortDirection = pagination.SortDirection,
            StatusFilter = pagination.StatusFilter,
            StartDate = pagination.StartDate,
            EndDate = pagination.EndDate
        };

        Pagination<Leave> leaves = await _leaveRepository.GetPaginatedLeavesAsync(leavePagination, userId);

        List<LeaveViewModel> leaveViewModels = [];
        foreach (Leave leave in leaves.Items)
        {
            leaveViewModels.Add(new LeaveViewModel
            {
                LeaveId = leave.LeaveId,
                RequestedUserId = leave.RequestedUserId,
                ApprovalUserId = leave.ApprovalUserId,
                RequestedName = leave.RequestedUser?.Name ?? string.Empty,
                ApprovalName = leave.ApprovalUser?.Name ?? string.Empty,
                ApprovedOn = leave.ApprovedOn?.ToString("dd-MM-yyyy"),
                IsAvailableOnPhone = leave.IsAvailableOnPhone,
                Reason = leave.Reason ?? string.Empty,
                StartDate = leave.StartDate?.ToString("dd-MM-yyyy"),
                EndDate = leave.EndDate?.ToString("dd-MM-yyyy"),
                Duration = leave.EndDate.HasValue && leave.StartDate.HasValue
                    ? (leave.EndDate.Value.ToDateTime(TimeOnly.MinValue) - leave.StartDate.Value.ToDateTime(TimeOnly.MinValue)).Days + 1
                    : null,
                ReturnDate = leave.EndDate?.AddDays(1).ToString("dd-MM-yyyy"),
                Status = leave.Status,
                CreatedAt = leave.CreatedAt?.ToString("dd-MM-yyyy"),
            });
        }

        return new Pagination<LeaveViewModel>
        {
            Items = leaveViewModels,
            TotalPages = leaves.TotalPages,
            TotalRecords = leaves.TotalRecords
        };
    }

    public async Task<bool> AddLeaveAsync(LeaveViewModel model, UserViewModel user)
    {
        Leave? leave = new()
        {
            RequestedUserId = user.UserId,
            ApprovalUserId = user.UserId,
            Reason = model.Reason?.Trim(),
            StartDate = model.StartDate != null ? DateOnly.Parse(model.StartDate) : null,
            EndDate = model.EndDate != null ? DateOnly.Parse(model.EndDate) : null,
            IsAvailableOnPhone = model.IsAvailableOnPhone,
            Status = "Pending",
            PhoneNumber = model.PhoneNumber,
            AlternatePhoneNumber = model.AlternatePhoneNumber,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        User admin = await _authRepository.GetUserByIdAsync(1);

        bool isAdded = await _leaveRepository.AddAsync(leave);
        if (isAdded)
        {
            string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "EmailTemplate", "LeaveEmail.html");
            string logoPath = Path.Combine(_hostingEnvironment.WebRootPath, "images", "ToDoLogo.png");
            logoPath = logoPath.Replace("\\", "/");
            string emailBody = File.ReadAllText(filePath);
            emailBody = emailBody.Replace("{ emailSubject }", Constants.LeaveRequestEmailSubject);
            emailBody = emailBody.Replace("{ employeeName }", user.Name);
            emailBody = emailBody.Replace("{ reason }", leave.Reason);
            emailBody = emailBody.Replace("{ startDate }", leave.StartDate.ToString());
            emailBody = emailBody.Replace("{ endDate }", leave.EndDate.ToString());
            emailBody = emailBody.Replace("{ returnDate }", leave.EndDate?.AddDays(1).ToString("yyyy-MM-dd"));
            emailBody = emailBody.Replace("{ statusMessage }", "");

            byte[] imageArray = File.ReadAllBytes(logoPath);
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);
            emailBody = emailBody.Replace("{ imageBase64 }", base64ImageRepresentation);

            HttpRequest? request = _httpContextAccessor.HttpContext?.Request;
            string leaveURL = request?.Scheme + "://" + request?.Host + "/Leave/TeamLeave";
            emailBody = emailBody.Replace("{ leaveURL }", leaveURL);

            string subject = Constants.LeaveRequestEmailSubject;
            if (admin.Email != null)
            {
                await _emailService.SendEmailAsync(admin.Email, subject, emailBody);
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<LeaveViewModel?> GetLeaveByIdAsync(int leaveId)
    {
        Leave? leave = await _leaveRepository.GetByIdAsync(leaveId);
        if (leave == null)
        {
            return new LeaveViewModel();
        }
        return new LeaveViewModel
        {
            LeaveId = leave.LeaveId,
            RequestedUserId = leave.RequestedUserId,
            ApprovalUserId = leave.ApprovalUserId,
            RequestedName = leave.RequestedUser?.Name ?? string.Empty,
            ApprovalName = leave.ApprovalUser?.Name ?? string.Empty,
            Reason = leave.Reason ?? string.Empty,
            StartDate = leave.StartDate?.ToString("yyyy-MM-dd"),
            EndDate = leave.EndDate?.ToString("yyyy-MM-dd"),
            ReturnDate = leave.EndDate?.AddDays(1).ToString("yyyy-MM-dd"),
            ApprovedOn = leave.ApprovedOn?.ToString("yyyy-MM-dd"),
            PhoneNumber = leave.PhoneNumber,
            AlternatePhoneNumber = leave.AlternatePhoneNumber,
            IsAvailableOnPhone = leave.IsAvailableOnPhone,
            Duration = leave.EndDate.HasValue && leave.StartDate.HasValue
                ? (leave.EndDate.Value.ToDateTime(TimeOnly.MinValue) - leave.StartDate.Value.ToDateTime(TimeOnly.MinValue)).Days + 1
                : null,
            Status = leave.Status,
            CommentByApproval = leave.CommentByApproval,
            CreatedAt = leave.CreatedAt?.ToString("yyyy-MM-dd")
        };
    }

    public async Task<bool> UpdateLeaveAsync(LeaveViewModel model)
    {
        Leave? leave = await _leaveRepository.GetByIdAsync(model.LeaveId ?? 0);
        if (leave == null)
        {
            return false;
        }
        leave.StartDate = model.StartDate != null ? DateOnly.Parse(model.StartDate) : null;
        leave.EndDate = model.EndDate != null ? DateOnly.Parse(model.EndDate) : null;
        leave.Reason = model.Reason?.Trim();
        leave.PhoneNumber = model.PhoneNumber;
        leave.AlternatePhoneNumber = model.AlternatePhoneNumber;
        leave.IsAvailableOnPhone = model.IsAvailableOnPhone;
        leave.CommentByApproval = model.CommentByApproval?.Trim();
        leave.ApprovedOn = model.Status == "Approved" ? DateOnly.FromDateTime(DateTime.Now) : null;

        if (model.Status != "Pending" && leave.Status != model.Status)
        {
            string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "EmailTemplate", "LeaveEmail.html");
            string emailBody = File.ReadAllText(filePath);
            string logoPath = Path.Combine(_hostingEnvironment.WebRootPath, "images", "ToDoLogo.png");
            logoPath = logoPath.Replace("\\", "/");
            string statusMessage = model.Status == "Approved" ? Constants.LeaveApprovedMessage : Constants.LeaveRejectedMessage;
            string emailSubject = model.Status == "Approved" ? Constants.LeaveApprovedEmailSubject : Constants.LeaveRejectedEmailSubject;
            emailBody = emailBody.Replace("{ emailSubject }", emailSubject);
            emailBody = emailBody.Replace("{ employeeName }", leave.RequestedUser?.Name);
            emailBody = emailBody.Replace("{ reason }", leave.Reason);
            emailBody = emailBody.Replace("{ startDate }", leave.StartDate.ToString());
            emailBody = emailBody.Replace("{ endDate }", leave.EndDate.ToString());
            emailBody = emailBody.Replace("{ returnDate }", leave.EndDate?.AddDays(1).ToString("yyyy-MM-dd"));
            emailBody = emailBody.Replace("{ statusMessage }", statusMessage);

            byte[] imageArray = File.ReadAllBytes(logoPath);
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);
            emailBody = emailBody.Replace("{ imageBase64 }", base64ImageRepresentation);

            HttpRequest? request = _httpContextAccessor.HttpContext?.Request;
            string leaveURL = request?.Scheme + "://" + request?.Host + "/Leave/SelfLeave";

            emailBody = emailBody.Replace("{ leaveURL }", leaveURL);

            string subject = leave.Status == "Approved" ? Constants.LeaveApprovedEmailSubject : Constants.LeaveRejectedEmailSubject;
            if (leave.RequestedUser?.Email != null)
            {
                await _emailService.SendEmailAsync(leave.RequestedUser.Email, subject, emailBody);
            }
        }
        leave.Status = model.Status;
        return await _leaveRepository.UpdateAsync(leave);
    }

    public async Task<bool> DeleteLeaveAsync(int leaveId)
    {
        Leave? leave = await _leaveRepository.GetByIdAsync(leaveId);
        if (leave == null)
        {
            return false;
        }
        leave.IsDeleted = true;
        leave.UpdatedAt = DateTime.UtcNow;
        return await _leaveRepository.UpdateAsync(leave);
    }
}