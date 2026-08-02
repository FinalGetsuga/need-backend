using Domain.DTO;
using Domain.Identity;
using Domain.Models;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class EmployeeService : IEmployeeService
{
    private readonly IRepository<Employee> _repository;
    private readonly IRepository<Business> _businessRepository;
    private readonly UserManager<AppUser> _userManager;
    private readonly IBackgroundJobClient _backgroundJobClient;

    public EmployeeService(
        IRepository<Employee> repository,
        IRepository<Business> businessRepository,
        UserManager<AppUser> userManager,
        IBackgroundJobClient backgroundJobClient)
    {
        _repository = repository;
        _businessRepository = businessRepository;
        _userManager = userManager;
        _backgroundJobClient = backgroundJobClient;
    }

    public async Task<List<Employee>> GetAllAsync()
    {
        return await _repository.GetAllAsync(selector: x => x);
    }

    public async Task<List<Employee>> GetAllByBusinessAsync(string userId, Guid businessId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        bool isAdmin = user != null && await _userManager.IsInRoleAsync(user, "Admin");

        if (!isAdmin)
        {
            var isOwner = await _businessRepository.ExistsAsync(b => b.Id == businessId && b.OwnerId == userId);
            if (!isOwner) throw new UnauthorizedAccessException("Only the business owner can view its employees.");
        }
        
        return await _repository.GetAllAsync(
            selector: x => x,
            predicate: x => x.BusinessId == businessId);
    }

    public async Task<Employee> GetByIdAsync(Guid id)
    {
        return await _repository.GetAsync(
            selector: x => x,
            predicate: x => x.Id == id);
    }

    public async Task<Employee> GetByIdNotNullAsync(Guid id)
    {
        var entity = await this.GetByIdAsync(id);

        if (entity == null)
        {
            throw new InvalidOperationException("Not found!");
        }

        return entity;
    }

    public async Task<Employee> CreateAsync(string userId, EmployeeDto dto)
    {
        var user = await _userManager.FindByIdAsync(userId);
        bool isAdmin = user != null && await _userManager.IsInRoleAsync(user, "Admin");

        if (!isAdmin)
        {
            var isOwner = await _businessRepository.ExistsAsync(b => b.Id == dto.BusinessId && b.OwnerId == userId);
            if (!isOwner) throw new UnauthorizedAccessException("Only the business owner can add employees.");
        }

        var targetOwnsAnotherBusiness = await _businessRepository.ExistsAsync(
            b => b.Id != dto.BusinessId && b.OwnerId == dto.UserId);
        if (targetOwnsAnotherBusiness)
            throw new InvalidOperationException("This user already owns a different business and cannot be added as an employee.");

        var targetIsEmployee = await _repository.ExistsAsync(e => e.UserId == dto.UserId && e.IsActive);
        if (targetIsEmployee)
            throw new InvalidOperationException("This user is already employed elsewhere.");

        var entity = new Employee()
        {
            BusinessId = dto.BusinessId,
            UserId = dto.UserId,
            IsActive = dto.IsActive
        };

        var created = await _repository.InsertAsync(entity);

        if (created.IsActive)
        {
            _backgroundJobClient.Enqueue<ITermGenerationService>(x => x.GenerateForBusinessAsync(created.BusinessId));
        }

        return created;
    }

    public async Task<Employee> UpdateAsync(string userId, Guid id, bool isActive)
    {
        var entity = await this.GetByIdNotNullAsync(id);

        var isOwner = await _businessRepository.ExistsAsync(b => b.Id == entity.BusinessId && b.OwnerId == userId);
        if (!isOwner) throw new UnauthorizedAccessException("Only the business owner can update this employee.");

        entity.IsActive = isActive;
        
        return await _repository.UpdateAsync(entity);
    }

    public async Task<Employee> DeleteAsync(string userId, Guid id)
    {
        var entity = await this.GetByIdNotNullAsync(id);
        
        var user = await _userManager.FindByIdAsync(userId);
        bool isAdmin = user != null && await _userManager.IsInRoleAsync(user, "Admin");

        if (!isAdmin)
        {
            var isOwner = await _businessRepository.ExistsAsync(b => b.Id == entity.BusinessId && b.OwnerId == userId);
            if (!isOwner) throw new UnauthorizedAccessException("Only the business owner can delete this employee.");
        }

        return await _repository.DeleteAsync(entity);
    }
}