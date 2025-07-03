using CursorConvertedEFCoreApp.Services;
using CursorConvertedEFCoreApp.Models;

namespace CursorConvertedEFCoreApp.Examples;

/// <summary>
/// Examples demonstrating how to use the converted EF Core methods
/// These examples show the equivalent usage of the original T-SQL stored procedures
/// </summary>
public class UsageExamples
{
    private readonly IBookStoreService _bookStoreService;

    public UsageExamples(IBookStoreService bookStoreService)
    {
        _bookStoreService = bookStoreService;
    }

    /// <summary>
    /// Example: Adding an author (equivalent to usp_add_author_storebook)
    /// </summary>
    public async Task ExampleAddAuthor()
    {
        // Original T-SQL:
        // EXEC dbo.usp_add_author_storebook 'John', 'Doe', 'Smith', @presultid output, @pmsgerror output

        var (resultId, errorMessage) = await _bookStoreService.AddAuthorAsync(
            firstname: "John",
            surname: "Doe",
            surname2: "Smith"
        );

        if (resultId.HasValue)
        {
            Console.WriteLine($"Author added successfully with ID: {resultId}");
        }
        else
        {
            Console.WriteLine($"Error adding author: {errorMessage}");
        }
    }

    /// <summary>
    /// Example: Getting authors with filtering (equivalent to usp_get_authors_storebook)
    /// </summary>
    public async Task ExampleGetAuthors()
    {
        // Original T-SQL:
        // EXEC dbo.usp_get_authors_storebook 'John', 'Doe'

        var authors = await _bookStoreService.GetAuthorsAsync(
            firstname: "John",
            surname: "Doe"
        );

        foreach (var author in authors)
        {
            Console.WriteLine($"Author: {author.Firstname} {author.Surname}");
        }
    }

    /// <summary>
    /// Example: Adding a book (equivalent to usp_add_book_storebook)
    /// </summary>
    public async Task ExampleAddBook()
    {
        // Original T-SQL:
        // EXEC dbo.usp_add_book_storebook '0764576593', 'JavaScript for dummies', 387, 2005, 1, @presult output, @pmsgerror output

        var (resultIsbn, errorMessage) = await _bookStoreService.AddBookAsync(
            isbn: "0764576593",
            title: "JavaScript for dummies",
            pages: 387,
            year: 2005,
            categoryId: 1
        );

        if (!string.IsNullOrEmpty(resultIsbn))
        {
            Console.WriteLine($"Book added successfully with ISBN: {resultIsbn}");
        }
        else
        {
            Console.WriteLine($"Error adding book: {errorMessage}");
        }
    }

    /// <summary>
    /// Example: Getting books with filtering (equivalent to usp_get_books_storebook)
    /// </summary>
    public async Task ExampleGetBooks()
    {
        // Original T-SQL:
        // EXEC dbo.usp_get_books_storebook '0764576593', '%'

        var books = await _bookStoreService.GetBooksAsync(
            isbn: "0764576593",
            title: "%"
        );

        foreach (var book in books)
        {
            Console.WriteLine($"Book: {book.Title} by {book.Author} (Category: {book.Category})");
        }
    }

    /// <summary>
    /// Example: Adding a category (equivalent to usp_add_category_storebook)
    /// </summary>
    public async Task ExampleAddCategory()
    {
        // Original T-SQL:
        // EXEC dbo.usp_add_category_storebook 'Programming', @presultid output, @pmsgerror output

        var (resultId, errorMessage) = await _bookStoreService.AddCategoryAsync("Programming");

        if (resultId.HasValue)
        {
            Console.WriteLine($"Category added successfully with ID: {resultId}");
        }
        else
        {
            Console.WriteLine($"Error adding category: {errorMessage}");
        }
    }

    /// <summary>
    /// Example: Adding author-book relationship (equivalent to usp_add_authorbook_storebook)
    /// </summary>
    public async Task ExampleAddAuthorBook()
    {
        // Original T-SQL:
        // EXEC dbo.usp_add_authorbook_storebook 1, '0764576593', @presult output, @pmsgerror output

        var (success, errorMessage) = await _bookStoreService.AddAuthorBookAsync(
            authorId: 1,
            isbn: "0764576593"
        );

        if (success)
        {
            Console.WriteLine("Author-Book relationship added successfully");
        }
        else
        {
            Console.WriteLine($"Error adding author-book relationship: {errorMessage}");
        }
    }

    /// <summary>
    /// Example: Modifying a book (equivalent to usp_modified_book_storebook)
    /// </summary>
    public async Task ExampleModifyBook()
    {
        // Original T-SQL:
        // EXEC dbo.usp_modified_book_storebook '0764576593', 'Updated Title', 400, 2006, 1, @presult output, @pmsgerror output

        var (success, errorMessage) = await _bookStoreService.ModifyBookAsync(
            isbn: "0764576593",
            title: "Updated Title",
            pages: 400,
            year: 2006,
            categoryId: 1
        );

        if (success)
        {
            Console.WriteLine("Book modified successfully");
        }
        else
        {
            Console.WriteLine($"Error modifying book: {errorMessage}");
        }
    }

    /// <summary>
    /// Example: Deleting a book (equivalent to usp_delete_book_storebook)
    /// </summary>
    public async Task ExampleDeleteBook()
    {
        // Original T-SQL:
        // EXEC dbo.usp_delete_book_storebook '0764576593', @presult output, @pmsgerror output

        var (success, errorMessage) = await _bookStoreService.DeleteBookAsync("0764576593");

        if (success)
        {
            Console.WriteLine("Book deleted successfully");
        }
        else
        {
            Console.WriteLine($"Error deleting book: {errorMessage}");
        }
    }

    /// <summary>
    /// Example: Complete workflow - Add category, author, book, and link them
    /// </summary>
    public async Task ExampleCompleteWorkflow()
    {
        Console.WriteLine("=== Complete Workflow Example ===");

        // 1. Add a category
        var (categoryId, categoryError) = await _bookStoreService.AddCategoryAsync("Fiction");
        if (!categoryId.HasValue)
        {
            Console.WriteLine($"Failed to add category: {categoryError}");
            return;
        }
        Console.WriteLine($"Category added with ID: {categoryId}");

        // 2. Add an author
        var (authorId, authorError) = await _bookStoreService.AddAuthorAsync("Jane", "Smith");
        if (!authorId.HasValue)
        {
            Console.WriteLine($"Failed to add author: {authorError}");
            return;
        }
        Console.WriteLine($"Author added with ID: {authorId}");

        // 3. Add a book
        var (bookIsbn, bookError) = await _bookStoreService.AddBookAsync(
            "1234567890123",
            "The Great Adventure",
            pages: 300,
            year: 2024,
            categoryId: categoryId
        );
        if (string.IsNullOrEmpty(bookIsbn))
        {
            Console.WriteLine($"Failed to add book: {bookError}");
            return;
        }
        Console.WriteLine($"Book added with ISBN: {bookIsbn}");

        // 4. Link author to book
        var (linkSuccess, linkError) = await _bookStoreService.AddAuthorBookAsync(authorId.Value, bookIsbn);
        if (!linkSuccess)
        {
            Console.WriteLine($"Failed to link author to book: {linkError}");
            return;
        }
        Console.WriteLine("Author linked to book successfully");

        // 5. Retrieve and display the book with author info
        var books = await _bookStoreService.GetBooksAsync(bookIsbn, "%");
        foreach (var book in books)
        {
            Console.WriteLine($"Retrieved: {book.Title} by {book.Author} (Category: {book.Category})");
        }

        Console.WriteLine("=== Workflow completed successfully ===");
    }
} 