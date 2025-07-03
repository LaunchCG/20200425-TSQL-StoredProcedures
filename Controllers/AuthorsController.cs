using Microsoft.AspNetCore.Mvc;
using CursorConvertedEFCoreApp.Services;
using CursorConvertedEFCoreApp.Models;

namespace CursorConvertedEFCoreApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorsController : ControllerBase
{
    private readonly IBookStoreService _bookStoreService;
    private readonly ILogger<AuthorsController> _logger;

    public AuthorsController(IBookStoreService bookStoreService, ILogger<AuthorsController> logger)
    {
        _bookStoreService = bookStoreService;
        _logger = logger;
    }

    // GET: api/authors
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Author>>> GetAuthors(
        [FromQuery] string firstname = "%", 
        [FromQuery] string surname = "%")
    {
        try
        {
            var authors = await _bookStoreService.GetAuthorsAsync(firstname, surname);
            return Ok(authors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving authors");
            return StatusCode(500, "Internal server error");
        }
    }

    // POST: api/authors
    [HttpPost]
    public async Task<ActionResult<Author>> CreateAuthor([FromBody] CreateAuthorRequest request)
    {
        try
        {
            var (resultId, errorMessage) = await _bookStoreService.AddAuthorAsync(
                request.Firstname, 
                request.Surname, 
                request.Surname2);

            if (resultId.HasValue)
            {
                return CreatedAtAction(nameof(GetAuthors), new { id = resultId }, new { Id = resultId });
            }
            else
            {
                return BadRequest(errorMessage);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating author");
            return StatusCode(500, "Internal server error");
        }
    }

    // PUT: api/authors/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAuthor(int id, [FromBody] UpdateAuthorRequest request)
    {
        try
        {
            var (success, errorMessage) = await _bookStoreService.ModifyAuthorAsync(
                id, 
                request.Firstname, 
                request.Surname, 
                request.Surname2);

            if (success)
            {
                return NoContent();
            }
            else
            {
                return BadRequest(errorMessage);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating author with ID: {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    // DELETE: api/authors/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAuthor(int id)
    {
        try
        {
            var (success, errorMessage) = await _bookStoreService.DeleteAuthorAsync(id);

            if (success)
            {
                return NoContent();
            }
            else
            {
                return BadRequest(errorMessage);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting author with ID: {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }
}

public class CreateAuthorRequest
{
    public string Firstname { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string? Surname2 { get; set; }
}

public class UpdateAuthorRequest
{
    public string Firstname { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string? Surname2 { get; set; }
} 