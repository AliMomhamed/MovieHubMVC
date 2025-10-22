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

        public ActorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Actor
        public async Task<IActionResult> Index()
        {
            var actors = await _context.Actors
                .Include(a => a.Movies)
                .ToListAsync();

            return View(actors);
        }

        // GET: Admin/Actor/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var actor = await _context.Actors
                .Include(a => a.Movies)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (actor == null) return NotFound();

            return View(actor);
        }

        // GET: Admin/Actor/Create
        public IActionResult Create()
        {
            ViewBag.Movies = new MultiSelectList(_context.Movies, "Id", "Name");
            return View();
        }

        // POST: Admin/Actor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Actor actor, int[] selectedMovies, IFormFile? ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "actors");

                    if (!Directory.Exists(uploadPath))
                        Directory.CreateDirectory(uploadPath);

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

                _context.Add(actor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Movies = new MultiSelectList(_context.Movies, "Id", "Name", selectedMovies);
            return View(actor);
        }

        // GET: Admin/Actor/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var actor = await _context.Actors
                .Include(a => a.Movies)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (actor == null) return NotFound();

            ViewBag.Movies = new MultiSelectList(
                _context.Movies, "Id", "Name", actor.Movies.Select(m => m.Id)
            );

            return View(actor);
        }

        // POST: Admin/Actor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Actor actor, int[] selectedMovies, IFormFile? ImageFile)
        {
            if (id != actor.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var existingActor = await _context.Actors
                    .Include(a => a.Movies)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (existingActor == null) return NotFound();

                existingActor.Name = actor.Name;
                existingActor.Bio = actor.Bio;
                existingActor.Movies.Clear();
                existingActor.Movies = _context.Movies
                    .Where(m => selectedMovies.Contains(m.Id))
                    .ToList();

                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "actors");

                    if (!Directory.Exists(uploadPath))
                        Directory.CreateDirectory(uploadPath);

                    var filePath = Path.Combine(uploadPath, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }

                    if (!string.IsNullOrEmpty(existingActor.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingActor.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                            System.IO.File.Delete(oldImagePath);
                    }

                    existingActor.ImageUrl = "/images/actors/" + fileName;
                }

                _context.Update(existingActor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Movies = new MultiSelectList(
                _context.Movies, "Id", "Name", selectedMovies
            );

            return View(actor);
        }

     
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var actor = await _context.Actors
                .Include(a => a.Movies)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (actor == null) return NotFound();

            return View(actor);
        }

        // POST: Admin/Actor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var actor = await _context.Actors.FindAsync(id);
            if (actor != null)
            {
             
                if (!string.IsNullOrEmpty(actor.ImageUrl))
                {
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", actor.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                        System.IO.File.Delete(imagePath);
                }

                _context.Actors.Remove(actor);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
