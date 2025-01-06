using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WarsztatMVC.Models;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Data;
using WarsztatMVC.DAL;
using Microsoft.CodeAnalysis;

namespace WarsztatMVC.Controllers
{
    public class CarController : Controller
    {
        private readonly getData _getData;
        private readonly string? _connectionString;

        public CarController(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionString")["Polaczenie"];
            _getData = new getData(_connectionString);
        }

        [HttpGet]
        public ActionResult Index(string? Marka, string? Model, string? NrRejestracyjny, string? NrVin)
        {
            if (Request.Cookies["CookieUserID"] != null)
            {
                List<CarModel> cars = _getData.getCars();

                if (Marka != null)
                {
                    Marka = Marka.Trim();
                    Marka = Marka.Replace(" ", "");
                    Marka = char.ToUpper(Marka[0]) + Marka.Substring(1);
                    cars = cars.Where(c => c.Marka == Marka).ToList();
                }
                if (Model != null)
                {
                    Model = Model.Trim();
                    Model = Model.Replace(" ", "");
                    Model = char.ToUpper(Model[0]) + Model.Substring(1);
                    cars = cars.Where(c => c.Model == Model).ToList();
                }
                if (NrRejestracyjny != null)
                {
                    NrRejestracyjny = NrRejestracyjny.Trim();
                    NrRejestracyjny = NrRejestracyjny.ToUpper();
                    cars = cars.Where(c => c.Nr_rejestracyjny == NrRejestracyjny).ToList();
                }
                if (NrVin != null)
                {
                    NrVin = NrVin.Trim();
                    NrVin = NrVin.ToUpper();
                    cars = cars.Where(c => c.VIN == NrVin).ToList();
                }

                if (TempData.ContainsKey("CarSuccess"))
                {
                    ViewBag.CarSuccess = TempData["CarSuccess"];
                }

                if (TempData.ContainsKey("CarError"))
                {
                    ViewBag.CarError = TempData["CarError"];
                }

                if (cars.Any())
                {
                    return View(cars);
                }
                else
                {
                    ViewBag.NoCarsMessage = "Brak samochdów o podanych danych.";
                    return View(cars);
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
                List<string> fuelTypes = _getData.getAllFuel();
                ViewBag.FuelTypes = fuelTypes;

                return View();
            }
            else
            {
                return RedirectToAction("Index", "WorkerLogin");
            }

            
        }

        // POST: CarController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CarModel car)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string errorMessage;
                    if (addCar(car, out errorMessage))
                    {
                        TempData["CarSuccess"] = "Samochód został dodany pomyślnie.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["CarError"] = "Dodanie samochodu nie powiodło się. Spróbuj ponownie. " + errorMessage; 
                        return RedirectToAction("Index");
                    }
                }
                ViewBag.FuelTypes = _getData.getAllFuel();
                return View(car);
            }
            catch (Exception ex)
            {
                TempData["CarError"] = "Wystąpił błąd: " + ex.Message;
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
                    TempData["CarError"] = "ID samochodu musi być większe niż 0.";
                    return RedirectToAction("Index");
                }

                List<string> fuelTypes = _getData.getAllFuel();
                ViewBag.FuelTypes = fuelTypes;
                List<CarModel> car = _getData.getCarById(id);
                if (car == null || car.Count == 0)
                {
                    TempData["CarError"] = "Samochod o takim id nie istnieje.";
                    return RedirectToAction("Index");
                }
                return View(car.FirstOrDefault());
            }   
            else
            {
                return RedirectToAction("Index", "WorkerLogin");
            }

           
        }

        // POST: CarController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CarModel car)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string errorMessage;
                    if (updateCar(car, out errorMessage))
                    {
                        TempData["CarSuccess"] = "Samochód został edytowany pomyślnie.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["CarError"] = "Edycja samochodu nie powiodło się. Spróbuj ponownie. " + errorMessage; ;
                        return RedirectToAction("Index");
                    }
                   
                }
                List<string> fuelTypes = _getData.getAllFuel();
                ViewBag.FuelTypes = fuelTypes;
                return View(car);
            }


            catch (Exception ex)
            {
                TempData["CarError"] = "Wystąpił błąd: " + ex.Message;
                return View();
            }
        
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (Request.Cookies["CookieUserID"] != null)
            {
                List<CarModel> carId = _getData.getCarById(id);
                if (carId == null || carId.Count == 0)
                {
                    TempData["CarError"] = "Brak samochodu o podanym ID";
                    return RedirectToAction("Index");
                }

                return View(carId.FirstOrDefault());
            }
            else
            {
                return RedirectToAction("Index", "WorkerLogin");
            }

            
        }

        // POST: CarController/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult deleteConfirmation(int id)
        {
            
            string result = removeCar(id);

            if (result != null)
            {
                TempData["CarSuccess"] = "Samochód został usunięty";
                return RedirectToAction("Index");
            }
            else
            {

                TempData["CarError"] = "Błąd";
                return RedirectToAction("Index");
            }
        }

        public bool addCar(CarModel car, out string errorMessage)
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
                        command.CommandText = "dodajSamochod";
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@Marka", car.Marka);
                        command.Parameters.AddWithValue("@Model", car.Model);
                        command.Parameters.AddWithValue("@Nr_rejestracyjny", car.Nr_rejestracyjny);
                        command.Parameters.AddWithValue("@VIN", car.VIN);
                        command.Parameters.AddWithValue("@Rok", car.Rok);
                        command.Parameters.AddWithValue("@Miesiac", car.Miesiac);
                        command.Parameters.AddWithValue("@Moc", car.Moc);
                        command.Parameters.AddWithValue("@Paliwo", car.Paliwo);

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
        public bool updateCar(CarModel car, out string errorMessage)
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
                        command.CommandText = "edytujSamochod";
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@Id_samochod", car.Id_samochod);
                        command.Parameters.AddWithValue("@Marka", car.Marka);
                        command.Parameters.AddWithValue("@Model", car.Model);
                        command.Parameters.AddWithValue("@Nr_rejestracyjny", car.Nr_rejestracyjny);
                        command.Parameters.AddWithValue("@VIN", car.VIN);
                        command.Parameters.AddWithValue("@Rok", car.Rok);
                        command.Parameters.AddWithValue("@Miesiac", car.Miesiac);
                        command.Parameters.AddWithValue("@Moc", car.Moc);
                        command.Parameters.AddWithValue("@Paliwo", car.Paliwo);

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

        public string removeCar(int idSamochod)
        {
            string result = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("usunSamochod", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Samochod_Id", idSamochod);

                command.ExecuteNonQuery();

                result = $"Samochód o {idSamochod} został usunięty";

                connection.Close();

                return result;
            }
        }

    }
}
