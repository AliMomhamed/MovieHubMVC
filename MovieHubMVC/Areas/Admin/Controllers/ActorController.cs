using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieHubMVC.Data;
using MovieHubMVC.Models;

namespace MovieHubMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ActorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ActorController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // Index
        public async Task<IActionResult> Index()
        {
            var actors = await _context.Actors.Include(a => a.Movies).ToListAsync();
            return View(actors); // ✅ List<Actor>
        }


        // Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var actor = await _context.Actors
                .Include(a => a.Movies)
                .FirstOrDefaultAsync(a => a.Id == id);
            if (actor == null) return NotFound();
            return View(actor);
        }

        // Create GET
        public IActionResult Create()
        {
            return View();
        }

        // Create POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Actor actor, IFormFile? ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var path = await SaveFile(ImageFile, "actors");
                    actor.ImageUrl = path;
                }


                _context.Add(actor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

           
            return View(actor);
        }

        // Edit GET
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var actor = await _context.Actors.Include(a => a.Movies).FirstOrDefaultAsync(a => a.Id == id);
            if (actor == null) return NotFound();
         
            return View(actor);
        }

        // Edit POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Actor actor, IFormFile? ImageFile)
        {
            if (id != actor.Id) return NotFound();

            var existing = await _context.Actors.Include(a => a.Movies).FirstOrDefaultAsync(a => a.Id == id);
            if (existing == null) return NotFound();

            if (ModelState.IsValid)
            {
                existing.Name = actor.Name;
                existing.Bio = actor.Bio;

                // image
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var newPath = await SaveFile(ImageFile, "actors");
                    // delete old
                    if (!string.IsNullOrEmpty(existing.ImageUrl))
                    {
                        DeleteFile(existing.ImageUrl);
                    }
                    existing.ImageUrl = newPath;
                }


                _context.Update(existing);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }


            return View(actor);
        }

        // Delete GET
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var actor = await _context.Actors.Include(a => a.Movies).FirstOrDefaultAsync(a => a.Id == id);
            if (actor == null) return NotFound();
            return View(actor);
        }

        // Delete POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var actor = await _context.Actors.FindAsync(id);
            if (actor != null)
            {
                if (!string.IsNullOrEmpty(actor.ImageUrl))
                    DeleteFile(actor.ImageUrl);

                _context.Actors.Remove(actor);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // helper save/delete
        private async Task<string> SaveFile(IFormFile file, string folder)
        {
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var ext = Path.GetExtension(file.FileName).ToLower();
            if (!allowed.Contains(ext)) throw new InvalidOperationException("Invalid image format.");

            var uploadDir = Path.Combine(_env.WebRootPath, "uploads", folder);
            Directory.CreateDirectory(uploadDir);

            var fileName = Guid.NewGuid().ToString() + ext;
            var filePath = Path.Combine(uploadDir, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
                await file.CopyToAsync(stream);

            return $"/uploads/{folder}/{fileName}";
        }

        private void DeleteFile(string virtualPath)
        {
            var physical = Path.Combine(_env.WebRootPath, virtualPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (System.IO.File.Exists(physical))
                System.IO.File.Delete(physical);
        }
    }
}
