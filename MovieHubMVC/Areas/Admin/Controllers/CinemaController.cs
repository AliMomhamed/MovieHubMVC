using Microsoft.AspNetCore.Mvc;
using MovieHubMVC.Data;
using MovieHubMVC.Models;

namespace MovieHubMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CinemaController : BaseAdminController<Cinema>
    {
        public CinemaController(ApplicationDbContext context, IWebHostEnvironment env)
            : base(context, env) { }
    }
}
