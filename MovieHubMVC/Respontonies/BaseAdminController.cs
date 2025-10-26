using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieHubMVC.Data;

namespace MovieHubMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public abstract class BaseAdminController<TEntity> : Controller where TEntity : class, new()
    {
        protected readonly ApplicationDbContext _context;
        protected readonly IWebHostEnvironment _env;
        protected readonly DbSet<TEntity> _dbSet;

        protected BaseAdminController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
            _dbSet = _context.Set<TEntity>();
        }

        // INDEX
        public virtual async Task<IActionResult> Index(CancellationToken token)
        {
            var list = await _dbSet.ToListAsync(token);
            return View(list);
        }

        // DETAILS
        public virtual async Task<IActionResult> Details(int? id, CancellationToken token)
        {
            if (id == null) return NotFound();

            var entity = await _dbSet.FindAsync([id], token);
            if (entity == null) return NotFound();

            return View(entity);
        }

        // CREATE - GET
        public virtual IActionResult Create() => View();

        // CREATE - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Create(TEntity entity, CancellationToken token)
        {
            if (!ModelState.IsValid) return View(entity);

            _dbSet.Add(entity);
            await _context.SaveChangesAsync(token);
            return RedirectToAction(nameof(Index));
        }

        // EDIT - GET
        public virtual async Task<IActionResult> Edit(int? id, CancellationToken token)
        {
            if (id == null) return NotFound();

            var entity = await _dbSet.FindAsync([id], token);
            if (entity == null) return NotFound();

            return View(entity);
        }

        // EDIT - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Edit(int id, TEntity entity, CancellationToken token)
        {
            if (id == 0) return NotFound();

            if (!ModelState.IsValid) return View(entity);

            _context.Update(entity);
            await _context.SaveChangesAsync(token);

            return RedirectToAction(nameof(Index));
        }

        // DELETE - GET
        public virtual async Task<IActionResult> Delete(int? id, CancellationToken token)
        {
            if (id == null) return NotFound();

            var entity = await _dbSet.FindAsync([id], token);
            if (entity == null) return NotFound();

            return View(entity);
        }

        // DELETE - POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> DeleteConfirmed(int id, CancellationToken token)
        {
            var entity = await _dbSet.FindAsync([id], token);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync(token);
            }
            return RedirectToAction(nameof(Index));
        }

        // FILE HANDLING 
        protected async Task<string> SaveFileAsync(IFormFile file, string folder, CancellationToken token = default)
        {
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var ext = Path.GetExtension(file.FileName).ToLower();

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

        protected void DeleteFile(string? virtualPath)
        {
            if (string.IsNullOrEmpty(virtualPath)) return;

            var fullPath = Path.Combine(_env.WebRootPath, virtualPath.TrimStart('/'));
            if (System.IO.File.Exists(fullPath))
                System.IO.File.Delete(fullPath);
        }
    }
}
