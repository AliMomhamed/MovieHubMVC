using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieHubMVC.Data;
using MovieHubMVC.Models;

namespace MovieHubMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CinemaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public CinemaController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _context.Cinemas.Include(c => c.Movies).ToListAsync();
            return View(list);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var cinema = await _context.Cinemas
                .Include(c => c.Movies)
                .ThenInclude(m => m.Category)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (cinema == null) return NotFound();
            return View(cinema);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cinema cinema)
        {
            if (ModelState.IsValid)
            {
                // ✅ معالجة الصورة المرفوعة
                if (cinema.ImageFile != null)
                {
                    var uploadDir = Path.Combine(_env.WebRootPath, "images/cinemas");
                    Directory.CreateDirectory(uploadDir);

                    var uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(cinema.ImageFile.FileName);
                    var filePath = Path.Combine(uploadDir, uniqueName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await cinema.ImageFile.CopyToAsync(stream);
                    }

                    cinema.ImageUrl = "/images/cinemas/" + uniqueName;
                }

                _context.Add(cinema);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(cinema);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var cinema = await _context.Cinemas.FindAsync(id);
            if (cinema == null) return NotFound();
            return View(cinema);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Cinema cinema)
        {
            if (id != cinema.Id) return NotFound();

            var existing = await _context.Cinemas.FindAsync(id);
            if (existing == null) return NotFound();

            if (ModelState.IsValid)
            {
                existing.Name = cinema.Name;
                existing.Description = cinema.Description;

                // ✅ تحديث الصورة لو المستخدم رفع واحدة جديدة
                if (cinema.ImageFile != null)
                {
                    var uploadDir = Path.Combine(_env.WebRootPath, "images/cinemas");
                    Directory.CreateDirectory(uploadDir);

                    // حذف الصورة القديمة لو موجودة
                    if (!string.IsNullOrEmpty(existing.ImageUrl))
                    {
                        var oldPath = Path.Combine(_env.WebRootPath, existing.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldPath))
                            System.IO.File.Delete(oldPath);
                    }

                    var uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(cinema.ImageFile.FileName);
                    var filePath = Path.Combine(uploadDir, uniqueName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await cinema.ImageFile.CopyToAsync(stream);
                    }

                    existing.ImageUrl = "/images/cinemas/" + uniqueName;
                }

                _context.Update(existing);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(cinema);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var cinema = await _context.Cinemas.FindAsync(id);
            if (cinema == null) return NotFound();
            return View(cinema);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cinema = await _context.Cinemas.FindAsync(id);
            if (cinema != null)
            {
                if (!string.IsNullOrEmpty(cinema.ImageUrl))
                {
                    var path = Path.Combine(_env.WebRootPath, cinema.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);
                }

                _context.Cinemas.Remove(cinema);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
