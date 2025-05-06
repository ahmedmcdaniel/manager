using Microsoft.AspNetCore.Mvc;
using SchoolManager.Models;

public class SubjectController : Controller
{
    private readonly ISubjectService _subjectService;

    public SubjectController(ISubjectService subjectService)
    {
        _subjectService = subjectService;
    }

    public async Task<IActionResult> Index()
    {
        var subjects = await _subjectService.GetAllAsync();
        return View(subjects);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var subject = await _subjectService.GetByIdAsync(id);
        if (subject == null) return NotFound();
        return View(subject);
    }

    //public IActionResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create(Subject subject)
    {
        if (!ModelState.IsValid || string.IsNullOrWhiteSpace(subject.Name))
            return BadRequest("Nombre inválido");

        var created = await _subjectService.CreateAsync(subject);
        return Json(new { id = created.Id, name = created.Name });
    }

    //public async Task<IActionResult> Edit(Guid id)
    //{
    //    var subject = await _subjectService.GetByIdAsync(id);
    //    if (subject == null) return NotFound();
    //    return View(subject);
    //}

    [HttpPost]
    public async Task<IActionResult> Edit([FromBody] Subject subject)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updatedSubject = await _subjectService.UpdateAsync(subject);
        return Json(new
        {
            id = updatedSubject.Id,
            name = updatedSubject.Name
        });
    }


    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
    {
        var subject = await _subjectService.GetByIdAsync(id);
        if (subject == null)
            return NotFound();

        await _subjectService.DeleteAsync(id);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> ListJson()
    {
        var subjects = await _subjectService.GetAllAsync();
        var result = subjects.Select(s => new { id = s.Id, name = s.Name });
        return Json(result);
    }

}
