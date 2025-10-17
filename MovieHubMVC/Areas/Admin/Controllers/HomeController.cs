using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MovieHubMVC.Models;

namespace MovieHubMVC.Areas.Admin.Controllers
{

    [Area("Admin")]
    public class HomeController : Controller
    {
        public ViewResult Index()
        {
            return View();
        }

    }
}
