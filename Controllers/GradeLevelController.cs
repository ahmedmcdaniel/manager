using Microsoft.AspNetCore.Mvc;
using SchoolManager.Models;
using SchoolManager.Services.Interfaces;

namespace SchoolManager.Controllers;

[Route("GradeLevel")]
public class GradeLevelController : Controller
{
    private readonly IGradeLevelService _gradeLevelService;

    public GradeLevelController(IGradeLevelService gradeLevelService)
    {
        _gradeLevelService = gradeLevelService;
    }

    [HttpGet("ListJson")]
    public async Task<IActionResult> ListJson()
    {
        var result = await _gradeLevelService.GetAllAsync();
        return Json(result);
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] GradeLevel data)
    {
        try
        {
            // Asegurar que tenga ID y fecha si no vienen del cliente
            data.Id = Guid.NewGuid();
            data.CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            var created = await _gradeLevelService.CreateAsync(data);

            return Json(new
            {
                success = true,
                id = created.Id,
                name = created.Name,
                description = created.Description
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("Edit")]
    public async Task<IActionResult> Edit([FromBody] GradeLevel data)
    {
        try
        {
   

            var updated = await _gradeLevelService.UpdateAsync(data);
            return Json(new { success = true, id = updated.Id, name = updated.Name, description = updated.Description });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("Delete")]
    public async Task<IActionResult> Delete([FromBody] GradeLevel data)
    {
        try
        {
        
            await _gradeLevelService.DeleteAsync(data.Id);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}
