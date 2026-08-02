using Domain.Enums;
using Domain.Models;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class TermGenerationService : ITermGenerationService
{
    private readonly IWorkScheduleService _workScheduleService;
    private readonly IRepository<Employee> _employeeRepository;
    private readonly IRepository<Term> _termRepository;

    public TermGenerationService(IWorkScheduleService workScheduleService, IRepository<Employee> employeeRepository, IRepository<Term> termRepository)
    {
        _workScheduleService = workScheduleService;
        _employeeRepository = employeeRepository;
        _termRepository = termRepository;
    }

    private const int RollingWindowDays = 7;

    public async Task GenerateForBusinessAsync(Guid businessId)
    {
        var schedule = await _workScheduleService.GetByBusinessIdAsync(businessId);
        
        if (schedule == null)
            return;

        var employees = await _employeeRepository.GetAllAsync(
            selector: x => x,
            predicate: e => e.BusinessId == businessId && e.IsActive);
        
        if (employees.Count == 0)
            return;

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var targetDate = today.AddDays(RollingWindowDays - 1);

        var cursor = schedule.LastGeneratedThroughDate.AddDays(1);
        if (cursor < today)
            cursor = today;

        for (var date = cursor; date <= targetDate; date = date.AddDays(1))
        {
            var workingDay = schedule.WorkingDays.FirstOrDefault(d => d.DayOfWeek == date.DayOfWeek);
            if (workingDay != null)
            {
                foreach (var employee in employees)
                {
                    await GenerateTermsForEmployeeDayAsync(
                        businessId, employee.Id, date, workingDay, schedule.TermDurationMinutes);
                }
            }
            
            schedule.LastGeneratedThroughDate = date;
        }
    }

    public async Task GenerateForAllBusinessesAsync()
    {
        var schedules = await _workScheduleService.GetAllAsync();

        foreach (var schedule in schedules)
        {
            await GenerateForBusinessAsync(schedule.BusinessId);
        }
    }

    private async Task GenerateTermsForEmployeeDayAsync(Guid businessId, Guid employeeId, DateOnly date,
        WorkingDay workingDay, int termDurationMinutes)
    {
        var alreadyGenerated = await _termRepository.ExistsAsync(t => t.EmployeeId == employeeId && t.Date == date);
        
        if (alreadyGenerated) 
            return;

        var current = workingDay.StartTime;

        while (true)
        {
            var end = current.AddMinutes(termDurationMinutes);
            if (end > workingDay.EndTime)
                break;

            await _termRepository.InsertAsync(new Term
            {
                BusinessId = businessId,
                EmployeeId = employeeId,
                Date = date,
                StartTime = current,
                EndTime = end,
                Status = TermStatus.Available
            });
            
            current = end;
        }
    }
}