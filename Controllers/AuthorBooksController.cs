using Microsoft.AspNetCore.Mvc;
using CursorConvertedEFCoreApp.Services;

namespace CursorConvertedEFCoreApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorBooksController : ControllerBase
{
    private readonly IBookStoreService _bookStoreService;
    private readonly ILogger<AuthorBooksController> _logger;

    public AuthorBooksController(IBookStoreService bookStoreService, ILogger<AuthorBooksController> logger)
    {
        _bookStoreService = bookStoreService;
        _logger = logger;
    }

    // POST: api/authorbooks
    [HttpPost]
    public async Task<IActionResult> CreateAuthorBook([FromBody] CreateAuthorBookRequest request)
    {
        try
        {
            var (success, errorMessage) = await _bookStoreService.AddAuthorBookAsync(
                request.AuthorId, 
                request.Isbn);

            if (success)
            {
                return CreatedAtAction(nameof(CreateAuthorBook), new { authorId = request.AuthorId, isbn = request.Isbn }, null);
            }
            else
            {
                return BadRequest(errorMessage);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating author-book relationship");
            return StatusCode(500, "Internal server error");
        }
    }

    // PUT: api/authorbooks/{authorId}/{isbn}
    [HttpPut("{authorId}/{isbn}")]
    public async Task<IActionResult> UpdateAuthorBook(int authorId, string isbn)
    {
        try
        {
            var (success, errorMessage) = await _bookStoreService.ModifyAuthorBookAsync(authorId, isbn);

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
            _logger.LogError(ex, "Error updating author-book relationship: AuthorId={AuthorId}, ISBN={Isbn}", authorId, isbn);
            return StatusCode(500, "Internal server error");
        }
    }
}

public class CreateAuthorBookRequest
{
    public int AuthorId { get; set; }
    public string Isbn { get; set; } = string.Empty;
} 