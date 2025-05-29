using LarmoireWeb.Models;

public interface IRenovationService
{
    Task<IEnumerable<RenovationRequest>> GetAllAsync();
    Task<IEnumerable<RenovationRequest>> GetByUserAsync(string userId);
    Task<RenovationRequest> GetByIdAsync(int id);             
    Task SubmitAsync(RenovationRequest request);
    Task UpdateStatusAsync(int id, RequestStatus status);
    Task DeleteAsync(int id);                                  
}
