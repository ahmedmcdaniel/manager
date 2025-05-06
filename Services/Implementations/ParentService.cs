using SchoolManager.Models;
using Microsoft.EntityFrameworkCore;

public class ParentService : IParentService
{
    private readonly SchoolDbContext _context;

    public ParentService(SchoolDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetParentByStudentIdAsync(Guid studentId)
    {
        var student = await _context.Students
            .Include(s => s.Parent)
            .FirstOrDefaultAsync(s => s.Id == studentId);

        return student?.Parent;
    }

    public async Task<List<Student>> GetStudentsByParentIdAsync(Guid parentId)
    {
        return await _context.Students
            .Where(s => s.ParentId == parentId)
            .ToListAsync();
    }

    public async Task AssignParentToStudentAsync(Guid parentId, Guid studentId)
    {
        var student = await _context.Students.FindAsync(studentId);
        if (student != null)
        {
            student.ParentId = parentId;
            await _context.SaveChangesAsync();
        }
    }

    public async Task RemoveParentFromStudentAsync(Guid studentId)
    {
        var student = await _context.Students.FindAsync(studentId);
        if (student != null)
        {
            student.ParentId = null;
            await _context.SaveChangesAsync();
        }
    }
}
