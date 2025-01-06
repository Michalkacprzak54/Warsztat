using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WarsztatMVC.Models;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Data;
using System.Diagnostics;
using System.Configuration;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using WarsztatMVC.DAL;

namespace WarsztatMVC.Controllers
{
    public class CompanyController : Controller
    {
        private readonly getData _getData;
        private readonly string? _connectionString;

        public CompanyController(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionString")["Polaczenie"];
            _getData = new getData(_connectionString);
        }



        [HttpGet]
        public ActionResult Index(string? Nazwa, long? nip, long? regon)
        {
            

            if (Request.Cookies["CookieUserID"] != null)
            {
                List<CompanyModel> companies = _getData.getCompanies();


                if (Nazwa != null)
                {
                    Nazwa = Nazwa.Trim();
                    Nazwa = Nazwa.Replace(" ", "");
                    Nazwa = char.ToUpper(Nazwa[0]) + Nazwa.Substring(1);

                    companies = companies.Where(c => c.nazwa_firma == Nazwa).ToList();
                }
                if (nip != null)
                {
                    companies = companies.Where(c => c.nip == nip).ToList();
                }
                if (regon != null)
                {
                    companies = companies.Where(c => c.regon == regon).ToList();
                }
                if (companies.Any())
                {
                    return View(companies);
                }
                else
                {
                    ViewBag.NoCompaniesMessage = "Brak firm o podanych danych.";
                    return View(companies);
                }
            }
            else
            {
                return RedirectToAction("Index", "WorkerLogin");
            }

        }

        // GET: CompanyController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        [HttpGet]
        public ActionResult Create()
        {
            if (Request.Cookies["CookieUserID"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "WorkerLogin");
            }

            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CompanyModel company)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string errorMessage;
                    if (addCompany(company, out errorMessage))
                    {
                        TempData["SuccessFirma"] = "Frima została dodany pomyślnie.";
                        return RedirectToAction("Index");
                    }
                    else
                    {

                        TempData["ErrorFirma"] = "Dodanie firmy nie powiodło się. Spróbuj ponownie. " + errorMessage;
                        return RedirectToAction("Index");
                    }
                }
                return View(company);
            }
            catch (Exception ex)
            {
                TempData["ErrorFirma"] = "Wystąpił błąd: " + ex.Message;
                return View();
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (Request.Cookies["CookieUserID"] != null)
            {
                if (id <= 0)
                {
                    TempData["ErrorFirma"] = "ID musi być większe niż 0.";
                    return RedirectToAction("Index");
                }

                List<CompanyModel> companyId = _getData.getCompanyById(id);
                if (companyId == null || companyId.Count == 0)
                {
                    TempData["ErrorFirma"] = "Firma o takim id nie istnieje.";
                    return RedirectToAction("Index");
                }
                return View(companyId.FirstOrDefault());
            }
            else
            {
                return RedirectToAction("Index", "WorkerLogin");
            }

            

        }

        // POST: CompanyController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CompanyModel company)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (updateCompany(company))
                    {
                        TempData["SuccessFirma"] = "Frima została edytowana pomyślnie.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["ErrorFirma"] = "Edycja firmy się nie powiodła.";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    foreach(var modelError in ModelState.Values.SelectMany(v => v.Errors))
{
                        // Tutaj możesz wykonać dowolne działania związane z błędem, np. logowanie, obsługa, etc.
                        var errorMessage = modelError.ErrorMessage;
                        var exception = modelError.Exception;

                        // Przykładowe logowanie błędów
                        Debug.WriteLine($"Błąd modelu: {errorMessage}, Exception: {exception?.Message}");
                    }
                }


                return View(company);
            }
            catch(Exception ex)
            {
                TempData["ErrorFirma"] = "Wystąpił błąd: " + ex.Message;
                return View();
            }

        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (Request.Cookies["CookieUserID"] != null)
            {
                List<CompanyModel> companyId = _getData.getCompanyById(id);
                if (companyId == null || companyId.Count == 0)
                {
                    TempData["ErrorFirma"] = "Brak firmy o podanym ID";
                    return RedirectToAction("Index");
                }
                return View(companyId.FirstOrDefault());
            }
            else
            {
                return RedirectToAction("Index", "WorkerLogin");
            }

            
        }

        // POST: CompanyController/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult deleteConfirmation(int id)
        {
            string result = removeCompany(id);
            if (result != null)
            {
                TempData["SuccessFirma"] = "Firma została usunięta.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorFirma"] = "Firma o takim id nie istnieje.";
                return RedirectToAction("Index");
            }
        }


        

        public bool addCompany(CompanyModel company, out string errorMessage)
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
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "dodajFirma";

                        command.Parameters.AddWithValue("@Nazwa", company.nazwa_firma);
                        command.Parameters.AddWithValue("@Miasto", company.miasto);
                        command.Parameters.AddWithValue("@Kod_pocztowy", company.kod_pocztowy);
                        command.Parameters.AddWithValue("@Ulica", company.ulica);
                        command.Parameters.AddWithValue("@Nr_domu", company.numer_domu);
                        command.Parameters.AddWithValue("@NIP", company.nip);
                        command.Parameters.AddWithValue("@REGON", company.regon);

                        if (company.numer_lokalu.HasValue)
                        {
                            command.Parameters.AddWithValue("@Nr_lokalu", company.numer_lokalu);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@Nr_lokalu", DBNull.Value);
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

        public string removeCompany(int idCompany)
        {
            string result = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("usunFirma", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Company_Id", idCompany);

                connection.Open();
                command.ExecuteNonQuery();

                result = $"Klient o {idCompany} został usunięty";

                connection.Close();

                return result;
            }
        }

        public bool updateCompany(CompanyModel company)
        {
            try
            {
                using(SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using(SqlCommand command = new SqlCommand()) 
                    { 
                        command.Connection = connection;
                        command.CommandText = "edytujFirma";
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@id_firma", company.Id_frima);
                        command.Parameters.AddWithValue("@nazwa_firma", company.nazwa_firma);
                        command.Parameters.AddWithValue("@miasto", company.miasto);
                        command.Parameters.AddWithValue("@kod_pocztowy", company.kod_pocztowy);
                        command.Parameters.AddWithValue("@ulica", company.ulica);
                        command.Parameters.AddWithValue("@numer_domu", company.numer_domu);
                        
                        if(company.numer_lokalu.HasValue)
                        {
                            command.Parameters.AddWithValue("@numer_lokalu", company.numer_lokalu.Value);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@numer_lokalu", DBNull.Value);
                        }

                        int rowsAffected = command.ExecuteNonQuery();
                        connection.Close();
                        return rowsAffected > 0;


                    }
                }

            }
            catch (Exception ex)
            {
                //Debug.WriteLine(ex.Message);
                return false;
            }
        }
            
    }
}
