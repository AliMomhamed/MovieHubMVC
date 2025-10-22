using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieHubMVC.Data;
using MovieHubMVC.Models;

namespace MovieHubMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MovieController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public MovieController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ✅ INDEX
        public async Task<IActionResult> Index()
        {
            var movies = await _context.Movies
                .Include(m => m.Cinema)
                .Include(m => m.Category)
                .Include(m => m.Actors)
                .Include(m => m.MovieImages)
                .ToListAsync();

            return View(movies);
        }

        // ✅ CREATE GET
        public IActionResult Create()
        {
            LoadDropdownLists();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Movie movie)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdownLists(movie);
                return View(movie);
            }

            // 🔹 حفظ الفيلم أولاً
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            // 🔹 حفظ الصور المرفوعة
            if (movie.UploadedImages != null && movie.UploadedImages.Any())
            {
                var uploadPath = Path.Combine(_env.WebRootPath, "images/movies");
                Directory.CreateDirectory(uploadPath);

                foreach (var file in movie.UploadedImages)
                {
                    if (file.Length > 0)
                    {
                        var uniqueName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                        var filePath = Path.Combine(uploadPath, uniqueName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                            await file.CopyToAsync(stream);

                        var img = new MovieImage
                        {
                            MovieId = movie.Id,
                            ImageUrl = "/images/movies/" + uniqueName
                        };
                        _context.MovieImages.Add(img);

                        // أول صورة تبقى الصورة الرئيسية
                        if (string.IsNullOrEmpty(movie.MainImg))
                            movie.MainImg = img.ImageUrl;
                    }
                }
                await _context.SaveChangesAsync();
            }

            // 🔹 الممثلين (لو اختار)
            if (movie.SelectedActorsIds != null && movie.SelectedActorsIds.Any())
            {
                var actors = await _context.Actors
                    .Where(a => movie.SelectedActorsIds.Contains(a.Id))
                    .ToListAsync();
                movie.Actors = actors;
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "✅ Movie added successfully!";
            return RedirectToAction(nameof(Index));
        }


        // ✅ EDIT GET
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var movie = await _context.Movies
                .Include(m => m.Actors)
                .Include(m => m.MovieImages)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null) return NotFound();

            movie.SelectedActorsIds = movie.Actors.Select(a => a.Id).ToList();
            LoadDropdownLists(movie);
            return View(movie);
        }

        // ✅ EDIT POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Movie movie)
        {
            if (id != movie.Id) return NotFound();

            var existing = await _context.Movies
                .Include(m => m.Actors)
                .Include(m => m.MovieImages)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (existing == null) return NotFound();

            if (!ModelState.IsValid)
            {
                LoadDropdownLists(movie);
                return View(movie);
            }

            // تحديث البيانات الأساسية
            existing.Name = movie.Name;
            existing.Description = movie.Description;
            existing.Price = movie.Price;
            existing.Status = movie.Status;
            existing.DateTime = movie.DateTime;
            existing.CinemaId = movie.CinemaId;
            existing.CategoryId = movie.CategoryId;

            // 🔹 تحديث الممثلين
            existing.Actors.Clear();
            if (movie.SelectedActorsIds != null)
            {
                existing.Actors = await _context.Actors
                    .Where(a => movie.SelectedActorsIds.Contains(a.Id))
                    .ToListAsync();
            }

            // 🔹 رفع صور جديدة
            if (movie.UploadedImages != null && movie.UploadedImages.Any())
            {
                var uploadDir = Path.Combine(_env.WebRootPath, "images/movies");
                Directory.CreateDirectory(uploadDir);

                foreach (var file in movie.UploadedImages)
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    var filePath = Path.Combine(uploadDir, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                        await file.CopyToAsync(stream);

                    _context.MovieImages.Add(new MovieImage
                    {
                        MovieId = existing.Id,
                        ImageUrl = $"/images/movies/{fileName}"
                    });
                }
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "🎞 Movie updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // ✅ DELETE GET
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var movie = await _context.Movies
                .Include(m => m.Cinema)
                .Include(m => m.Category)
                .Include(m => m.MovieImages)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null) return NotFound();

            return View(movie);
        }

        // ✅ DELETE POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies
                .Include(m => m.MovieImages)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie != null)
            {
                foreach (var img in movie.MovieImages)
                {
                    var imgPath = Path.Combine(_env.WebRootPath, img.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imgPath))
                        System.IO.File.Delete(imgPath);
                }

                _context.Movies.Remove(movie);
                await _context.SaveChangesAsync();
            }

            TempData["Success"] = "🗑 Movie deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        private void LoadDropdownLists(Movie? movie = null)
        {
            ViewBag.CinemaId = new SelectList(_context.Cinemas, "Id", "Name", movie?.CinemaId);
            ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name", movie?.CategoryId);
            ViewBag.Actors = new MultiSelectList(_context.Actors, "Id", "Name", movie?.SelectedActorsIds);
        }

    }
}
