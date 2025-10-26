using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieHubMVC.Models
{
    public class Cinema
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        public ICollection<Movie> Movies { get; set; } = new List<Movie>();
    }
}
