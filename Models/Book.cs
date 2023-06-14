using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required()]
        [Display(Name = "Library")]
        public int LibraryId { get; set; }

        [Required(), MaxLength(250)]
        public string Title { get; set; }
        
        public string Summary { get; set; }

        [Required(), MaxLength(150)]
        public string Author { get; set; }

        [Required()]
        public DateTime Published { get; set; }

        public string? Photo { get; set; }

        // Parent reference
        public Library? Library { get; set; }
    }
}
