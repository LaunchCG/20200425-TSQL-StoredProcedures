using System.ComponentModel.DataAnnotations;

namespace CursorConvertedEFCoreApp.Models;

public class Category
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(64)]
    public string CategoryName { get; set; } = string.Empty;
    
    // Navigation properties
    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
} 