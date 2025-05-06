using SchoolManager.Models;

public interface IDisciplineReportService
{
    Task<List<DisciplineReport>> GetAllAsync();
    Task<DisciplineReport?> GetByIdAsync(Guid id);
    Task CreateAsync(DisciplineReport report);
    Task UpdateAsync(DisciplineReport report);
    Task DeleteAsync(Guid id);
    Task<List<DisciplineReport>> GetByStudentAsync(Guid studentId);
}
