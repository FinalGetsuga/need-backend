
using Domain.DTO;
using Domain.Models;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class CategoryService : ICategoryService
{
    private readonly IRepository<Category> _repository;

    public CategoryService(IRepository<Category> repository)
    {
        _repository = repository;
    }

    public async Task<List<Category>> GetAllAsync()
    {
        return await _repository.GetAllAsync(selector: x => x);
    }

    public async Task<Category> GetByIdAsync(Guid id)
    {
        return await _repository.GetAsync(
            selector: x => x,
            predicate: x => x.Id == id);
    }

    public async Task<Category> GetByIdNotNullAsync(Guid id)
    {
        var entity = await this.GetByIdAsync(id);

        if (entity == null)
        {
            throw new InvalidOperationException("Not found!");
        }

        return entity;
    }

    public async Task<Category> CreateAsync(CategoryDto dto)
    {
        var category = new Category
        {
            Name = dto.Name,
            Description = dto.Description,
            IsActive = dto.IsActive,
        };
        
        return await _repository.InsertAsync(category);
    }

    public async Task<Category> UpdateAsync(Guid id, CategoryDto dto)
    {
        var category = await this.GetByIdNotNullAsync(id);
        
        category.Name = dto.Name;
        category.Description = dto.Description;
        category.IsActive = dto.IsActive;
        
        return await _repository.UpdateAsync(category);
    }

    public async Task<Category> DeleteAsync(Guid id)
    {
        var category = await this.GetByIdNotNullAsync(id);
        return await _repository.DeleteAsync(category);
    }
}