using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using WarsztatMVC.Models;
using WarsztatMVC.DAL;
using System.Security.Cryptography;
using System.Text;
using System.Linq.Expressions;
using System.Diagnostics;

namespace WarsztatMVC.Controllers
{
    public class WorkerController : Controller
    {

        private readonly getData _getData;
        private readonly string? _connectionString;

        public WorkerController(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionString")["Polaczenie"];
            _getData = new getData(_connectionString);
        }
        // GET: WorkerController
        public ActionResult Index()
        {
            if (Request.Cookies["CookieIsAdmin"] == "True")
            {
                var workers = _getData.getWorkers();

                if (TempData.ContainsKey("WorkerSuccess"))
                {
                    ViewBag.WorkerSuccess = TempData["WorkerSuccess"];
                }

                if (TempData.ContainsKey("WorkerError"))
                {
                    ViewBag.WorkerError = TempData["WorkerError"];
                }

                return View(workers);
            }

            else
            {
                return RedirectToAction("Index", "WorkerLogin");
            }



        }


        // GET: WorkerController/Create
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

        // POST: WorkerController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(WorkerModel worker)
        {
            if (Request.Cookies["CookieIsAdmin"] == "True")
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        string errorMessage;
                        if (addWorker(worker, out errorMessage))
                        {
                            TempData["WorkerSuccess"] = "Pracownik został dodany pomyślnie.";
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            TempData["WorkerError"] = "Dodanie pracownika nie powiodło się. Spróbuj ponownie. " + errorMessage;
                            return RedirectToAction("Index");
                        }
                    }

                    return View(worker);
                }
                catch (Exception ex)
                {
                    TempData["WorkerError"] = "Wystąpił błąd: " + ex.Message;
                    return View();
                }

            }
            else
            {
                return RedirectToAction("Index", "WorkerLogin");
            }
        }

        // GET: WorkerController/Edit/5
        public ActionResult Edit(int id)
        {
            if (Request.Cookies["CookieIsAdmin"] == "True")
            {
                if (id <= 0)
                {
                    TempData["WorkerError"] = "ID pracownika musi być większe niż 0.";
                    return RedirectToAction("Index");
                }
                WorkerModel worker = _getData.getWorker(id);
                if (worker == null)
                {
                    TempData["WorkerError"] = "Pracownik o takim id nie istnieje.";
                    return RedirectToAction("Index");
                }

                worker.Pracownik_haslo = "";
                return View(worker);
            }
            else
            {
                return RedirectToAction("Index", "WorkerLogin");
            }
        }

        // POST: WorkerController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(WorkerModel worker)
        {
            if (Request.Cookies["CookieIsAdmin"] == "True")
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        string errorMessage;
                        if (updateWorker(worker, out errorMessage))
                        {
                            TempData["WorkerSuccess"] = "Pracownik został edytowany pomyślnie.";
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            TempData["WorkerError"] = "Edycja pracownika nie powiodło się. Spróbuj ponownie. " + errorMessage; 
                            return RedirectToAction("Index");
                        }
                    }

                    return View(worker);
                }
                catch (Exception ex)
                {
                    TempData["WorkerError"] = "Wystąpił błąd: " + ex.Message;
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Index", "WorkerLogin");
            }
        }

        // GET: WorkerController/Delete/5
        public ActionResult Delete(int id)
        {
            if(Request.Cookies["CookieIsAdmin"] == "True")
            {
                if (id <= 0)
                {
                    TempData["WorkerError"] = "ID pracownika musi być większe niż 0.";
                    return RedirectToAction("Index");
                }
                WorkerModel worker = _getData.getWorker(id);
                if (worker == null)
                {
                    TempData["WorkerError"] = "Pracownik o takim id nie istnieje.";
                    return RedirectToAction("Index");
                }
                return View(worker);
            }
            else
            {
                return RedirectToAction("Index", "WorkerLogin");
            }
        }

        // POST: WorkerController/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult deleteConfirmation(int id)
        {
            if (Request.Cookies["CookieIsAdmin"] == "True")
            {
                string result = removeWorker(id);
                if (result != null)
                {
                    TempData["WorkerSuccess"] = "Pracownik został usunięty";
                    return RedirectToAction("Index");
                }
                else
                {

                    TempData["WorkerError"] = "Błąd";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("Index", "WorkerLogin");
            }
        }

        public bool addWorker(WorkerModel worker, out string errorMessage)
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
                        command.CommandText = "dodajPracownik";  
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@Pracownik_imie", worker.Pracownik_imie);
                        command.Parameters.AddWithValue("@Pracownik_nazwisko", worker.Pracownik_nazwisko);
                        command.Parameters.AddWithValue("@Pracownik_nr_telefonu", worker.Pracownik_nr_telefonu);
                        command.Parameters.AddWithValue("@Pracownik_email", worker.Pracownik_email);
                        string hashedPassword = GenerateMD5Hash(worker.Pracownik_haslo);
                        command.Parameters.AddWithValue("@Pracownik_haslo", hashedPassword);  

                        
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

        public bool updateWorker(WorkerModel worker, out string errorMessage)
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
                        command.CommandText = "edytujPracownik";
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@Id_pracownik", worker.Id_pracownik);
                        command.Parameters.AddWithValue("@Pracownik_imie", worker.Pracownik_imie);
                        command.Parameters.AddWithValue("@Pracownik_nazwisko", worker.Pracownik_nazwisko);
                        command.Parameters.AddWithValue("@Pracownik_nr_telefonu", worker.Pracownik_nr_telefonu);
                        command.Parameters.AddWithValue("@Pracownik_email", worker.Pracownik_email);

                        if (string.IsNullOrWhiteSpace(worker.Pracownik_haslo))
                        {
                            command.Parameters.AddWithValue("@Pracownik_haslo", DBNull.Value);
                        }
                        else
                        {
                            string hashedPassword = GenerateMD5Hash(worker.Pracownik_haslo);
                            command.Parameters.AddWithValue("@Pracownik_haslo", hashedPassword);
                        }

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

        public string removeWorker(int id)
        {
            string result = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("usunPracownik", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id_pracownik", id);

                command.ExecuteNonQuery();

                result = $"Pracownik o {id} został usunięty";

                connection.Close();

                return result;
            }
        }
        public static string GenerateMD5Hash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }

}





