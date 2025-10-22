using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieHubMVC.Models
{
    public class MovieImage
    {
        [Key]
        public int Id { get; set; }

        [StringLength(250)]
        public string ImageUrl { get; set; } = null!;

        public int MovieId { get; set; }
        [ForeignKey("MovieId")]
        public virtual Movie Movie { get; set; } = null!;
    }
}
