using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotesAPI.Models;
using NotesAPI.Services;
using System.Security.Claims;

namespace NotesAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotesController : ControllerBase
{
    private readonly INoteService _noteService;

    public NotesController(INoteService noteService)
    {
        _noteService = noteService;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim!);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? page, [FromQuery] int? pageSize)
    {
        var userId = GetCurrentUserId();
        
        // If pagination params provided, return paginated results
        if (page.HasValue && pageSize.HasValue)
        {
            var paginatedResult = await _noteService.GetPaginatedByUserIdAsync(userId, page.Value, pageSize.Value);
            return Ok(paginatedResult);
        }
        
        // Otherwise return all notes (backward compatibility)
        var notes = await _noteService.GetAllByUserIdAsync(userId);
        return Ok(notes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = GetCurrentUserId();
        var note = await _noteService.GetByIdAsync(id, userId);
        
        if (note == null)
        {
            return NotFound(new { message = "Note not found" });
        }

        return Ok(note);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateNoteRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var note = await _noteService.CreateAsync(request, userId);
            return CreatedAtAction(nameof(GetById), new { id = note.Id }, note);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateNoteRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var note = await _noteService.UpdateAsync(id, request, userId);
            return Ok(note);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetCurrentUserId();
        var success = await _noteService.DeleteAsync(id, userId);
        
        if (!success)
        {
            return NotFound(new { message = "Note not found" });
        }

        return NoContent();
    }
}
