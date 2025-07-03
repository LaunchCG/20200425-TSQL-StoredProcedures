# T-SQL Stored Procedures to EF Core Conversion

This project demonstrates the conversion of T-SQL stored procedures to idiomatic C# Entity Framework Core methods using async/await patterns and parameterized queries.

## Overview

The original T-SQL stored procedures from the BookStore database have been converted to modern C# EF Core methods that follow best practices:

- **Async/Await Pattern**: All database operations are asynchronous
- **Parameterized Queries**: EF Core automatically handles parameterization to prevent SQL injection
- **Strongly Typed Models**: Full type safety with C# entity models
- **Dependency Injection**: Proper service registration and dependency management
- **Error Handling**: Comprehensive exception handling with logging
- **RESTful API**: Clean API endpoints following REST conventions

## Database Schema

The BookStore database contains the following entities:

- **Authors**: Author information (Id, Firstname, Surname, Surname2)
- **Books**: Book information (ISBN, Title, Pages, Year, CategoryId)
- **Categories**: Book categories (Id, CategoryName)
- **AuthorBook**: Many-to-many relationship between Authors and Books

## Converted Stored Procedures

### Author Operations
- `usp_add_author_storebook` → `AddAuthorAsync()`
- `usp_get_authors_storebook` → `GetAuthorsAsync()`
- `usp_modified_author_storebook` → `ModifyAuthorAsync()`
- `usp_delete_author_storebook` → `DeleteAuthorAsync()`

### Book Operations
- `usp_add_book_storebook` → `AddBookAsync()`
- `usp_get_books_storebook` → `GetBooksAsync()`
- `usp_modified_book_storebook` → `ModifyBookAsync()`
- `usp_delete_book_storebook` → `DeleteBookAsync()`

### Category Operations
- `usp_add_category_storebook` → `AddCategoryAsync()`

### AuthorBook Operations
- `usp_add_authorbook_storebook` → `AddAuthorBookAsync()`
- `usp_modified_authorbook_storebook` → `ModifyAuthorBookAsync()`

## Key Improvements

### 1. Type Safety
- Strongly typed C# models instead of dynamic SQL results
- Compile-time validation of parameters and return types

### 2. Performance
- EF Core query optimization and caching
- Lazy loading and eager loading options
- Connection pooling managed by EF Core

### 3. Security
- Automatic parameterization prevents SQL injection
- Input validation through data annotations

### 4. Maintainability
- Clean separation of concerns with service layer
- Dependency injection for testability
- Comprehensive logging and error handling

### 5. Modern Patterns
- Async/await for non-blocking operations
- Repository pattern implementation
- RESTful API design

## API Endpoints

### Authors
- `GET /api/authors` - Get all authors (with optional filtering)
- `POST /api/authors` - Create a new author
- `PUT /api/authors/{id}` - Update an author
- `DELETE /api/authors/{id}` - Delete an author

### Books
- `GET /api/books` - Get all books (with optional filtering)
- `POST /api/books` - Create a new book
- `PUT /api/books/{isbn}` - Update a book
- `DELETE /api/books/{isbn}` - Delete a book

### Categories
- `POST /api/categories` - Create a new category

### AuthorBooks
- `POST /api/authorbooks` - Create author-book relationship
- `PUT /api/authorbooks/{authorId}/{isbn}` - Update author-book relationship

## Setup Instructions

1. **Database Setup**
   ```sql
   -- Run the createScript.sql to create the BookStore database
   ```

2. **Connection String**
   Update the connection string in `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=your-server;Database=BookStore;Trusted_Connection=true;TrustServerCertificate=true;MultipleActiveResultSets=true"
     }
   }
   ```

3. **Run the Application**
   ```bash
   dotnet run
   ```

4. **Access Swagger UI**
   Navigate to `https://localhost:7001/swagger` to test the API endpoints.

## Migration from T-SQL to EF Core

### Before (T-SQL Stored Procedure)
```sql
CREATE PROCEDURE [dbo].[usp_add_author_storebook] 
    @pfirstname varchar(128),
    @psurname varchar(128),
    @psurname2 varchar(128) = null,
    @presultid int out,
    @pmsgerror varchar(256) out
AS
BEGIN TRY
    INSERT INTO Authors(Firstname,Surname,Surname2)
    VALUES(@pfirstname,@psurname,@psurname2);
    IF @@ROWCOUNT > 0 AND @@ERROR = 0
        SELECT @presultid = Id FROM Authors
        WHERE Id = SCOPE_IDENTITY();
END TRY
BEGIN CATCH
    SET @pmsgerror = convert(varchar(8),ERROR_LINE()) + ': ' + ERROR_MESSAGE()
END CATCH
```

### After (C# EF Core Method)
```csharp
public async Task<(int? resultId, string? errorMessage)> AddAuthorAsync(
    string firstname, string surname, string? surname2 = null)
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
```

## Benefits of the Conversion

1. **Developer Experience**: IntelliSense, compile-time checking, and better debugging
2. **Performance**: EF Core's query optimization and caching
3. **Security**: Automatic parameterization and input validation
4. **Maintainability**: Clean, testable, and well-structured code
5. **Scalability**: Async operations and connection pooling
6. **Modern Standards**: Following current .NET and EF Core best practices

## Testing

The application includes comprehensive error handling and logging. You can test the endpoints using:

- Swagger UI (built-in)
- Postman or similar API testing tools
- Unit tests (can be added using xUnit and Moq)

All tests will run using the in-memory EF Core provider, so no real database is needed.

**How to run the tests:**
1. From your project root, run:
   ```sh
   dotnet test
   ```

If you want to add more test scenarios or need help with test coverage for controllers or other layers, let me know!

## Dependencies

- .NET 9.0
- Entity Framework Core 9.0.6
- ASP.NET Core Web API
- SQL Server (or SQL Server Express)
