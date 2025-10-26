using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieHubMVC.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public double Price { get; set; }

        public string? Status { get; set; }

        public DateTime DateTime { get; set; } = DateTime.Now;

        public int CinemaId { get; set; }
        public Cinema? Cinema { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public ICollection<Actor> Actors { get; set; } = new List<Actor>();

        public string? MainImg { get; set; }

       
        public string? ImageUrl { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        [NotMapped]
        public List<IFormFile>? UploadedImages { get; set; }

        [NotMapped]
        public List<int>? SelectedActorsIds { get; set; }


        public ICollection<MovieImage> MovieImages { get; set; } = new List<MovieImage>();
    }
}
