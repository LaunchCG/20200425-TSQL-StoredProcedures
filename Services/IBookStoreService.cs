using CursorConvertedEFCoreApp.Models;

namespace CursorConvertedEFCoreApp.Services;

public interface IBookStoreService
{
    // Author operations
    Task<(int? resultId, string? errorMessage)> AddAuthorAsync(string firstname, string surname, string? surname2 = null);
    Task<IEnumerable<Author>> GetAuthorsAsync(string firstname = "%", string surname = "%");
    Task<(bool success, string? errorMessage)> ModifyAuthorAsync(int authorId, string firstname, string surname, string? surname2 = null);
    Task<(bool success, string? errorMessage)> DeleteAuthorAsync(int authorId);
    
    // Book operations
    Task<(string? resultIsbn, string? errorMessage)> AddBookAsync(string isbn, string title, int? pages = null, int? year = null, int? categoryId = null);
    Task<IEnumerable<BookWithAuthorInfo>> GetBooksAsync(string isbn = "%", string title = "%");
    Task<(bool success, string? errorMessage)> ModifyBookAsync(string isbn, string title, int? pages = null, int? year = null, int? categoryId = null);
    Task<(bool success, string? errorMessage)> DeleteBookAsync(string isbn);
    
    // Category operations
    Task<(int? resultId, string? errorMessage)> AddCategoryAsync(string category);
    
    // AuthorBook operations
    Task<(bool success, string? errorMessage)> AddAuthorBookAsync(int authorId, string isbn);
    Task<(bool success, string? errorMessage)> ModifyAuthorBookAsync(int authorId, string isbn);
}

// DTO for book with author information (equivalent to the stored procedure result)
public class BookWithAuthorInfo
{
    public string Isbn { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int? Pages { get; set; }
    public int? Year { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
} 