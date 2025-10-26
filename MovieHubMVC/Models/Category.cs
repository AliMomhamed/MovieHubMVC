using System.ComponentModel.DataAnnotations;

namespace MovieHubMVC.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public ICollection<Movie> Movies { get; set; } = new List<Movie>();
    }
}

