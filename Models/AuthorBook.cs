using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CursorConvertedEFCoreApp.Models;

public class AuthorBook
{
    [Required]
    public int IdAuthor { get; set; }
    
    [Required]
    [MaxLength(13)]
    public string Isbn { get; set; } = string.Empty;
    
    public DateTime Created { get; set; } = DateTime.Now;
    
    // Navigation properties
    [ForeignKey(nameof(IdAuthor))]
    public virtual Author Author { get; set; } = null!;
    
    [ForeignKey(nameof(Isbn))]
    public virtual Book Book { get; set; } = null!;
} 