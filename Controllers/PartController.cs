using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using WarsztatMVC.DAL;
using WarsztatMVC.Models;
using System.Diagnostics;

namespace WarsztatMVC.Controllers
{
    public class PartController : Controller
    {
        private readonly getData _getData;
        private readonly string? _connectionString;

        public PartController(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionString")["Polaczenie"];
            _getData = new getData(_connectionString);
        }
        public ActionResult Index()
        {
            if (Request.Cookies["CookieIsAdmin"] == "True")
            {
                var parts = _getData.getParts();

                if (TempData.ContainsKey("PartSuccess"))
                {
                    ViewBag.PartSuccess = TempData["PartSuccess"];
                }

                if (TempData.ContainsKey("PartError"))
                {
                    ViewBag.PartError = TempData["PartError"];
                }

                return View(parts);
            }
            else
            {
                return RedirectToAction("Index", "WorkerLogin");
            }
        }

        // GET: PartController/Create
        public ActionResult Create()
        {
            if (Request.Cookies["CookieIsAdmin"] == "True")
            {
                ViewBag.CzynnosciList = _getData.getActivityNamesOnly();
                ViewBag.ProducentList = _getData.getProducents();

                return View();
            }
            else
            {
                return RedirectToAction("Index", "WorkerLogin");
            }
        }

        // POST: PartController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PartModel part)
        {
            if (Request.Cookies["CookieIsAdmin"] == "True")
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        string errorMessage;
                        if (addPart(part, out errorMessage))
                        {
                            TempData["PartSuccess"] = "Część została dodana pomyślnie.";
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            TempData["PartError"] = "Dodanie części nie powiodło się. Spróbuj ponownie. " + errorMessage;
                            return RedirectToAction("Index");
                        }
                    }

                    return View(part);
                }
                catch (Exception ex)
                {
                    TempData["PartError"] = "Wystąpił błąd: " + ex.Message;
                    return View();
                }

            }
            else
            {
                return RedirectToAction("Index", "WorkerLogin");
            }
        }

        // GET: PartController/Edit/5
        public ActionResult Edit(int id)
        {
            if (Request.Cookies["CookieIsAdmin"] == "True")
            {
                if (id <= 0)
                {
                    TempData["PartError"] = "ID części musi być większe niż 0.";
                    return RedirectToAction("Index");
                }

                ViewBag.CzynnosciList = _getData.getActivityNamesOnly();
                ViewBag.ProducentList = _getData.getProducents();
                PartModel part = _getData.getPart(id);
                if (part == null)
                {
                    TempData["PartError"] = "Część o takim id nie istnieje.";
                    return RedirectToAction("Index");
                }
                return View(part);
            }
            else
            {
                return RedirectToAction("Index", "WorkerLogin");
            }
        }

        // POST: PartController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PartModel part)
        {
            if (Request.Cookies["CookieIsAdmin"] == "True")
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        string errorMessage;
                        if (updatePart(part, out errorMessage))
                        {
                            TempData["PartSuccess"] = "Część została edytowana pomyślnie.";
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            TempData["PartError"] = "Edycja części nie powiodło się. Spróbuj ponownie. " + errorMessage; ;
                            return RedirectToAction("Index");
                        }

                    }
                    return View(part);
                }


                catch (Exception ex)
                {
                    TempData["PartError"] = "Wystąpił błąd: " + ex.Message;
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Index", "WorkerLogin");
            }
        }

        // GET: PartController/Delete/5
        public ActionResult Delete(int id)
        {
            if (Request.Cookies["CookieIsAdmin"] == "True")
            {
                if (id <= 0)
                {
                    TempData["PartError"] = "ID części musi być większe niż 0.";
                    return RedirectToAction("Index");
                }
                PartModel part = _getData.getPart(id);
                if (part == null)
                {
                    TempData["PartError"] = "Część o takim id nie istnieje.";
                    return RedirectToAction("Index");
                }
                return View(part);
            }
            else
            {
                return RedirectToAction("Index", "WorkerLogin");
            }
        }

        // POST: PartController/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult deleteConfirmation(int id)
        {
            if (Request.Cookies["CookieIsAdmin"] == "True")
            {
                string result = removePart(id);
                if (result != null)
                {
                    TempData["PartSuccess"] = "Część została usunięta";
                    return RedirectToAction("Index");
                }
                else
                {

                    TempData["PartError"] = "Błąd";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("Index", "WorkerLogin");
            }
        }


        public bool addPart(PartModel part, out string errorMessage)
        {
            errorMessage = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("dodajCzesc", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@Czynnosc", (object)part.Czynnosc ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Nazwa", part.Czesc);
                        command.Parameters.AddWithValue("@ProducentNazwa", (object)part.Producent ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Nr_czesc", (object)part.Nr_czesc ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Cena_netto", (object)part.Cena_netto ?? DBNull.Value);

                        int rowsAffected = command.ExecuteNonQuery();

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

        public bool updatePart(PartModel part, out string errorMessage)
        {
            errorMessage = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("edytujCzesc", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@Id_czesc", part.Id_czesc);
                        command.Parameters.AddWithValue("@Czynnosc", (object)part.Czynnosc ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Nazwa", part.Czesc);
                        command.Parameters.AddWithValue("@ProducentNazwa", (object)part.Producent ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Nr_czesc", (object)part.Nr_czesc ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Cena_netto", (object)part.Cena_netto ?? DBNull.Value);

                        int rowsAffected = command.ExecuteNonQuery();

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

        public string removePart(int id)
        {
            string result = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("usunCzesc", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id_czesc", id);

                command.ExecuteNonQuery();

                result = $"Cześć o {id} została usunięta";

                connection.Close();

                return result;
            }
        }
    }
}
