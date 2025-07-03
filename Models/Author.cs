using System.ComponentModel.DataAnnotations;

namespace CursorConvertedEFCoreApp.Models;

public class Author
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(128)]
    public string Firstname { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(128)]
    public string Surname { get; set; } = string.Empty;
    
    [MaxLength(128)]
    public string? Surname2 { get; set; }
    
    // Navigation properties
    public virtual ICollection<AuthorBook> AuthorBooks { get; set; } = new List<AuthorBook>();
} 