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
<<<<<<< HEAD
        public async Task<IActionResult> Create(Actor actor, IFormFile? imageFile, CancellationToken token)
=======
<<<<<<< HEAD
        public override async Task<IActionResult> Create(Actor actor, CancellationToken token)
        {
            if (!ModelState.IsValid) return View(actor);
=======
<<<<<<< HEAD
        public async Task<IActionResult> Create(Actor actor, IFormFile? ImageFile)
=======
        public async Task<IActionResult> Create(Actor actor, int[] selectedMovies, IFormFile? ImageFile)
>>>>>>> c96d00b87659b9e5d31057d644ee82e4190593ad
>>>>>>> 646407670c8dfea6397340b330a92f8260744824
        {
            if (!ModelState.IsValid)
                return View(actor);

<<<<<<< HEAD
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

=======
=======
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "actors");
>>>>>>> e133ec3780a4770ca7812d7d77ac7058b01f79b9

            if (actor.ImageFile != null)
                actor.ImageUrl = await SaveFileAsync(actor.ImageFile, "actors", token);

<<<<<<< HEAD
            _dbSet.Add(actor);
            await _context.SaveChangesAsync(token);
=======
                    var filePath = Path.Combine(uploadPath, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }

                    actor.ImageUrl = "/images/actors/" + fileName;
                }

                actor.Movies = _context.Movies
                    .Where(m => selectedMovies.Contains(m.Id))
                    .ToList();
>>>>>>> c96d00b87659b9e5d31057d644ee82e4190593ad

                _context.Add(actor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

           
>>>>>>> 646407670c8dfea6397340b330a92f8260744824
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
<<<<<<< HEAD

            _context.Update(actor);
            await _context.SaveChangesAsync(token);

=======
>>>>>>> e133ec3780a4770ca7812d7d77ac7058b01f79b9
>>>>>>> 646407670c8dfea6397340b330a92f8260744824
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
