using Domain.DTO;
using Domain.Models;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class BusinessService : IBusinessService
{
    private readonly IRepository<Business> _repository;

    public BusinessService(IRepository<Business> repository)
    {
        _repository = repository;
    }

    public async Task<List<Business>> GetAllAsync()
    {
        return await _repository.GetAllAsync(selector: x => x);
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

    public async Task<Business> CreateAsync(BusinessDto dto)
    {
        var business = new Business()
        {
            Name = dto.Name,
            Description = dto.Description,
            Address = dto.Address,
            ImageUrl = dto.ImageUrl,
            WebsiteUrl = dto.WebsiteUrl,
            OwnerId = dto.OwnerId,
            CategoryId = dto.CategoryId,
            WorkScheduleId = dto.WorkScheduleId
        };
        
        business.Owner.IsOwner = true;
        
        return await _repository.InsertAsync(business);
    }

    public async Task<Business> UpdateAsync(Guid id, BusinessDto dto)
    {
        var entity = await this.GetByIdNotNullAsync(id);
        
        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.Address = dto.Address;
        entity.ImageUrl = dto.ImageUrl;
        entity.WebsiteUrl = dto.WebsiteUrl;
        entity.OwnerId = dto.OwnerId;
        entity.CategoryId = dto.CategoryId;
        entity.WorkScheduleId = dto.WorkScheduleId;
        
        return await _repository.UpdateAsync(entity);
    }

    public async Task<Business> DeleteAsync(Guid id)
    {
        var entity = await this.GetByIdNotNullAsync(id);
        return await _repository.DeleteAsync(entity);
    }
}