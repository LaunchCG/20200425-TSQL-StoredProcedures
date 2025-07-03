using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CursorConvertedEFCoreApp.Models;

public class Book
{
    [Key]
    [MaxLength(13)]
    public string Isbn { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(256)]
    public string Title { get; set; } = string.Empty;
    
    public int? Pages { get; set; }
    
    public int? Year { get; set; }
    
    public int? CategoryId { get; set; }
    
    // Navigation properties
    [ForeignKey(nameof(CategoryId))]
    public virtual Category? Category { get; set; }
    
    public virtual ICollection<AuthorBook> AuthorBooks { get; set; } = new List<AuthorBook>();
} 