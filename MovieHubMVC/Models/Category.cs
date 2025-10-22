using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MovieHubMVC.Models
{
    public partial class Category
    {
        [Key]
        public int Id { get; set; }

        [StringLength(100)]
        public string Name { get; set; } = null!;

        [StringLength(250)]
        public string? Description { get; set; }

        public virtual ICollection<Movie> Movies { get; set; } = new List<Movie>();
    }
}
