using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MovieHubMVC.Models;

public partial class Movie
{
    [Key]
    public int Id { get; set; }

    [StringLength(200)]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? Price { get; set; }

    [StringLength(50)]
    public string? Status { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DateTime { get; set; }

    [StringLength(250)]
    public string? MainImg { get; set; }

    public int CategoryId { get; set; }

    public int CinemaId { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("Movies")]
    public virtual Category Category { get; set; } = null!;

    [ForeignKey("CinemaId")]
    [InverseProperty("Movies")]
    public virtual Cinema Cinema { get; set; } = null!;

    [InverseProperty("Movie")]
    public virtual ICollection<MovieImage> MovieImages { get; set; } = new List<MovieImage>();

    [ForeignKey("MovieId")]
    [InverseProperty("Movies")]
    public virtual ICollection<Actor> Actors { get; set; } = new List<Actor>();
}
