using Microsoft.AspNetCore.Mvc;
using SchoolManager.ViewModels;
using System;
using System.Threading.Tasks;
using SchoolManager.Services.Interfaces;

namespace SchoolManager.Controllers
{
    public class DirectorController : Controller
    {
        private readonly IDirectorService _directorService;
        private readonly ITrimesterService _trimesterService;

        public DirectorController(IDirectorService directorService, ITrimesterService trimesterService)
        {
            _directorService = directorService;
            _trimesterService = trimesterService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _directorService.GetDashboardViewModelAsync("");
            return View("Director", model);
        }

        [HttpPost]
        public async Task<IActionResult> FiltrarPorTrimestre([FromBody] string trimestre)
        {
            try
            {
                var model = await _directorService.GetDashboardViewModelAsync(trimestre);
                return Json(model);
            }
            catch (Exception)
            {
                return BadRequest(new { error = "Ocurrió un error al filtrar los datos." });
            }
        }
    }
} 