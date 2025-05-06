using Microsoft.AspNetCore.Mvc;
using SchoolManager.Services.Interfaces;
using SchoolManager.Dtos;
using System;
using System.Threading.Tasks;
using SchoolManager.Models;
using SchoolManager.Services.Implementations;
using System.Linq;

public class StudentReportController : Controller
{
    private readonly IStudentReportService _reportService;

    public StudentReportController(IStudentReportService reportService)
    {
        _reportService = reportService;
    }

    public async Task<IActionResult> Index()
    {
        // Aquí 'quemamos' el studentId, lo que significa que no puedes cambiarlo dinámicamente
        var studentId = Guid.Parse("43403bf4-2e34-43fe-bc14-91770aafc9e9");

        // Obtener el reporte real desde el servicio (sin pasar trimestre)
        var report = await _reportService.GetReportByStudentIdAsync(studentId);

        if (report == null)
        {
            return NotFound("No se encontró el reporte para este estudiante.");
        }

        // Forzar que el trimestre seleccionado sea 1T si existe, si no el primero disponible
        var availableTrimesters = report.AvailableTrimesters.Select(t => t.Trimester).ToList();
        string selectedTrimester = availableTrimesters.Contains("1T") ? "1T" : availableTrimesters.FirstOrDefault();
        if (selectedTrimester != null && report.Trimester != selectedTrimester)
        {
            // Volver a pedir el reporte solo para el trimestre seleccionado
            report = await _reportService.GetReportByStudentIdAndTrimesterAsync(studentId, selectedTrimester);
            report.AvailableTrimesters = availableTrimesters.Select(t => new AvailableTrimesters { Trimester = t }).ToList();
        }
        report.StudentId = studentId;
        ViewBag.AvailableTrimesters = report.AvailableTrimesters;
        return View(report);
    }

    
    public async Task<IActionResult> GetTrimesterData(Guid studentId, string trimester)
    {
        var report = await _reportService.GetReportByStudentIdAndTrimesterAsync(studentId, trimester);

        if (report == null)
        {
            return Json(new { error = "No se encontraron datos para el trimestre seleccionado." });
        }

        return Json(new
        {
            grades = report.Grades.Select(g => new
            {
                type = g.Type,
                teacher = g.Teacher,
                value = g.Value
            }),
            trimester = report.Trimester,
            attendanceByTrimester = report.AttendanceByTrimester.Select(a => new {
                month = a.Month,
                present = a.Present,
                absent = a.Absent,
                late = a.Late
            }),
            attendanceByMonth = report.AttendanceByMonth.Select(a => new {
                month = a.Month,
                present = a.Present,
                absent = a.Absent,
                late = a.Late
            })
        });
    }




}


