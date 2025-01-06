using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WarsztatMVC.Models;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Data;
using System.Diagnostics;
using System.Net;
using WarsztatMVC.DAL;


namespace WarsztatMVC.Controllers
{
    public class RepairController : Controller
    {

        private readonly getData _getData;
        private readonly string? _connectionString;

        public RepairController(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionString")["Polaczenie"];
            _getData = new getData(_connectionString);
        }

        List<ActivityModel> actions = new List<ActivityModel>();
        List<PartModel> parts = new List<PartModel>();
        List<VisitModel> visits = new List<VisitModel>();
        List<FixModel> fixes = new List<FixModel>();


        [HttpGet]
        public ActionResult Index()
        {

            if (Request.Cookies["CookieUserID"] != null)
            {
                fixes = _getData.getFixes();

                RepairModel objRepairViewModel = new RepairModel();
                objRepairViewModel.FixViewModel = fixes;

                if (TempData.ContainsKey("RepairSuccess"))
                {
                    ViewBag.RepairSucces = TempData["RepairSuccess"];
                }
                if (TempData.ContainsKey("RepairError"))
                {
                    ViewBag.RepairError = TempData["RepairError"];
                }

                return View(objRepairViewModel);
            }
            else
            {
                return RedirectToAction("Index", "WorkerLogin");
            }
            
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {

            if (Request.Cookies["CookieUserID"] != null)
            {
                actions = _getData.getActivities();
                parts = _getData.getParts();
                visits = _getData.getVisits();
                fixes = _getData.getFixById(id);


                RepairModel objRepairViewMOdel = new RepairModel
                {
                    ActionViewModel = actions,
                    PartViewModel = parts,
                    VisitViewModel = visits,
                    FixViewModel = fixes
                };
                if (fixes == null || fixes.Count == 0)
                {
                    TempData["RepairError"] = "Naprawa o takim id nie istenieje";
                    return RedirectToAction("Index");

                }
                return View(objRepairViewMOdel);
            }
            else
            {
                return RedirectToAction("Index", "WorkerLogin");
            }

            

        }
        [HttpPost]
        public ActionResult Edit(FixModel fixId)
        {

            try
            {
                if(ModelState.IsValid)
                {
                    if(updateFix(fixId))
                    {

                        TempData["RepairSuccess"] = "Edycja naprawy się powiodła";

                        return RedirectToAction("Index", "Appointment");
                    }
                    else
                    {
                        TempData["RepairError"] = "Nie udało się edytować naprawy.";
                        return RedirectToAction("Index", "Appointment");
                    }
                }
                else
                {
                    TempData["RepairError"] = "Nie udało się edytować naprawy bardzo.";
                    return RedirectToAction("Index", "Appointment");
                }
                
                //return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["RepairError"] = "Wystąpił błąd: " + ex.Message;
                Debug.WriteLine(ex.Message);
                return View();
            }

            //return View();
        }

        [HttpGet]
        public ActionResult Create(int id)
        {
            if (Request.Cookies["CookieUserID"] != null)
            {
                if (id == null)
                {
                    return View();
                }
                else
                {

                }
                actions = _getData.getActivities();
                parts = _getData.getParts();
                visits = _getData.getVisits();
                fixes = _getData.getFixes();


                RepairModel objRepairViewMOdel = new RepairModel
                {
                    ActionViewModel = actions,
                    PartViewModel = parts,
                    VisitViewModel = visits,
                    FixViewModel = new List<FixModel> { new FixModel() }
                };

                return View(objRepairViewMOdel);
            }
            else
            {
                return RedirectToAction("Index", "WorkerLogin");
            }

            
        }

        // POST: RepairController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FixModel fix, int id)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    if(addFix(fix, id))
                    {
                        TempData["RepairSuccess"] = "Naprawa została dodana pomyślnie";

                        return RedirectToAction("Index", "Appointment");
                    }
                    else
                    {
                        TempData["RepairError"] = "Nie udało się dodać naprawy.";
                        return RedirectToAction("Index", "Appointment");
                    }
                }
                else
                {

                    
                    TempData["RepairError"] = "Nie udało się dodać naprawy.";
                    return RedirectToAction("Index", "Appointment");
                }

            }
            catch (Exception ex)
            {
                TempData["RepairError"] = "Wystąpił błąd: " + ex.Message;
                Debug.WriteLine(ex.Message);
                return RedirectToAction("Index", "Appointment");
            }
        }

        

        public bool addFix(FixModel fix, int id)
        {
            try
            {
                using(SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using(SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "dodajNaprawa";

                        int id_pracownik = Convert.ToInt32(Request.Cookies["CookieUserID"]);
                        Debug.WriteLine("Id pracownik " + id_pracownik);
                        command.Parameters.AddWithValue("@Id_czynnosc", fix.Id_czynnosc);
                        command.Parameters.AddWithValue("@Id_wizyta", id); //pobierane to jest z linku czy z czegoś tam
                        command.Parameters.AddWithValue("@Id_czesc", fix.Id_czesc);
                        command.Parameters.AddWithValue("@Id_pracownik", id_pracownik);
                        command.Parameters.AddWithValue("@Uwagi", fix.Uwagi);

                        int rowsAffected = command.ExecuteNonQuery();

                        connection.Close();

                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex) 
            {
                Debug.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        public bool updateFix(FixModel fix)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "edytujNaprawa";

                        int id_pracownik = Convert.ToInt32(Request.Cookies["CookieUserID"]);
                        Debug.WriteLine("Id pracownik " +  id_pracownik);
                        command.Parameters.AddWithValue("@Id_czynnosc", fix.Id_czynnosc);
                        command.Parameters.AddWithValue("@Id_naprawa", fix.Id);
                        command.Parameters.AddWithValue("@Id_czesc", fix.Id_czesc);
                        command.Parameters.AddWithValue("@Id_pracownik", id_pracownik);
                        command.Parameters.AddWithValue("@Uwagi", fix.Uwagi);

                        int rowsAffected = command.ExecuteNonQuery();

                        connection.Close();

                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

    }
}
