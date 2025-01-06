using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WarsztatMVC.Models;
using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using WarsztatMVC.DAL;

namespace WarsztatMVC.Controllers
{

    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<HomeController> _logger;
        private readonly getData _getData;
        private readonly string? _connectionString;

        public HomeController(IConfiguration configuration, ILogger<HomeController> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _connectionString = _configuration.GetSection("ConnectionString")["Polaczenie"];
            _getData = new getData(_connectionString);
        }

        List<VisitModel> visits = new List<VisitModel>();

        public ActionResult Index()
        {

            //if (User.Identity.IsAuthenticated)
            if (Request.Cookies["CookieUserID"] != null)
            {
                visits = _getData.getVisits();

                return View(visits);
            }
            else
            {
                return RedirectToAction("Index", "WorkerLogin");
            }


        }

        public IActionResult Privacy()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



    }
}