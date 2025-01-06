using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WarsztatMVC.Models;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Data;
using System.Diagnostics;
using System.ComponentModel;
using WarsztatMVC.DAL;

namespace WarsztatMVC.Controllers
{
    public class ClientController : Controller
    {

        private readonly getData _getData;
        private readonly string? _connectionString;

        public ClientController(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionString")["Polaczenie"];
            _getData = new getData(_connectionString);
        }

        [HttpGet]
        public ActionResult Index(string? nrTelefonu, string? Imie, string? Nazwisko)
        {

            if (Request.Cookies["CookieUserID"] != null)
            {
                List<ClientModel> clients = _getData.getClients();

                if (Imie != null)
                {
                    Imie = Imie.Trim();
                    Imie = Imie.Replace(" ", "");
                    Imie = char.ToUpper(Imie[0]) + Imie.Substring(1);
                    clients = clients.Where(c => c.Imie == Imie).ToList();
                }
                if (Nazwisko != null)
                {
                    Nazwisko = Nazwisko.Trim();
                    Nazwisko = Nazwisko.Replace(" ", "");
                    Nazwisko = char.ToUpper(Nazwisko[0]) + Nazwisko.Substring(1);
                    clients = clients.Where(c => c.Nazwisko == Nazwisko).ToList();
                }
                if (nrTelefonu != null)
                {
                    nrTelefonu = nrTelefonu.Trim();
                    nrTelefonu = nrTelefonu.Replace(" ", "");
                    nrTelefonu = nrTelefonu.Replace("-", "");
                    clients = clients.Where(c => c.Numer_telefonu == nrTelefonu).ToList();
                }

                if (TempData.ContainsKey("SuccessMessage"))
                {
                    ViewBag.SuccessMessage = TempData["SuccessMessage"];
                }

                if (TempData.ContainsKey("ErrorMessage"))
                {
                    ViewBag.ErrorMessage = TempData["ErrorMessage"];
                }
                if (clients.Any())
                {
                    return View(clients);
                }
                else
                {
                    ViewBag.NoClientsMessage = "Brak klientów o podanych danych.";
                    return View(clients);
                }
            }
            else
            {
                return RedirectToAction("Index", "WorkerLogin");
            }

            
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

        // POST: ClientController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ClientModel client)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string errorMessage;
                    if (addClient(client, out errorMessage))
                    {
                        TempData["SuccessMessage"] = "Klient został dodany pomyślnie.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Dodanie klienta nie powiodło się. Spróbuj ponownie. " + errorMessage;
                        return RedirectToAction("Index");
                    }
                }
                return View(client);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Wystąpił błąd: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // GET: ClientController/Edit/5
        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (Request.Cookies["CookieUserID"] != null)
            {
                List<ClientModel> clientId = _getData.getClientById(id);
                if (clientId == null)
                {
                    TempData["ErrorMessage"] = "Klient o takim id nie istnieje.";
                    return RedirectToAction("Index");


                }
                return View(clientId.FirstOrDefault());
            }
            else
            {
                return RedirectToAction("Index", "WorkerLogin");
            }

            
        }

        // POST: ClientController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ClientModel client)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    string errorMessage;
                    if (updateClient(client, out errorMessage)) 
                    {
                        TempData["SuccessMessage"] = "Klient został edytowany pomyślnie.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Edycja klienta nie powiodła się. Spróbuj ponownie. " + errorMessage;
                        return RedirectToAction(nameof(Index));
                    }
                }
                return View(client);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Wystąpił błąd: " + ex.Message;
                return View();
            }
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (Request.Cookies["CookieUserID"] != null)
            {
                List<ClientModel> clientId = _getData.getClientById(id);
                if (clientId == null)
                {
                    TempData["ErrorMessage"] = "Klient o takim id nie istnieje.";
                    return RedirectToAction("Index");


                }
                return View(clientId.FirstOrDefault());
            }
            else
            {
                return RedirectToAction("Index", "WorkerLogin");
            }

            
        }

        // POST: ClientController/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult deleteConfirmation(int id)
        {
            string result = removeClient(id);
            if(result != null)
            {
                TempData["SuccessMessage"] = "Klient został usunięty.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorMessage"] = "Klient o takim id nie istnieje.";
                return RedirectToAction("Index");
            }
            //return View();
        }
        
        

        public bool addClient(ClientModel client, out string errorMessage)
        {
            errorMessage = null;
            try
            {
                client.Numer_telefonu = new string(client.Numer_telefonu.Where(char.IsDigit).ToArray());
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "dodajKlient";
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@Imie", client.Imie);
                        command.Parameters.AddWithValue("@Nazwisko", client.Nazwisko);
                        command.Parameters.AddWithValue("@Nr_telefonu", client.Numer_telefonu);

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
        

        public bool updateClient(ClientModel client, out string errorMessage)
        {
            errorMessage = null;
            try
            {
                using(SqlConnection connection = new SqlConnection(_connectionString)) 
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand())
                    {

                        command.Connection = connection;
                        command.CommandText = "edytujKlient";
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@Klient_Id", client.Id_klient);
                        command.Parameters.AddWithValue("@Imie", client.Imie);
                        command.Parameters.AddWithValue("@Nazwisko", client.Nazwisko);
                        command.Parameters.AddWithValue("@Nr_telefonu", client.Numer_telefonu);

                        int rowsAffected = command.ExecuteNonQuery();

                        connection.Close();
                        
                        return rowsAffected > 0;

                    }
                }

            }
            catch (SqlException ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public string removeClient(int idClient)
        {
            string result = null;

            using(SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("usunKlient", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Klient_Id", idClient);

                connection.Open();
                command.ExecuteNonQuery();

                result = $"Klient o {idClient} został usunięty";

                connection.Close();

                return result;
            }
        }
    }
}
