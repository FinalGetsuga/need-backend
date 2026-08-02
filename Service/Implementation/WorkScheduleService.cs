using Domain.DTO;
using Domain.Identity;
using Domain.Models;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class WorkScheduleService : IWorkScheduleService
{
    private readonly IRepository<WorkSchedule> _repository;
    private readonly IRepository<WorkingDay> _workingDayRepository;
    private readonly IRepository<Business> _businessRepository;
    private readonly UserManager<AppUser> _userManager;
    private readonly IBackgroundJobClient _backgroundJobClient;

    public WorkScheduleService(
        IRepository<WorkSchedule> repository,
        IRepository<WorkingDay> workingDayRepository,
        IRepository<Business> businessRepository,
        UserManager<AppUser> userManager,
        IBackgroundJobClient backgroundJobClient)
    {
        _repository = repository;
        _workingDayRepository = workingDayRepository;
        _businessRepository = businessRepository;
        _userManager = userManager;
        _backgroundJobClient = backgroundJobClient;
    }

    public async Task<List<WorkSchedule>> GetAllAsync()
    {
        return await _repository.GetAllAsync(selector: x => x);
    }

    public async Task<WorkSchedule> GetByBusinessIdAsync(Guid businessId)
    {
        var schedule = await _repository.GetAsync(
            selector: x => x,
            predicate: x => x.BusinessId == businessId,
            include: q => q.Include(x => x.WorkingDays));

        if (schedule == null)
            throw new InvalidOperationException("Not found!");
        
        return schedule;
    }

    public async Task<WorkSchedule> CreateAsync(string userId, WorkScheduleDto dto)
    {
        await AuthorizeAsync(userId, dto.BusinessId);
        Validate(dto);
        
        var alreadyExists = await _repository.ExistsAsync(x => x.BusinessId == dto.BusinessId);
        if (alreadyExists)
            throw new InvalidOperationException("This business already has a work schedule");

        var schedule = new WorkSchedule
        {
            BusinessId = dto.BusinessId,
            TermDurationMinutes = dto.TermDurationMinutes,
            LastGeneratedThroughDate = DateOnly.FromDateTime(DateTime.Now).AddDays(-1),
            WorkingDays = dto.WorkingDays.Select(d => new WorkingDay
            {
                DayOfWeek = d.DayOfWeek,
                StartTime = d.StartTime,
                EndTime = d.EndTime
            }).ToList()
        };
        
        var created = await _repository.InsertAsync(schedule);
        _backgroundJobClient.Enqueue<ITermGenerationService>(x => x.GenerateForBusinessAsync(schedule.BusinessId));
        return created;
    }

    public async Task<WorkSchedule> UpdateAsync(string userId, Guid businessId, WorkScheduleDto dto)
    {
        await AuthorizeAsync(userId, dto.BusinessId);
        Validate(dto);

        var schedule = await this.GetByBusinessIdAsync(businessId);
        
        schedule.TermDurationMinutes = dto.TermDurationMinutes;

        foreach (var existingDay in schedule.WorkingDays.ToList())
        {
            await _workingDayRepository.DeleteAsync(existingDay);
        }

        foreach (var day in dto.WorkingDays )
        {
            await _workingDayRepository.InsertAsync(new WorkingDay
            {
                WorkScheduleId = schedule.Id,
                DayOfWeek = day.DayOfWeek,
                StartTime = day.StartTime,
                EndTime = day.EndTime
            });
        }
        
        var updated = await _repository.UpdateAsync(schedule);
        _backgroundJobClient.Enqueue<ITermGenerationService>(x => x.GenerateForBusinessAsync(businessId));
        return updated;
    }

    private async Task AuthorizeAsync(string userId, Guid businessId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        bool isAdmin = user != null && await _userManager.IsInRoleAsync(user, "Admin");

        if (!isAdmin)
        {
            var isOwner = await _businessRepository.ExistsAsync(b => b.Id == businessId && b.OwnerId == userId);
            if (!isOwner)
                throw new UnauthorizedAccessException("Only the business owner can manage its work schedule.");
        }
    }

    private static void Validate(WorkScheduleDto dto)
    {
        if (dto.TermDurationMinutes <= 0)
            throw new InvalidOperationException("Term duration must be greater than zero.");

        if (dto.WorkingDays.Count == 0)
            throw new InvalidOperationException("At least one working day is required.");

        if (dto.WorkingDays.Select(d => d.DayOfWeek).Distinct().Count() != dto.WorkingDays.Count)
            throw new InvalidOperationException("Duplicate day of week in working days.");

        foreach (var day in dto.WorkingDays)
        {
            if (day.StartTime >= day.EndTime)
                throw new InvalidOperationException(
                    $"Invalid time range for {day.DayOfWeek}: start must be before end.");
        }
    }
}