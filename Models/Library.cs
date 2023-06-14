using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Library
    {
        public int Id { get; set; }

        [Required(), MaxLength(250)]
        public string Name { get; set; }

        [Required(), MaxLength(1000)]
        [Display(Name = "Library Location")]
        public string Location { get; set; }

        [Required(), MaxLength(300)]
        [Display(Name = "Website URL")]
        public string Website { get; set; }

        [Required()]
        public DateTime Created { get; set; }

        // Child reference
        public List<Book>? Books { get; set; }
    }
}
