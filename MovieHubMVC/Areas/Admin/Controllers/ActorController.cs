using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieHubMVC.Data;
using MovieHubMVC.Models;

namespace MovieHubMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]/[action]")]
    public class ActorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ActorController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ✅ INDEX
        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken token)
        {
            var actors = await _context.Actors
                .Include(a => a.Movies)
                .ToListAsync(token);

            return View(actors);
        }

        // ✅ DETAILS
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Details(int id, CancellationToken token)
        {
            var actor = await _context.Actors
                .Include(a => a.Movies)
                .FirstOrDefaultAsync(a => a.Id == id, token);

            if (actor == null)
                return NotFound();

            return View(actor);
        }

        // ✅ CREATE (GET)
        [HttpGet]
        public IActionResult Create() => View();

        // ✅ CREATE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Actor actor, IFormFile? imageFile, CancellationToken token)
        {
            if (!ModelState.IsValid)
                return View(actor);

            if (imageFile != null && imageFile.Length > 0)
                actor.ImageUrl = await SaveFileAsync(imageFile, "actors", token);

            _context.Actors.Add(actor);
            await _context.SaveChangesAsync(token);

            return RedirectToAction(nameof(Index));
        }

        // ✅ EDIT (GET)
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Edit(int id, CancellationToken token)
        {
            var actor = await _context.Actors.FindAsync(new object?[] { id }, token);
            if (actor == null)
                return NotFound();

            return View(actor);
        }

        // ✅ EDIT (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Actor updatedActor, IFormFile? imageFile, CancellationToken token)
        {
            var actor = await _context.Actors.FindAsync(new object?[] { id }, token);
            if (actor == null)
                return NotFound();

            if (!ModelState.IsValid)
                return View(updatedActor);

            actor.Name = updatedActor.Name;
            actor.Bio = updatedActor.Bio;

            if (imageFile != null && imageFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(actor.ImageUrl))
                    DeleteFile(actor.ImageUrl);

                actor.ImageUrl = await SaveFileAsync(imageFile, "actors", token);
            }

            _context.Update(actor);
            await _context.SaveChangesAsync(token);

            return RedirectToAction(nameof(Index));
        }

        // ✅ DELETE (GET)
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken token)
        {
            var actor = await _context.Actors
                .Include(a => a.Movies)
                .FirstOrDefaultAsync(a => a.Id == id, token);

            if (actor == null)
                return NotFound();

            return View(actor);
        }

        // ✅ DELETE (POST)
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken token)
        {
            var actor = await _context.Actors.FindAsync(new object?[] { id }, token);
            if (actor == null)
                return NotFound();

            if (!string.IsNullOrEmpty(actor.ImageUrl))
                DeleteFile(actor.ImageUrl);

            _context.Actors.Remove(actor);
            await _context.SaveChangesAsync(token);

            return RedirectToAction(nameof(Index));
        }

        // ✅ SAVE FILE
        private async Task<string> SaveFileAsync(IFormFile file, string folder, CancellationToken token)
        {
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowed.Contains(ext))
                throw new InvalidOperationException("Invalid image format.");

            var uploadDir = Path.Combine(_env.WebRootPath, "uploads", folder);
            Directory.CreateDirectory(uploadDir);

            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadDir, fileName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream, token);

            return $"/uploads/{folder}/{fileName}";
        }

        // ✅ DELETE FILE
        private void DeleteFile(string virtualPath)
        {
            var physicalPath = Path.Combine(_env.WebRootPath, virtualPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (System.IO.File.Exists(physicalPath))
                System.IO.File.Delete(physicalPath);
        }
    }
}
