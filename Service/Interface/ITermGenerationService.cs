namespace Service.Interface;

public interface ITermGenerationService
{
    Task GenerateForBusinessAsync(Guid businessId);
    Task GenerateForAllBusinessesAsync();
}