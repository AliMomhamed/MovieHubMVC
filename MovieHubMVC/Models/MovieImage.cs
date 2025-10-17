using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MovieHubMVC.Models;

public partial class MovieImage
{
    [Key]
    public int Id { get; set; }

    [StringLength(250)]
    public string? ImageUrl { get; set; }

    public int MovieId { get; set; }

    [ForeignKey("MovieId")]
    [InverseProperty("MovieImages")]
    public virtual Movie Movie { get; set; } = null!;
}
