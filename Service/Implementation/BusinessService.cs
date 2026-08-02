using Domain.DTO;
using Domain.Identity;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class BusinessService : IBusinessService
{
    private readonly IRepository<Business> _repository;
    private readonly UserManager<AppUser> _userManager;

    public BusinessService(IRepository<Business> repository, UserManager<AppUser> userManager)
    {
        _repository = repository;
        _userManager = userManager;
    }

    public async Task<List<Business>> GetAllAsync()
    {
        return await _repository.GetAllAsync(selector: x => x);
    }

    public async Task<List<Business>> GetAllByCategoryAsync(Guid categoryId)
    {
        return await _repository.GetAllAsync(
            selector: x => x,
            predicate: x => x.CategoryId == categoryId);
    }

    public async Task<Business> GetByIdAsync(Guid id)
    {
        return await _repository.GetAsync(
            selector: x => x,
            predicate: x => x.Id == id);
    }

    public async Task<Business> GetByIdNotNullAsync(Guid id)
    {
        var entity = await this.GetByIdAsync(id);

        if (entity == null)
        {
            throw new InvalidOperationException("Not found!");
        }

        return entity;
    }

    public async Task<Business> CreateAsync(string userId, BusinessDto dto)
    {
        var user = await _userManager.FindByIdAsync(userId);

        var alreadyOwnsBusiness = await _repository.ExistsAsync(b => b.OwnerId == user.Id);
        if (alreadyOwnsBusiness) throw new InvalidOperationException("This user already owns a business!");
        
        var business = new Business()
        {
            Name = dto.Name,
            Description = dto.Description,
            Address = dto.Address,
            ImageUrl = dto.ImageUrl,
            WebsiteUrl = dto.WebsiteUrl,
            OwnerId = userId,
            CategoryId = dto.CategoryId,
        };

        if (user != null && !await _userManager.IsInRoleAsync(user, "Owner"))
        {
            await _userManager.AddToRoleAsync(user, "Owner");
        }

        return await _repository.InsertAsync(business);
    }

    public async Task<Business> UpdateAsync(string userId, Guid id, BusinessDto dto)
    {
        var entity = await this.GetByIdNotNullAsync(id);
        
        var user = await _userManager.FindByIdAsync(userId);
        bool isAdmin = user != null && await _userManager.IsInRoleAsync(user, "Admin");

        if (!isAdmin)
        {
            if (entity.OwnerId != user.Id) throw new InvalidOperationException("Not authorized!");
        }

        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.Address = dto.Address;
        entity.ImageUrl = dto.ImageUrl;
        entity.WebsiteUrl = dto.WebsiteUrl;
        entity.CategoryId = dto.CategoryId;
        
        return await _repository.UpdateAsync(entity);
    }

    public async Task<Business> DeleteAsync(string userId, Guid id)
    {
        var entity = await this.GetByIdNotNullAsync(id);
        
        var user = await _userManager.FindByIdAsync(userId);
        bool isAdmin = user != null && await _userManager.IsInRoleAsync(user, "Admin");

        if (!isAdmin)
        {
            if (entity.OwnerId != user.Id) throw new InvalidOperationException("Not authorized!");
        }

        if (entity.Owner != null) await _userManager.RemoveFromRoleAsync(entity.Owner, "Owner");
        return await _repository.DeleteAsync(entity);
    }
}