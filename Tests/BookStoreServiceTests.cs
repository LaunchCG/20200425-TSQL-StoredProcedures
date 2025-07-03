using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using CursorConvertedEFCoreApp.Data;
using CursorConvertedEFCoreApp.Services;
using CursorConvertedEFCoreApp.Models;

namespace Tests;

/// <summary>
/// Unit tests for BookStoreService that convert all the original T-SQL test scenarios
/// to modern C# xUnit tests using EF Core InMemory database
/// </summary>
public class BookStoreServiceTests : IDisposable
{
    private readonly DbContextOptions<BookStoreContext> _options;
    private readonly Mock<ILogger<BookStoreService>> _mockLogger;
    private readonly BookStoreContext _context;
    private readonly BookStoreService _service;

    public BookStoreServiceTests()
    {
        // Use InMemory database for testing
        _options = new DbContextOptionsBuilder<BookStoreContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _mockLogger = new Mock<ILogger<BookStoreService>>();
        _context = new BookStoreContext(_options);
        _service = new BookStoreService(_context, _mockLogger.Object);

        // Ensure database is created
        _context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    #region Author Tests

    /// <summary>
    /// Test: Test_add_author.sql - Adding an author successfully
    /// Original SQL: EXEC dbo.usp_add_author_storebook 'Emily', 'Vander', 'Veer', @presultid output, @pmsgerror output
    /// </summary>
    [Fact]
    public async Task AddAuthor_WithValidData_ShouldReturnAuthorId()
    {
        // Arrange
        var firstname = "Emily";
        var surname = "Vander";
        var surname2 = "Veer";

        // Act
        var (resultId, errorMessage) = await _service.AddAuthorAsync(firstname, surname, surname2);

        // Assert
        Assert.NotNull(resultId);
        Assert.True(resultId > 0);
        Assert.Null(errorMessage);

        // Verify the author was actually saved to the database
        var savedAuthor = await _context.Authors.FindAsync(resultId);
        Assert.NotNull(savedAuthor);
        Assert.Equal(firstname, savedAuthor.Firstname);
        Assert.Equal(surname, savedAuthor.Surname);
        Assert.Equal(surname2, savedAuthor.Surname2);
    }

    /// <summary>
    /// Test: Test_get_authors.sql - Getting all authors
    /// Original SQL: EXEC dbo.usp_get_authors_storebook
    /// </summary>
    [Fact]
    public async Task GetAuthors_WithDefaultParameters_ShouldReturnAllAuthors()
    {
        // Arrange
        var authors = new List<Author>
        {
            new() { Firstname = "John", Surname = "Doe" },
            new() { Firstname = "Jane", Surname = "Smith" },
            new() { Firstname = "Bob", Surname = "Johnson" }
        };

        await _context.Authors.AddRangeAsync(authors);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetAuthorsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
        Assert.Contains(result, a => a.Firstname == "John" && a.Surname == "Doe");
        Assert.Contains(result, a => a.Firstname == "Jane" && a.Surname == "Smith");
        Assert.Contains(result, a => a.Firstname == "Bob" && a.Surname == "Johnson");
    }

    /// <summary>
    /// Test: Test_modified_author.sql - Modifying an author
    /// Original SQL: EXEC dbo.usp_modified_author_storebook 'Steven', 'W.', 'Disbrow', 2, @presultid output, @pmsgerror output
    /// </summary>
    [Fact]
    public async Task ModifyAuthor_WithValidData_ShouldUpdateAuthor()
    {
        // Arrange
        var author = new Author { Firstname = "John", Surname = "Doe" };
        await _context.Authors.AddAsync(author);
        await _context.SaveChangesAsync();

        var newFirstname = "Steven";
        var newSurname = "W.";
        var newSurname2 = "Disbrow";

        // Act
        var (success, errorMessage) = await _service.ModifyAuthorAsync(
            author.Id, newFirstname, newSurname, newSurname2);

        // Assert
        Assert.True(success);
        Assert.Null(errorMessage);

        // Verify the author was actually updated
        var updatedAuthor = await _context.Authors.FindAsync(author.Id);
        Assert.NotNull(updatedAuthor);
        Assert.Equal(newFirstname, updatedAuthor.Firstname);
        Assert.Equal(newSurname, updatedAuthor.Surname);
        Assert.Equal(newSurname2, updatedAuthor.Surname2);
    }

    /// <summary>
    /// Test: test_delete_author.sql - Deleting an author
    /// Original SQL: EXEC dbo.usp_delete_author_storebook 3, @presultid output, @pmsgerror output
    /// </summary>
    [Fact]
    public async Task DeleteAuthor_WithValidId_ShouldDeleteAuthor()
    {
        // Arrange
        var author = new Author { Firstname = "John", Surname = "Doe" };
        await _context.Authors.AddAsync(author);
        await _context.SaveChangesAsync();

        // Act
        var (success, errorMessage) = await _service.DeleteAuthorAsync(author.Id);

        // Assert
        Assert.True(success);
        Assert.Null(errorMessage);

        // Verify the author was actually deleted
        var deletedAuthor = await _context.Authors.FindAsync(author.Id);
        Assert.Null(deletedAuthor);
    }

    [Fact]
    public async Task DeleteAuthor_WithInvalidId_ShouldReturnError()
    {
        // Act
        var (success, errorMessage) = await _service.DeleteAuthorAsync(999);

        // Assert
        Assert.False(success);
        Assert.NotNull(errorMessage);
        Assert.Contains("not found", errorMessage);
    }

    #endregion

    #region Book Tests

    /// <summary>
    /// Test: Test_add_book.sql - Adding a book successfully
    /// Original SQL: EXEC dbo.usp_add_book_storebook '0764576593', 'JavaScript for dummies', 387, 2005, 1, @presult output, @pmsgerror output
    /// </summary>
    [Fact]
    public async Task AddBook_WithValidData_ShouldReturnIsbn()
    {
        // Arrange
        var isbn = "0764576593";
        var title = "JavaScript for dummies";
        var pages = 387;
        var year = 2005;
        var categoryId = 1;

        // Act
        var (resultIsbn, errorMessage) = await _service.AddBookAsync(isbn, title, pages, year, categoryId);

        // Assert
        Assert.NotNull(resultIsbn);
        Assert.Equal(isbn, resultIsbn);
        Assert.Null(errorMessage);

        // Verify the book was actually saved to the database
        var savedBook = await _context.Books.FindAsync(isbn);
        Assert.NotNull(savedBook);
        Assert.Equal(title, savedBook.Title);
        Assert.Equal(pages, savedBook.Pages);
        Assert.Equal(year, savedBook.Year);
        Assert.Equal(categoryId, savedBook.CategoryId);
    }

    /// <summary>
    /// Test: Test_modified_book.sql - Modifying a book
    /// Original SQL: EXEC dbo.usp_modified_book_storebook '0764576593', 'JavaScript for Dummies', 387, 2005, 1, @presult output, @pmsgerror output
    /// </summary>
    [Fact]
    public async Task ModifyBook_WithValidData_ShouldUpdateBook()
    {
        // Arrange
        var book = new Book 
        { 
            Isbn = "0764576593", 
            Title = "Original Title", 
            Pages = 300, 
            Year = 2000 
        };
        await _context.Books.AddAsync(book);
        await _context.SaveChangesAsync();

        var newTitle = "JavaScript for Dummies";
        var newPages = 387;
        var newYear = 2005;
        var newCategoryId = 1;

        // Act
        var (success, errorMessage) = await _service.ModifyBookAsync(
            book.Isbn, newTitle, newPages, newYear, newCategoryId);

        // Assert
        Assert.True(success);
        Assert.Null(errorMessage);

        // Verify the book was actually updated
        var updatedBook = await _context.Books.FindAsync(book.Isbn);
        Assert.NotNull(updatedBook);
        Assert.Equal(newTitle, updatedBook.Title);
        Assert.Equal(newPages, updatedBook.Pages);
        Assert.Equal(newYear, updatedBook.Year);
        Assert.Equal(newCategoryId, updatedBook.CategoryId);
    }

    /// <summary>
    /// Test: Test_delete_book_storebook.sql - Deleting a book
    /// Original SQL: EXEC dbo.usp_delete_book_storebook '0764576593', @presult output, @pmsgerror output
    /// </summary>
    [Fact]
    public async Task DeleteBook_WithValidIsbn_ShouldDeleteBook()
    {
        // Arrange
        var book = new Book { Isbn = "0764576593", Title = "Test Book" };
        await _context.Books.AddAsync(book);
        await _context.SaveChangesAsync();

        // Act
        var (success, errorMessage) = await _service.DeleteBookAsync(book.Isbn);

        // Assert
        Assert.True(success);
        Assert.Null(errorMessage);

        // Verify the book was actually deleted
        var deletedBook = await _context.Books.FindAsync(book.Isbn);
        Assert.Null(deletedBook);
    }

    [Fact]
    public async Task DeleteBook_WithInvalidIsbn_ShouldReturnError()
    {
        // Act
        var (success, errorMessage) = await _service.DeleteBookAsync("INVALID-ISBN");

        // Assert
        Assert.False(success);
        Assert.NotNull(errorMessage);
        Assert.Contains("not found", errorMessage);
    }

    [Fact]
    public async Task GetBooks_WithValidData_ShouldReturnBooksWithAuthorInfo()
    {
        // Arrange
        var category = new Category { CategoryName = "Programming" };
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();

        var author = new Author { Firstname = "John", Surname = "Doe" };
        await _context.Authors.AddAsync(author);
        await _context.SaveChangesAsync();

        var book = new Book 
        { 
            Isbn = "0764576593", 
            Title = "JavaScript for dummies", 
            Pages = 387, 
            Year = 2005, 
            CategoryId = category.Id 
        };
        await _context.Books.AddAsync(book);
        await _context.SaveChangesAsync();

        var authorBook = new AuthorBook 
        { 
            IdAuthor = author.Id, 
            Isbn = book.Isbn 
        };
        await _context.AuthorBooks.AddAsync(authorBook);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetBooksAsync(book.Isbn, "%");

        // Assert
        Assert.NotNull(result);
        var bookResult = result.FirstOrDefault();
        Assert.NotNull(bookResult);
        Assert.Equal(book.Isbn, bookResult.Isbn);
        Assert.Equal(book.Title, bookResult.Title);
        Assert.Equal(book.Pages, bookResult.Pages);
        Assert.Equal(book.Year, bookResult.Year);
        Assert.Equal(category.CategoryName, bookResult.Category);
        Assert.Equal($"{author.Surname}, {author.Firstname}", bookResult.Author);
    }

    #endregion

    #region Category Tests

    /// <summary>
    /// Test: Test_add_category.sql - Adding a category successfully
    /// Original SQL: EXEC dbo.usp_add_category_storebook 'JavaScript', @presultid output, @pmsgerror output
    /// </summary>
    [Fact]
    public async Task AddCategory_WithValidData_ShouldReturnCategoryId()
    {
        // Arrange
        var categoryName = "JavaScript";

        // Act
        var (resultId, errorMessage) = await _service.AddCategoryAsync(categoryName);

        // Assert
        Assert.NotNull(resultId);
        Assert.True(resultId > 0);
        Assert.Null(errorMessage);

        // Verify the category was actually saved to the database
        var savedCategory = await _context.Categories.FindAsync(resultId);
        Assert.NotNull(savedCategory);
        Assert.Equal(categoryName, savedCategory.CategoryName);
    }

    [Fact]
    public async Task AddCategory_WithDuplicateName_ShouldReturnError()
    {
        // Arrange
        var categoryName = "JavaScript";
        await _service.AddCategoryAsync(categoryName);

        // Act
        var (resultId, errorMessage) = await _service.AddCategoryAsync(categoryName);

        // Assert
        Assert.Null(resultId);
        Assert.NotNull(errorMessage);
    }

    #endregion

    #region AuthorBook Tests

    /// <summary>
    /// Test: Test_add_author_book.sql - Adding author-book relationship
    /// Original SQL: EXEC dbo.usp_add_authorbook_storebook 1, '0764576593', @presult output, @pmsgerror output
    /// </summary>
    [Fact]
    public async Task AddAuthorBook_WithValidData_ShouldCreateRelationship()
    {
        // Arrange
        var author = new Author { Firstname = "John", Surname = "Doe" };
        await _context.Authors.AddAsync(author);
        await _context.SaveChangesAsync();

        var book = new Book { Isbn = "0764576593", Title = "Test Book" };
        await _context.Books.AddAsync(book);
        await _context.SaveChangesAsync();

        // Act
        var (success, errorMessage) = await _service.AddAuthorBookAsync(author.Id, book.Isbn);

        // Assert
        Assert.True(success);
        Assert.Null(errorMessage);

        // Verify the relationship was actually created
        var authorBook = await _context.AuthorBooks
            .FirstOrDefaultAsync(ab => ab.IdAuthor == author.Id && ab.Isbn == book.Isbn);
        Assert.NotNull(authorBook);
    }

    [Fact]
    public async Task AddAuthorBook_WithInvalidAuthorId_ShouldReturnError()
    {
        // Arrange
        var book = new Book { Isbn = "0764576593", Title = "Test Book" };
        await _context.Books.AddAsync(book);
        await _context.SaveChangesAsync();

        // Act
        var (success, errorMessage) = await _service.AddAuthorBookAsync(999, book.Isbn);

        // Assert
        Assert.False(success);
        Assert.NotNull(errorMessage);
    }

    [Fact]
    public async Task ModifyAuthorBook_WithValidData_ShouldUpdateTimestamp()
    {
        // Arrange
        var author = new Author { Firstname = "John", Surname = "Doe" };
        await _context.Authors.AddAsync(author);
        await _context.SaveChangesAsync();

        var book = new Book { Isbn = "0764576593", Title = "Test Book" };
        await _context.Books.AddAsync(book);
        await _context.SaveChangesAsync();

        var authorBook = new AuthorBook 
        { 
            IdAuthor = author.Id, 
            Isbn = book.Isbn, 
            Created = DateTime.Now.AddDays(-1) 
        };
        await _context.AuthorBooks.AddAsync(authorBook);
        await _context.SaveChangesAsync();

        var originalCreated = authorBook.Created;

        // Act
        var (success, errorMessage) = await _service.ModifyAuthorBookAsync(author.Id, book.Isbn);

        // Assert
        Assert.True(success);
        Assert.Null(errorMessage);

        // Verify the timestamp was updated
        var updatedAuthorBook = await _context.AuthorBooks
            .FirstOrDefaultAsync(ab => ab.IdAuthor == author.Id && ab.Isbn == book.Isbn);
        Assert.NotNull(updatedAuthorBook);
        Assert.True(updatedAuthorBook.Created > originalCreated);
    }

    #endregion

    #region Integration Tests

    /// <summary>
    /// Integration test: Complete workflow similar to the original SQL tests
    /// </summary>
    [Fact]
    public async Task CompleteWorkflow_ShouldWorkEndToEnd()
    {
        // 1. Add a category
        var (categoryId, categoryError) = await _service.AddCategoryAsync("Programming");
        Assert.NotNull(categoryId);
        Assert.Null(categoryError);

        // 2. Add an author
        var (authorId, authorError) = await _service.AddAuthorAsync("Emily", "Vander", "Veer");
        Assert.NotNull(authorId);
        Assert.Null(authorError);

        // 3. Add a book
        var (bookIsbn, bookError) = await _service.AddBookAsync(
            "0764576593", "JavaScript for dummies", 387, 2005, categoryId);
        Assert.NotNull(bookIsbn);
        Assert.Null(bookError);

        // 4. Link author to book
        var (linkSuccess, linkError) = await _service.AddAuthorBookAsync(authorId.Value, bookIsbn);
        Assert.True(linkSuccess);
        Assert.Null(linkError);

        // 5. Retrieve and verify the book with author info
        var books = await _service.GetBooksAsync(bookIsbn, "%");
        var bookResult = books.FirstOrDefault();
        Assert.NotNull(bookResult);
        Assert.Equal("JavaScript for dummies", bookResult.Title);
        Assert.Equal("Vander, Emily", bookResult.Author);
        Assert.Equal("Programming", bookResult.Category);

        // 6. Modify the book
        var (modifySuccess, modifyError) = await _service.ModifyBookAsync(
            bookIsbn, "JavaScript for Dummies", 400, 2006, categoryId);
        Assert.True(modifySuccess);
        Assert.Null(modifyError);

        // 7. Verify the modification
        var modifiedBooks = await _service.GetBooksAsync(bookIsbn, "%");
        var modifiedBook = modifiedBooks.FirstOrDefault();
        Assert.NotNull(modifiedBook);
        Assert.Equal("JavaScript for Dummies", modifiedBook.Title);
        Assert.Equal(400, modifiedBook.Pages);
        Assert.Equal(2006, modifiedBook.Year);

        // 8. Delete the book (this should also delete the author-book relationship)
        var (deleteSuccess, deleteError) = await _service.DeleteBookAsync(bookIsbn);
        Assert.True(deleteSuccess);
        Assert.Null(deleteError);

        // 9. Verify the book is deleted
        var deletedBooks = await _service.GetBooksAsync(bookIsbn, "%");
        Assert.Empty(deletedBooks);
    }

    #endregion

    #region Error Handling Tests

    [Fact]
    public async Task AddAuthor_WithNullFirstname_ShouldReturnError()
    {
        // Act
        var (resultId, errorMessage) = await _service.AddAuthorAsync(null!, "Doe");

        // Assert
        Assert.Null(resultId);
        Assert.NotNull(errorMessage);
    }

    [Fact]
    public async Task AddBook_WithDuplicateIsbn_ShouldReturnError()
    {
        // Arrange
        var isbn = "0764576593";
        await _service.AddBookAsync(isbn, "First Book");

        // Act
        var (resultIsbn, errorMessage) = await _service.AddBookAsync(isbn, "Second Book");

        // Assert
        Assert.Null(resultIsbn);
        Assert.NotNull(errorMessage);
    }

    [Fact]
    public async Task ModifyAuthor_WithInvalidId_ShouldReturnError()
    {
        // Act
        var (success, errorMessage) = await _service.ModifyAuthorAsync(999, "John", "Doe");

        // Assert
        Assert.False(success);
        Assert.NotNull(errorMessage);
        Assert.Contains("not found", errorMessage);
    }

    [Fact]
    public async Task ModifyBook_WithInvalidIsbn_ShouldReturnError()
    {
        // Act
        var (success, errorMessage) = await _service.ModifyBookAsync("INVALID-ISBN", "New Title");

        // Assert
        Assert.False(success);
        Assert.NotNull(errorMessage);
        Assert.Contains("not found", errorMessage);
    }

    #endregion
} 