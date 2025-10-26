using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieHubMVC.Models
{
    public class MovieImage
    {
        public int Id { get; set; }

        [Required]
        public string ImageUrl { get; set; } = string.Empty;

        public int MovieId { get; set; }

        [ForeignKey(nameof(MovieId))]
        public Movie? Movie { get; set; }
    }
}
