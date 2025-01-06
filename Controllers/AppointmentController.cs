using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using WarsztatMVC.Models;
using System.Diagnostics;
using WarsztatMVC.DAL;

namespace WarsztatMVC.Controllers
{
    public class AppointmentController : Controller
    {


        private readonly getData _getData;
        private readonly string? _connectionString;
    
        public AppointmentController(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionString")["Polaczenie"];
            _getData = new getData(_connectionString);
        }

        List<CarModel> cars = new List<CarModel>();
        List<ClientModel> clients = new List<ClientModel>();
        List<CompanyModel> companies = new List<CompanyModel>();
        List<VisitModel> visits = new List<VisitModel>();
        List<DataRepairModel> repairs = new List<DataRepairModel>();


        [HttpGet]
        public ActionResult Index(bool? czyZakonczona, DateTime? dataRozpoczecia)
        {
            if (Request.Cookies["CookieUserID"] != null)
            {
                visits = _getData.getVisits();

                if (czyZakonczona.HasValue)
                {
                    visits = visits.Where(v => v.Czy_zakonczona == czyZakonczona.Value).ToList();
                    foreach (var v in visits)
                    {
                        Debug.WriteLine(v.Czy_zakonczona);
                    }
                }
                if (dataRozpoczecia.HasValue)
                {
                    visits = visits.Where(v => v.Data_start.Value.Date == dataRozpoczecia.Value.Date).ToList();
                }

                AppointmentModel objAppointmentVeiwModel = new AppointmentModel();
                objAppointmentVeiwModel.VisitViewModel = visits;

                if (TempData.ContainsKey("AppointmentSuccess"))
                {
                    ViewBag.AppointmentSuccess = TempData["AppointmentSuccess"];
                }

                if (TempData.ContainsKey("AppointmentError"))
                {
                    ViewBag.AppointmentError = TempData["AppointmentError"];
                }
                if (TempData.ContainsKey("RepairSuccess"))
                {
                    ViewBag.RepairSuccess = TempData["RepairSuccess"];
                }

                if (TempData.ContainsKey("RepairError"))
                {
                    ViewBag.RepairError = TempData["RepairError"];
                }
                

                if (visits.Any())
                {
                    objAppointmentVeiwModel.VisitViewModel = visits;
                    return View(objAppointmentVeiwModel);
                }
                else
                {
                    ViewBag.NoVisitsMessage = "Brak wizyt.";
                    return View(objAppointmentVeiwModel);
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
                cars = _getData.carsData();
                Console.WriteLine("Zapytanie SQL wykonane.");
                Console.WriteLine("Liczba samochodów: " + cars.Count);
                clients = _getData.getClients();
                companies = _getData.getCompaniesForAppointment();
                visits = _getData.getVisits();

                AppointmentModel objAppointmentVeiwModel = new AppointmentModel
                {
                    CarViewModel = cars,
                    ClientViewModel = clients,
                    CompanyViewModel = companies,
                    VisitViewModel = new List<VisitModel> { new VisitModel() } 
                };

                return View(objAppointmentVeiwModel);
            }
            else
            {
                return RedirectToAction("Index", "WorkerLogin");
            }

           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(VisitModel visit)
        {
            try
            {
                
                if (ModelState.IsValid)
                {
                    string errorMessage;

                    if (addAppointment(visit, out errorMessage))
                    {

                        TempData["AppointmentSuccess"] = "Wizyta została dodana pomyślnie.";
                        return RedirectToAction("Index");
                    }
                    else
                    {

                        TempData["AppointmentError"] = "Nie udało się dodać wizyty. " + errorMessage;
                        return RedirectToAction("Index");

                    }
                }
                
                ViewBag.carsInfo = _getData.carsData();
                ViewBag.clientsInfo = _getData.getClients();
                ViewBag.companiesInfo = _getData.getCompaniesForAppointment();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["AppointmentError"] = "Wystąpił błąd: " + ex.Message;
                Debug.WriteLine(ex.Message);
                return View();
            }
        }

        [HttpGet]
        public ActionResult Details(int id)
        {

            if (Request.Cookies["CookieUserID"] != null)
            {
                List<VisitModel> visitID = _getData.getVisitById(id);
                List<DataRepairModel> repairID = _getData.getVisitsReparis(id);

                AppointmentModel objAppointmentVeiwModel = new AppointmentModel();
                objAppointmentVeiwModel.VisitViewModel = visitID;
                objAppointmentVeiwModel.DataRepairViewModel = repairID;

                if (visitID == null)
                {
                    TempData["AppointmentError"] = "Wizyta o takim id nie istnieje.";
                    Debug.WriteLine("Nie działa");
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(objAppointmentVeiwModel);
                }
            }
            else
            {
                return RedirectToAction("Index", "WorkerLogin");
            }

            

            
        }

        [HttpGet]
        public ActionResult EndVisit(int id)
        {

            if (Request.Cookies["CookieUserID"] != null)
            {
                List<VisitModel> visitID = _getData.getVisitById(id);

                AppointmentModel objAppointmentVeiwModel = new AppointmentModel();
                objAppointmentVeiwModel.VisitViewModel = visitID;

                if (visitID == null || visitID.Count == 0)
                {
                    // Jeśli nie znaleziono wizyty, przekieruj do akcji Index z informacją
                    TempData["AppointmentError"] = "Nie znaleziono wizyty o podanym ID.";
                    return RedirectToAction("Index");
                }

                // Sprawdź, czy wizyta już została zakończona
                if (visitID.Any(v => v.Czy_zakonczona.GetValueOrDefault()))
                {
                    TempData["AppointmentError"] = "Wizyta została już zakończona.";
                    return RedirectToAction("Index");
                }

                // Wyświetl formularz zakończenia wizyty
                return View(visitID.First());
            }
            else
            {
                return RedirectToAction("Index", "WorkerLogin");
            }

            
        }
        [HttpPost]
        public ActionResult EndVisit(VisitModel visit)
        {
            if (ModelState.IsValid)
            {
                if(markVisitAsCompleted(visit.Id_visit, visit.Data_koniec))
                {
                    TempData["AppointmentSuccess"] = "Wizyta została zakończona pomyślnie.";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["AppointmentSuccess"] = "Nie udało się zakończyć wizyty";
                }
                
            }

            TempData["AppointmentError"] = "Wystąpiły błędy walidacji: ";
            return View(visit);
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (Request.Cookies["CookieUserID"] != null)
            {

                clients = _getData.getClients();
                companies = _getData.getCompaniesForAppointment();
                visits = _getData.getVisitById(id);
                cars = _getData.carsData();
                if (visits.First().Czy_zakonczona == true)
                {

                    return RedirectToAction("Index");
                }
                else
                {
                    AppointmentModel objAppointmentVeiwModel = new AppointmentModel
                    {
                        CarViewModel = cars,
                        ClientViewModel = clients,
                        CompanyViewModel = companies,
                        VisitViewModel = visits
                    };

                    return View(objAppointmentVeiwModel);
                }
            }
            else
            {
                return RedirectToAction("Index", "WorkerLogin");
            }

            

            
        }

        // POST: AppointmentController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(VisitModel visit)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Debug.WriteLine(Request.Cookies["CookieUserID"]);
                    string errorMessage;
                    if (updateAppointment(visit, out errorMessage))
                    {

                        TempData["AppointmentSuccess"] = "Edycja wizyty się powiodła";

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["AppointmentError"] = "Nie udało się edytować wizyty." + errorMessage;
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    Debug.WriteLine("Model state nie jest valid");
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["RepairError"] = "Wystąpił błąd: " + ex.Message;
                Debug.WriteLine(ex.Message);
                return View();
            }
        }


        

        public bool addAppointment(VisitModel visit, out string errorMessage)
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
                        command.CommandText = "dodajWizyta";
                        int id_pracownik = Convert.ToInt32(Request.Cookies["CookieUserID"]);
                        Debug.WriteLine($"Id pracownik {id_pracownik}");
                        command.Parameters.AddWithValue("@Nr_rejestracyjny", visit.Nr_rejestracyjny);
                        command.Parameters.AddWithValue("@Nr_telefonu", visit.Numer_telefonu);
                        command.Parameters.AddWithValue("@NIP", visit.nip);
                        command.Parameters.AddWithValue("@Pracownik", id_pracownik);
                        command.Parameters.AddWithValue("@Przebieg_start", visit.Przebieg_start);
                        command.Parameters.AddWithValue("@Data_start", visit.Data_start);
                        command.Parameters.AddWithValue("@Poziom_paliwa_start", visit.Paliwo_start);
                        command.Parameters.AddWithValue("@Opis_klienta", visit.Opis_klienta);
                        command.Parameters.AddWithValue("@Czy_zakonczona", false);

                        

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

        private bool markVisitAsCompleted(int? visitId, DateTime? dataKoniec)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;

                    command.CommandText = "UPDATE Wizyta SET Czy_zakonczona = 1, Data_koniec = @Data_koniec WHERE Id_wizyta = @Id_wizyta";
                    command.Parameters.AddWithValue("@Id_wizyta", visitId);
                    command.Parameters.AddWithValue("@Data_koniec", dataKoniec ?? (object)DBNull.Value);
                    
                    int rowsAffected = command.ExecuteNonQuery();
                    connection.Close();
                    return rowsAffected > 0;

                }
                
            }
        }
        public bool updateAppointment(VisitModel visit, out string errorMessage)
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
                        command.CommandText = "edytujWizyta";

                        int id_pracownik = Convert.ToInt32(Request.Cookies["CookieUserID"]);
                        command.Parameters.AddWithValue("@Id_wizyta", visit.Id_visit);
                        command.Parameters.AddWithValue("@Nr_rejestracyjny", visit.Nr_rejestracyjny);
                        command.Parameters.AddWithValue("@Nr_telefonu", visit.Numer_telefonu);
                        command.Parameters.AddWithValue("@NIP", visit.nip);
                        command.Parameters.AddWithValue("@Pracownik", id_pracownik);
                        command.Parameters.AddWithValue("@Przebieg_start", visit.Przebieg_start);
                        command.Parameters.AddWithValue("@Data_start", visit.Data_start);
                        command.Parameters.AddWithValue("@Poziom_paliwa_start", visit.Paliwo_start);
                        command.Parameters.AddWithValue("@Opis_klienta", visit.Opis_klienta);
                        //command.Parameters.AddWithValue("@Czy_zakonczona", false);



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

    }

}

