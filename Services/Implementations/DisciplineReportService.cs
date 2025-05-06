using SchoolManager.Models;
using Microsoft.EntityFrameworkCore;

public class DisciplineReportService : IDisciplineReportService
{
    private readonly SchoolDbContext _context;

    public DisciplineReportService(SchoolDbContext context)
    {
        _context = context;
    }

    public async Task<List<DisciplineReport>> GetAllAsync() =>
        await _context.DisciplineReports.ToListAsync();

    public async Task<DisciplineReport?> GetByIdAsync(Guid id) =>
        await _context.DisciplineReports.FindAsync(id);

    public async Task CreateAsync(DisciplineReport report)
    {
        _context.DisciplineReports.Add(report);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(DisciplineReport report)
    {
        _context.DisciplineReports.Update(report);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var report = await _context.DisciplineReports.FindAsync(id);
        if (report != null)
        {
            _context.DisciplineReports.Remove(report);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<DisciplineReport>> GetByStudentAsync(Guid studentId)
    {
        return await _context.DisciplineReports
            .Where(r => r.StudentId == studentId)
            .ToListAsync();
    }
}
