using Microsoft.AspNetCore.Mvc;
using SchoolManager.Models;

public class DisciplineReportController : Controller
{
    private readonly IDisciplineReportService _disciplineReportService;

    public DisciplineReportController(IDisciplineReportService disciplineReportService)
    {
        _disciplineReportService = disciplineReportService;
    }

    public async Task<IActionResult> Index()
    {
        var reports = await _disciplineReportService.GetAllAsync();
        return View(reports);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var report = await _disciplineReportService.GetByIdAsync(id);
        if (report == null) return NotFound();
        return View(report);
    }

    public IActionResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create(DisciplineReport report)
    {
        if (ModelState.IsValid)
        {
            await _disciplineReportService.CreateAsync(report);
            return RedirectToAction(nameof(Index));
        }
        return View(report);
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var report = await _disciplineReportService.GetByIdAsync(id);
        if (report == null) return NotFound();
        return View(report);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(DisciplineReport report)
    {
        if (ModelState.IsValid)
        {
            await _disciplineReportService.UpdateAsync(report);
            return RedirectToAction(nameof(Index));
        }
        return View(report);
    }

    public async Task<IActionResult> Delete(Guid id)
    {
        var report = await _disciplineReportService.GetByIdAsync(id);
        if (report == null) return NotFound();
        return View(report);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        await _disciplineReportService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
