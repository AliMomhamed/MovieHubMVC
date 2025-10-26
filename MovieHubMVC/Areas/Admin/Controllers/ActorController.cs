using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieHubMVC.Data;
using MovieHubMVC.Models;

namespace MovieHubMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ActorController : BaseAdminController<Actor>
    {
        public ActorController(ApplicationDbContext context, IWebHostEnvironment env)
            : base(context, env) { }

        public override async Task<IActionResult> Index(CancellationToken token)
        {
            var actors = await _context.Actors.Include(a => a.Movies).ToListAsync(token);
            return View(actors);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
<<<<<<< HEAD
                    var path = await SaveFile(ImageFile, "actors");
                    actor.ImageUrl = path;
                }

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
<<<<<<< HEAD
        public async Task<IActionResult> Edit(int id, Actor actor, IFormFile? ImageFile)
=======
        public async Task<IActionResult> Edit(int id, Actor actor, int[] selectedMovies, IFormFile? ImageFile)
>>>>>>> c96d00b87659b9e5d31057d644ee82e4190593ad
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

<<<<<<< HEAD

                _context.Update(existing);
=======
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
>>>>>>> c96d00b87659b9e5d31057d644ee82e4190593ad
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }


            return View(actor);
        }

<<<<<<< HEAD
        // Delete GET
=======
     
>>>>>>> c96d00b87659b9e5d31057d644ee82e4190593ad
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
<<<<<<< HEAD
                if (!string.IsNullOrEmpty(actor.ImageUrl))
                    DeleteFile(actor.ImageUrl);
=======
             
                if (!string.IsNullOrEmpty(actor.ImageUrl))
                {
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", actor.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                        System.IO.File.Delete(imagePath);
                }
>>>>>>> c96d00b87659b9e5d31057d644ee82e4190593ad

                _context.Actors.Remove(actor);
                await _context.SaveChangesAsync();
            }
>>>>>>> e133ec3780a4770ca7812d7d77ac7058b01f79b9
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override async Task<IActionResult> Edit(int id, Actor actor, CancellationToken token)
        {
            if (id != actor.Id) return NotFound();
            var existing = await _dbSet.FindAsync([id], token);
            if (existing == null) return NotFound();

            existing.Name = actor.Name;
            existing.Bio = actor.Bio;

            if (actor.ImageFile != null)
            {
                DeleteFile(existing.ImageUrl);
                existing.ImageUrl = await SaveFileAsync(actor.ImageFile, "actors", token);
            }

            _context.Update(existing);
            await _context.SaveChangesAsync(token);
            return RedirectToAction(nameof(Index));
        }
    }
}
