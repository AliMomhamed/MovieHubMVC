using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace MovieHubMVC.Models
{
    public partial class Movie
    {
        [Key]
        public int Id { get; set; }

        [StringLength(150)]
        [Required]
        public string Name { get; set; } = null!;

        [StringLength(500)]
        public string? Description { get; set; }

        public double Price { get; set; }

        [StringLength(250)]
        public string? MainImg { get; set; }

        public DateTime DateTime { get; set; } = DateTime.Now;

        public bool Status { get; set; } = true;

        // ✅ Category (اختياري)
        [ForeignKey("Category")]
        public int? CategoryId { get; set; }
        public virtual Category? Category { get; set; }

        // ✅ Cinema (اختياري)
        [ForeignKey("Cinema")]
        public int? CinemaId { get; set; }
        public virtual Cinema? Cinema { get; set; }

        // ✅ Actors (Many-to-Many)
        public virtual ICollection<Actor> Actors { get; set; } = new List<Actor>();

        // ✅ Movie Images (One-to-Many)
        public virtual ICollection<MovieImage> MovieImages { get; set; } = new List<MovieImage>();

        // ✅ Not mapped fields for upload
        [NotMapped]
        public List<int> SelectedActorsIds { get; set; } = new();

        [NotMapped]
        public List<IFormFile>? UploadedImages { get; set; }
    }
}
