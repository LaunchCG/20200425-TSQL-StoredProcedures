using Microsoft.EntityFrameworkCore;
using CursorConvertedEFCoreApp.Models;

namespace CursorConvertedEFCoreApp.Data;

public class BookStoreContext : DbContext
{
    public BookStoreContext(DbContextOptions<BookStoreContext> options) : base(options)
    {
    }
    
    public DbSet<Author> Authors { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<AuthorBook> AuthorBooks { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure AuthorBook as a many-to-many relationship table
        modelBuilder.Entity<AuthorBook>()
            .HasKey(ab => new { ab.IdAuthor, ab.Isbn });
        
        modelBuilder.Entity<AuthorBook>()
            .HasOne(ab => ab.Author)
            .WithMany(a => a.AuthorBooks)
            .HasForeignKey(ab => ab.IdAuthor)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<AuthorBook>()
            .HasOne(ab => ab.Book)
            .WithMany(b => b.AuthorBooks)
            .HasForeignKey(ab => ab.Isbn)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Configure Category unique constraint
        modelBuilder.Entity<Category>()
            .HasIndex(c => c.CategoryName)
            .IsUnique();
        
        // Configure Book-Category relationship
        modelBuilder.Entity<Book>()
            .HasOne(b => b.Category)
            .WithMany(c => c.Books)
            .HasForeignKey(b => b.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);
    }
} 