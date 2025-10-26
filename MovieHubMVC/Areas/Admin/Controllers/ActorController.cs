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
        public override async Task<IActionResult> Create(Actor actor, CancellationToken token)
        {
            if (!ModelState.IsValid) return View(actor);

            if (actor.ImageFile != null)
                actor.ImageUrl = await SaveFileAsync(actor.ImageFile, "actors", token);

            _dbSet.Add(actor);
            await _context.SaveChangesAsync(token);
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
