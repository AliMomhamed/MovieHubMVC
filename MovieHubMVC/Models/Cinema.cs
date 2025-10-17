using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MovieHubMVC.Models;

public partial class Cinema
{
    [Key]
    public int Id { get; set; }

    [StringLength(150)]
    public string Name { get; set; } = null!;

    [StringLength(250)]
    public string? Location { get; set; }

    [StringLength(250)]
    public string? ImageUrl { get; set; }

    [InverseProperty("Cinema")]
    public virtual ICollection<Movie> Movies { get; set; } = new List<Movie>();
}
