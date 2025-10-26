using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieHubMVC.Data;
using MovieHubMVC.Models;

namespace MovieHubMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : BaseAdminController<Category>
    {
        public CategoryController(ApplicationDbContext context, IWebHostEnvironment env)
            : base(context, env) { }

        public override async Task<IActionResult> Index(CancellationToken token)
        {
            var cats = await _context.Categories.Include(c => c.Movies).ToListAsync(token);
            return View(cats);
        }
    }
}
