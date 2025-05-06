using System.Threading.Tasks;
using SchoolManager.ViewModels;

namespace SchoolManager.Services.Interfaces
{
    public interface IDirectorService
    {
        Task<DirectorViewModel> GetDashboardViewModelAsync(string trimestre = null);
    }
} 