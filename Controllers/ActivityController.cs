using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Data.SqlClient;
using System.Data;
using WarsztatMVC.DAL;
using WarsztatMVC.Models;
using System.Diagnostics;
using System.Drawing;


namespace WarsztatMVC.Controllers
{
    public class ActivityController : Controller
    {
        private readonly getData _getData;
        private readonly string? _connectionString;

        public ActivityController(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionString")["Polaczenie"];
            _getData = new getData(_connectionString);
        }

        // GET: ActionController
        public ActionResult Index()
        {
            if (Request.Cookies["CookieIsAdmin"] == "True")
            {
                var actions = _getData.getActivities();

                if (TempData.ContainsKey("ActivitySuccess"))
                {
                    ViewBag.ActivitySuccess = TempData["ActivitySuccess"];
                }

                if (TempData.ContainsKey("ActivityError"))
                {
                    ViewBag.ActivityError = TempData["ActivityError"];
                }
                return View(actions);
            }
            else
            {
                return RedirectToAction("Index", "WorkerLogin");
            }
        }

        // GET: ActionController/Create
        public ActionResult Create()
        {
            if (Request.Cookies["CookieIsAdmin"] == "True")
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "WorkerLogin");
            }
        }

        // POST: ActionController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ActivityModel activity)
        {
            if (Request.Cookies["CookieIsAdmin"] == "True")
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        string errorMessage;
                        if (addAction(activity, out errorMessage))
                        {
                            TempData["ActivitySuccess"] = "Czynność została dodana pomyślnie.";
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            TempData["ActivityError"] = "Dodanie czynności nie powiodło się. Spróbuj ponownie. " + errorMessage;
                            return RedirectToAction("Index");
                        }
                    }

                    return View(activity);
                }
                catch (Exception ex)
                {
                    TempData["ActivityError"] = "Wystąpił błąd: " + ex.Message;
                    return View();
                }

            }
            else
            {
                return RedirectToAction("Index", "WorkerLogin");
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (Request.Cookies["CookieIsAdmin"] == "True")
            {
                if (id <= 0)
                {
                    TempData["ActivityError"] = "ID czynnosci musi być większe niż 0.";
                    return RedirectToAction("Index");
                }
                var action = _getData.getActivity(id);
                if (action == null)
                {
                    TempData["ActivityError"] = "Czynność o takim id nie istnieje.";
                    return RedirectToAction("Index");
                }

                // Przekazanie modelu do widoku
                return View(action);
            }
            else
            {
                return RedirectToAction("Index", "WorkerLogin");
            }
        }

        // POST: ActionController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ActivityModel activity)
        {
            if (Request.Cookies["CookieIsAdmin"] == "True")
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        string errorMessage;
                        if (updateAction(activity, out errorMessage))
                        {
                            TempData["ActivitySuccess"] = "Czynność została edytowana pomyślnie.";
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            TempData["ActivityError"] = "Edycja czynnosci nie powiodła się. Spróbuj ponownie. " + errorMessage; ;
                            return View(activity);
                        }
                    }
                    else
                    {
                        return View(activity);  
                    }


                }
                catch (Exception ex)
                {
                    TempData["ActivityError"] = "Wystąpił błąd: " + ex.Message;
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Index", "WorkerLogin");
            }
        }

        public ActionResult Delete(int id)
        {
            if (Request.Cookies["CookieIsAdmin"] == "True")
            {
                if (id <= 0)
                {
                    TempData["ActivityError"] = "ID czynnosci musi być większe niż 0.";
                    return RedirectToAction("Index");
                }
                var action = _getData.getActivity(id);
                if (action == null)
                {
                    TempData["ActivityError"] = "Czynność o takim id nie istnieje.";
                    return RedirectToAction("Index");
                }

                // Przekazanie modelu do widoku
                return View(action);
            }
            else
            {
                return RedirectToAction("Index", "WorkerLogin");
            }
        }

        // POST: ActionController/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult deleteConfirmation(int id)
        {
            if (Request.Cookies["CookieIsAdmin"] == "True")
            {
                string result = removeAction(id);
                if (result != null)
                {
                    TempData["ActivitySuccess"] = "Czynność została usunięta";
                    return RedirectToAction("Index");
                }
                else
                {

                    TempData["ActivityError"] = "Błąd";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("Index", "WorkerLogin");
            }
        }

        public bool addAction(ActivityModel activity, out string errorMessage)
        {
            errorMessage = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "dodajCzynnosc";  
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@Nazwa", activity.Nazwa);
                        command.Parameters.AddWithValue("@Cena_netto", activity.Cena_netto);
                        command.Parameters.AddWithValue("@Roboczogodziny", activity.Roboczogodziny);

                        int rowsAffected = command.ExecuteNonQuery();

                        connection.Close();

                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public bool updateAction(ActivityModel activity, out string errorMessage)
        {
            errorMessage = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "edytujCzynnosc";  
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@Id", activity.Id_czynnosc);
                        command.Parameters.AddWithValue("@Nazwa", activity.Nazwa);
                        command.Parameters.AddWithValue("@Cena_netto", activity.Cena_netto);
                        command.Parameters.AddWithValue("@Roboczogodziny", activity.Roboczogodziny);

                        int rowsAffected = command.ExecuteNonQuery();

                        connection.Close();

                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public string removeAction(int id)
        {
            string result = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("usunCzynnosc", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id_czynnosc", id);

                command.ExecuteNonQuery();

                result = $"Czynność o {id} została usunięta";

                connection.Close();

                return result;
            }
        }

    }
}
