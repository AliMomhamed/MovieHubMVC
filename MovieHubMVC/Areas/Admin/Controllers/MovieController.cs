using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieHubMVC.Data;
using MovieHubMVC.Models;

namespace MovieHubMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MovieController : BaseAdminController<Movie>
    {
        public MovieController(ApplicationDbContext context, IWebHostEnvironment env)
            : base(context, env) { }

        public override async Task<IActionResult> Index(CancellationToken token)
        {
            var movies = await _context.Movies
                .Include(m => m.Cinema)
                .Include(m => m.Category)
                .Include(m => m.Actors)
                .ToListAsync(token);

            return View(movies);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override async Task<IActionResult> Create(Movie movie, CancellationToken token)
        {
            if (!ModelState.IsValid) return View(movie);

            if (movie.ImageFile != null)
                movie.ImageUrl = await SaveFileAsync(movie.ImageFile, "movies", token);

            _dbSet.Add(movie);
            await _context.SaveChangesAsync(token);
            return RedirectToAction(nameof(Index));
        }
    }
}
