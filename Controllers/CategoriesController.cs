using Microsoft.AspNetCore.Mvc;
using CursorConvertedEFCoreApp.Services;
using CursorConvertedEFCoreApp.Models;

namespace CursorConvertedEFCoreApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IBookStoreService _bookStoreService;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(IBookStoreService bookStoreService, ILogger<CategoriesController> logger)
    {
        _bookStoreService = bookStoreService;
        _logger = logger;
    }

    // POST: api/categories
    [HttpPost]
    public async Task<ActionResult<Category>> CreateCategory([FromBody] CreateCategoryRequest request)
    {
        try
        {
            var (resultId, errorMessage) = await _bookStoreService.AddCategoryAsync(request.CategoryName);

            if (resultId.HasValue)
            {
                return CreatedAtAction(nameof(CreateCategory), new { id = resultId }, new { Id = resultId });
            }
            else
            {
                return BadRequest(errorMessage);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category");
            return StatusCode(500, "Internal server error");
        }
    }
}

public class CreateCategoryRequest
{
    public string CategoryName { get; set; } = string.Empty;
} 