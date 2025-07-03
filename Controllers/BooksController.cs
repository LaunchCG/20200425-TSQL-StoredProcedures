using Microsoft.AspNetCore.Mvc;
using CursorConvertedEFCoreApp.Services;
using CursorConvertedEFCoreApp.Models;

namespace CursorConvertedEFCoreApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookStoreService _bookStoreService;
    private readonly ILogger<BooksController> _logger;

    public BooksController(IBookStoreService bookStoreService, ILogger<BooksController> logger)
    {
        _bookStoreService = bookStoreService;
        _logger = logger;
    }

    // GET: api/books
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookWithAuthorInfo>>> GetBooks(
        [FromQuery] string isbn = "%", 
        [FromQuery] string title = "%")
    {
        try
        {
            var books = await _bookStoreService.GetBooksAsync(isbn, title);
            return Ok(books);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving books");
            return StatusCode(500, "Internal server error");
        }
    }

    // POST: api/books
    [HttpPost]
    public async Task<ActionResult<Book>> CreateBook([FromBody] CreateBookRequest request)
    {
        try
        {
            var (resultIsbn, errorMessage) = await _bookStoreService.AddBookAsync(
                request.Isbn, 
                request.Title, 
                request.Pages, 
                request.Year, 
                request.CategoryId);

            if (!string.IsNullOrEmpty(resultIsbn))
            {
                return CreatedAtAction(nameof(GetBooks), new { isbn = resultIsbn }, new { Isbn = resultIsbn });
            }
            else
            {
                return BadRequest(errorMessage);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating book");
            return StatusCode(500, "Internal server error");
        }
    }

    // PUT: api/books/{isbn}
    [HttpPut("{isbn}")]
    public async Task<IActionResult> UpdateBook(string isbn, [FromBody] UpdateBookRequest request)
    {
        try
        {
            var (success, errorMessage) = await _bookStoreService.ModifyBookAsync(
                isbn, 
                request.Title, 
                request.Pages, 
                request.Year, 
                request.CategoryId);

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
            _logger.LogError(ex, "Error updating book with ISBN: {Isbn}", isbn);
            return StatusCode(500, "Internal server error");
        }
    }

    // DELETE: api/books/{isbn}
    [HttpDelete("{isbn}")]
    public async Task<IActionResult> DeleteBook(string isbn)
    {
        try
        {
            var (success, errorMessage) = await _bookStoreService.DeleteBookAsync(isbn);

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
            _logger.LogError(ex, "Error deleting book with ISBN: {Isbn}", isbn);
            return StatusCode(500, "Internal server error");
        }
    }
}

public class CreateBookRequest
{
    public string Isbn { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int? Pages { get; set; }
    public int? Year { get; set; }
    public int? CategoryId { get; set; }
}

public class UpdateBookRequest
{
    public string Title { get; set; } = string.Empty;
    public int? Pages { get; set; }
    public int? Year { get; set; }
    public int? CategoryId { get; set; }
} 