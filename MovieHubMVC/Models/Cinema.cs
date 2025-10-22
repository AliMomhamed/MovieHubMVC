using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace MovieHubMVC.Models
{
    public partial class Cinema
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Cinema name is required")]
        [StringLength(150)]
        public string Name { get; set; } = null!;

        [StringLength(250)]
        public string? Location { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        // 🔹 رابط الصورة بعد الحفظ في السيرفر
        [StringLength(250)]
        public string? ImageUrl { get; set; }

        // 🔹 الصورة التي يرفعها المستخدم (لن تُخزن في قاعدة البيانات)
        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        // 🔹 قائمة الأفلام المرتبطة بالسينما
        public virtual ICollection<Movie> Movies { get; set; } = new List<Movie>();
    }
}
