using Microsoft.AspNetCore.Mvc;
using SchoolManager.Models;

public class GroupController : Controller
{
    private readonly IGroupService _groupService;

    public GroupController(IGroupService groupService)
    {
        _groupService = groupService;
    }

    // Vista tradicional
    public async Task<IActionResult> Index()
    {
        var groups = await _groupService.GetAllAsync();
        return View(groups);
    }

    // 🔹 API para obtener lista JSON de grupos
    [HttpGet]
    public async Task<IActionResult> ListJson()
    {
        var groups = await _groupService.GetAllAsync();
        return Json(groups);
    }

    // 🔹 Crear grupo desde modal
    [HttpPost]
    public async Task<IActionResult> Create([FromForm] string name, [FromForm] string? grade)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return BadRequest("El nombre del grupo es obligatorio.");
        }

        var group = new Group
        {
            Id = Guid.NewGuid(),
            Name = name,
            Grade = grade,
            CreatedAt = DateTime.UtcNow
        };

        await _groupService.CreateAsync(group);
        return Json(group);
    }

    // 🔹 Editar grupo desde modal (por AJAX)
    [HttpPost]
    public async Task<IActionResult> Edit([FromBody] Group group)
    {
        if (group == null || string.IsNullOrWhiteSpace(group.Name))
        {
            return BadRequest("Datos inválidos.");
        }

        var existing = await _groupService.GetByIdAsync(group.Id);
        if (existing == null) return NotFound();

        existing.Name = group.Name;
        existing.Grade = group.Grade;
        await _groupService.UpdateAsync(existing);

        return Json(existing);
    }

    // 🔹 Eliminar grupo desde modal (por AJAX)
    [HttpPost]
    public async Task<IActionResult> Delete([FromForm] Guid id)
    {
        try
        {
            var existing = await _groupService.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new { success = false, message = "Grupo no encontrado." });

            await _groupService.DeleteAsync(id);
            return Ok(new { success = true, message = "Grupo eliminado correctamente." });
        }
        catch (Exception ex)
        {
            // Puedes registrar el error aquí si tienes un sistema de logging
            return StatusCode(500, new { success = false, message = "Ocurrió un error al eliminar el grupo.", error = ex.Message });
        }
    }


    // ⚠️ Opcional: puedes eliminar estos si no usas vistas tradicionales:
    public async Task<IActionResult> Details(Guid id)
    {
        var group = await _groupService.GetByIdAsync(id);
        if (group == null) return NotFound();
        return View(group);
    }

    public IActionResult Create() => View();

    public async Task<IActionResult> Edit(Guid id)
    {
        var group = await _groupService.GetByIdAsync(id);
        if (group == null) return NotFound();
        return View(group);
    }



}
