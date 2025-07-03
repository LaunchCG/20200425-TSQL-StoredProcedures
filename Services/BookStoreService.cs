using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CursorConvertedEFCoreApp.Data;
using CursorConvertedEFCoreApp.Models;

namespace CursorConvertedEFCoreApp.Services;

public class BookStoreService : IBookStoreService
{
    private readonly BookStoreContext _context;
    private readonly ILogger<BookStoreService> _logger;

    public BookStoreService(BookStoreContext context, ILogger<BookStoreService> logger)
    {
        _context = context;
        _logger = logger;
    }

    // Author operations - converted from usp_add_author_storebook
    public async Task<(int? resultId, string? errorMessage)> AddAuthorAsync(string firstname, string surname, string? surname2 = null)
    {
        try
        {
            var author = new Author
            {
                Firstname = firstname,
                Surname = surname,
                Surname2 = surname2
            };

            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            return (author.Id, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding author: {Firstname} {Surname}", firstname, surname);
            return (null, $"Error: {ex.Message}");
        }
    }

    // Converted from usp_get_authors_storebook
    public async Task<IEnumerable<Author>> GetAuthorsAsync(string firstname = "%", string surname = "%")
    {
        var query = _context.Authors.AsQueryable();

        if (firstname != "%")
        {
            query = query.Where(a => a.Firstname.Contains(firstname));
        }

        if (surname != "%")
        {
            query = query.Where(a => a.Surname.Contains(surname));
        }

        return await query.ToListAsync();
    }

    // Converted from usp_modified_author_storebook
    public async Task<(bool success, string? errorMessage)> ModifyAuthorAsync(int authorId, string firstname, string surname, string? surname2 = null)
    {
        try
        {
            var author = await _context.Authors.FindAsync(authorId);
            if (author == null)
            {
                return (false, "Author not found");
            }

            author.Firstname = firstname;
            author.Surname = surname;
            author.Surname2 = surname2;

            await _context.SaveChangesAsync();
            return (true, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error modifying author with ID: {AuthorId}", authorId);
            return (false, $"Error: {ex.Message}");
        }
    }

    // Converted from usp_delete_author_storebook
    public async Task<(bool success, string? errorMessage)> DeleteAuthorAsync(int authorId)
    {
        try
        {
            var author = await _context.Authors.FindAsync(authorId);
            if (author == null)
            {
                return (false, "Author not found");
            }

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();
            return (true, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting author with ID: {AuthorId}", authorId);
            return (false, $"Error: {ex.Message}");
        }
    }

    // Book operations - converted from usp_add_book_storebook
    public async Task<(string? resultIsbn, string? errorMessage)> AddBookAsync(string isbn, string title, int? pages = null, int? year = null, int? categoryId = null)
    {
        try
        {
            var book = new Book
            {
                Isbn = isbn,
                Title = title,
                Pages = pages,
                Year = year,
                CategoryId = categoryId
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return (book.Isbn, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding book with ISBN: {Isbn}", isbn);
            return (null, $"Error: {ex.Message}");
        }
    }

    // Converted from usp_get_books_storebook
    public async Task<IEnumerable<BookWithAuthorInfo>> GetBooksAsync(string isbn = "%", string title = "%")
    {
        var query = from b in _context.Books
                    join cat in _context.Categories on b.CategoryId equals cat.Id into categoryJoin
                    from cat in categoryJoin.DefaultIfEmpty()
                    join ab in _context.AuthorBooks on b.Isbn equals ab.Isbn into authorBookJoin
                    from ab in authorBookJoin.DefaultIfEmpty()
                    join a in _context.Authors on ab.IdAuthor equals a.Id into authorJoin
                    from a in authorJoin.DefaultIfEmpty()
                    where (isbn == "%" || b.Isbn.Contains(isbn)) &&
                          (title == "%" || b.Title.Contains(title))
                    select new BookWithAuthorInfo
                    {
                        Isbn = b.Isbn,
                        Title = b.Title,
                        Pages = b.Pages,
                        Year = b.Year,
                        Category = cat != null ? cat.CategoryName : string.Empty,
                        Author = a != null ? $"{a.Surname}, {a.Firstname}" : string.Empty
                    };

        return await query.ToListAsync();
    }

    // Converted from usp_modified_book_storebook
    public async Task<(bool success, string? errorMessage)> ModifyBookAsync(string isbn, string title, int? pages = null, int? year = null, int? categoryId = null)
    {
        try
        {
            var book = await _context.Books.FindAsync(isbn);
            if (book == null)
            {
                return (false, "Book not found");
            }

            book.Title = title;
            book.Pages = pages;
            book.Year = year;
            book.CategoryId = categoryId;

            await _context.SaveChangesAsync();
            return (true, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error modifying book with ISBN: {Isbn}", isbn);
            return (false, $"Error: {ex.Message}");
        }
    }

    // Converted from usp_delete_book_storebook
    public async Task<(bool success, string? errorMessage)> DeleteBookAsync(string isbn)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var book = await _context.Books.FindAsync(isbn);
            if (book == null)
            {
                return (false, "Book not found");
            }

            // Delete associated AuthorBook records first (cascade will handle this automatically)
            var authorBooks = await _context.AuthorBooks
                .Where(ab => ab.Isbn == isbn)
                .ToListAsync();

            if (authorBooks.Any())
            {
                _context.AuthorBooks.RemoveRange(authorBooks);
            }

            // Delete the book
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            return (true, null);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error deleting book with ISBN: {Isbn}", isbn);
            return (false, $"Error: {ex.Message}");
        }
    }

    // Category operations - converted from usp_add_category_storebook
    public async Task<(int? resultId, string? errorMessage)> AddCategoryAsync(string category)
    {
        try
        {
            var categoryEntity = new Category
            {
                CategoryName = category
            };

            _context.Categories.Add(categoryEntity);
            await _context.SaveChangesAsync();

            return (categoryEntity.Id, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding category: {Category}", category);
            return (null, $"Error: {ex.Message}");
        }
    }

    // AuthorBook operations - converted from usp_add_authorbook_storebook
    public async Task<(bool success, string? errorMessage)> AddAuthorBookAsync(int authorId, string isbn)
    {
        try
        {
            var authorBook = new AuthorBook
            {
                IdAuthor = authorId,
                Isbn = isbn,
                Created = DateTime.Now
            };

            _context.AuthorBooks.Add(authorBook);
            await _context.SaveChangesAsync();

            return (true, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding author-book relationship: AuthorId={AuthorId}, ISBN={Isbn}", authorId, isbn);
            return (false, $"Error: {ex.Message}");
        }
    }

    // Converted from usp_modified_authorbook_storebook
    public async Task<(bool success, string? errorMessage)> ModifyAuthorBookAsync(int authorId, string isbn)
    {
        try
        {
            var authorBook = await _context.AuthorBooks
                .FirstOrDefaultAsync(ab => ab.IdAuthor == authorId && ab.Isbn == isbn);

            if (authorBook == null)
            {
                return (false, "Author-Book relationship not found");
            }

            // Update the Created timestamp
            authorBook.Created = DateTime.Now;
            await _context.SaveChangesAsync();

            return (true, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error modifying author-book relationship: AuthorId={AuthorId}, ISBN={Isbn}", authorId, isbn);
            return (false, $"Error: {ex.Message}");
        }
    }
} 