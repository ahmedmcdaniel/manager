using SchoolManager.Models;

public interface IParentService
{
    Task<User?> GetParentByStudentIdAsync(Guid studentId);
    Task<List<Student>> GetStudentsByParentIdAsync(Guid parentId);
    Task AssignParentToStudentAsync(Guid parentId, Guid studentId);
    Task RemoveParentFromStudentAsync(Guid studentId);
}
