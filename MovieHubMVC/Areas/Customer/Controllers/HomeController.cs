using System.Diagnostics;
using MovieHubMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MovieHubMVC.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        public ViewResult Index()
        {
            return View();
        }

    }
}
