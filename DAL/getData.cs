using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using WarsztatMVC.Models;

namespace WarsztatMVC.DAL
{
    public class getData
    {
        private readonly string _connectionString;

        public getData(string connectionString)
        {
            _connectionString = connectionString;
        }


        public List<ActivityModel> getActivities()
        {
            List<ActivityModel> actions = new List<ActivityModel>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = @"
                SELECT c.Id_czynnosc, c.Nazwa, c.Cena_netto, c.Roboczogodziny,
                       CASE WHEN EXISTS (SELECT 1 FROM Naprawa n WHERE n.Id_czynnosc = c.Id_czynnosc) THEN 1 ELSE 0 END AS IsLinkedToRepair
                FROM Czynnosc c";

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ActivityModel model = new ActivityModel()
                            {

                                Id_czynnosc = reader.GetInt32("Id_czynnosc"),
                                Nazwa = reader["Nazwa"].ToString(),
                                Cena_netto = reader.GetDecimal("Cena_netto"),
                                Roboczogodziny = reader.GetDecimal("Roboczogodziny"),
                                IsLinkedToRepair = reader.GetInt32("IsLinkedToRepair") == 1

                            };
                            actions.Add(model);
                        }

                    }
                }
                connection.Close();
            }
            return actions;

        }


        public ActivityModel getActivity(int id)
        {
            ActivityModel action = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "SELECT * FROM Czynnosc WHERE Id_czynnosc = @id";
                    command.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            action = new ActivityModel()
                            {
                                Id_czynnosc = reader.GetInt32(reader.GetOrdinal("Id_czynnosc")),
                                Nazwa = reader["Nazwa"].ToString(),
                                Cena_netto = reader.GetFieldValue<decimal>("Cena_netto"),
                                Roboczogodziny = reader.GetFieldValue<decimal>("Roboczogodziny")
                            };
                        }

                    }
                }
                connection.Close();
            }
            return action;
        }

        public List<string> getProducents()
        {
            List<string> producents = new List<string>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "SELECT * FROM Producent";

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            producents.Add(reader["Nazwa"].ToString());
                        }
                    }
                }
                connection.Close();
            }
            return producents;
        }

        public List<string> getActivityNamesOnly()
        {
            List<string> activityNames = new List<string>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = @"
                SELECT Nazwa
                FROM Czynnosc";

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            activityNames.Add(reader["Nazwa"].ToString());
                        }
                    }
                }
                connection.Close();
            }
            return activityNames;
        }

        public List<PartModel> getParts()
        {
            List<PartModel> parts = new List<PartModel>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "SELECT * FROM widokCzesc";

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PartModel part = new PartModel()
                            {

                                Id_czesc = reader.GetInt32("ID"),
                                Czesc = reader["Część"].ToString(),
                                IsLinkedToRepair = reader.GetInt32("IsLinkedToRepair") == 1


                            };
                            if (!Convert.IsDBNull(reader["Czynność"]))
                            {
                                part.Czynnosc = reader["Czynność"].ToString();
                                part.Producent = reader["Producent"].ToString();
                                part.Nr_czesc = reader["Nr_czesc"].ToString();
                                part.Cena_netto = reader.GetDecimal("Cena_netto");
                            }
                            else
                            {
                                part.Czynnosc = null;
                                part.Producent = null;
                                part.Nr_czesc = null;
                                part.Cena_netto = null;

                            }
                            parts.Add(part);
                        }

                    }
                }
                connection.Close();
            }
            return parts;
        }

        public PartModel getPart(int id)
        {
            PartModel part = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "SELECT * FROM widokCzesc WHERE ID = @id";
                    command.Parameters.AddWithValue("@id", id);


                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            part = new PartModel()
                            {

                                Id_czesc = reader.GetInt32("ID"),
                                Czesc = reader["Część"].ToString()


                            };
                            if (!Convert.IsDBNull(reader["Czynność"]))
                            {
                                part.Czynnosc = reader["Czynność"].ToString();
                                part.Producent = reader["Producent"].ToString();
                                part.Nr_czesc = reader["Nr_czesc"].ToString();
                                part.Cena_netto = reader.GetDecimal("Cena_netto");
                            }
                            else
                            {
                                part.Czynnosc = null;
                                part.Producent = null;
                                part.Nr_czesc = null;
                                part.Cena_netto = null;

                            }
                        }

                    }
                }
                connection.Close();
            }
            return part;
        }
        public List<FixModel> getFixes()
        {
            List<FixModel> fixes = new List<FixModel>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "EXEC pokazNaprawa";

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            FixModel fix = new FixModel()
                            {
                                Id = reader.GetInt32("ID"),
                                Id_czynnosc = reader.GetInt32("Id_czynnosc"),
                                Czynnosc = reader["Czynność"].ToString(),
                                Id_wizyta = reader.GetInt32("Id_Wizyta"),
                                Nr_rejestracyjny = reader["Nr_rejestracyjny"].ToString(),
                                Samochod = reader["Samochód"].ToString(),
                                Czesc = reader["Część"].ToString(),
                                Id_czesc = reader.GetInt32("Id_czesc"),
                                Pracownik = reader["Pracownik"].ToString(),
                                Id_pracownik = reader.GetInt32("Id_pracownik")
                            };
                            if (!Convert.IsDBNull(reader["Uwagi"]))
                            {
                                fix.Uwagi = reader["Uwagi"].ToString();
                            }
                            else
                            {
                                fix.Uwagi = null;
                            }
                            fixes.Add(fix);
                        }

                    }
                }
                connection.Close();
            }
            return fixes;

        }

        public List<FixModel> getFixById(int idFiX)
        {
            List<FixModel> fixId = new List<FixModel>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "SELECT * FROM widokNaprawa WHERE ID  = @Id_naprawa";
                    command.Parameters.AddWithValue("@Id_naprawa", idFiX);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            FixModel fix = new FixModel()
                            {
                                Id = reader.GetInt32("ID"),
                                Id_czynnosc = reader.GetInt32("Id_czynnosc"),
                                Czynnosc = reader["Czynność"].ToString(),
                                Id_wizyta = reader.GetInt32("Id_Wizyta"),
                                Nr_rejestracyjny = reader["Nr_rejestracyjny"].ToString(),
                                Samochod = reader["Samochód"].ToString(),
                                Czesc = reader["Część"].ToString(),
                                Id_czesc = reader.GetInt32("Id_czesc"),
                                Pracownik = reader["Pracownik"].ToString(),
                                Id_pracownik = reader.GetInt32("Id_pracownik")
                            };
                            if (!Convert.IsDBNull(reader["Uwagi"]))
                            {
                                fix.Uwagi = reader["Uwagi"].ToString();
                            }
                            else
                            {
                                fix.Uwagi = null;
                            }
                            fixId.Add(fix);
                        }
                    }
                }
                connection.Close();
            }
            return fixId;
        }
        public List<DataRepairModel> getVisitsReparis(int idVisit)
        {
            List<DataRepairModel> repairs = new List<DataRepairModel>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "SELECT * FROM widokNaprawa WHERE Id_wizyta  = @Id_wizyta";
                    command.Parameters.AddWithValue("@Id_wizyta", idVisit);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DataRepairModel repair = new DataRepairModel()
                            {
                                Id_naprawa = Convert.ToInt32(reader["ID"]),
                                Id_wizyta = Convert.ToInt32(reader["Id_wizyta"]),
                                Czynnosc = reader["Czynność"].ToString(),
                                Czesc = reader["Część"].ToString(),
                                Pracownik = reader["Pracownik"].ToString(),
                                Uwagi = reader["Uwagi"].ToString(),
                            };
                            repairs.Add(repair);
                        }
                    }
                }

                connection.Close();
            }
            return repairs;
        }


        public List<CarModel> carsData()
        {
            List<CarModel> cars = new List<CarModel>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "pokazSamochody";



                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            CarModel car = new CarModel()
                            {
                                Id_samochod = Convert.ToInt32(reader["Id_samochod"]),
                                Marka = reader["Marka"].ToString(),
                                Model = reader["Model"].ToString(),
                                Nr_rejestracyjny = reader["Nr_rejestracyjny"].ToString(),
                                VIN = reader["Vin"].ToString(),
                                Rok = Convert.ToInt32(reader["Rok"]),
                                Miesiac = Convert.ToInt32(reader["Miesiac"]),
                                Moc = Convert.ToInt32(reader["Moc"]),
                                Paliwo = reader["Paliwo"].ToString(),
                            };
                            cars.Add(car);
                        }
                    }
                }
                connection.Close();
            }
            return cars;

        }

        
        public List<VisitModel> getVisits()
        {
            List<VisitModel> visits = new List<VisitModel>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "pokazWizyta";


                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            VisitModel visit = new VisitModel()
                            {
                                Id_visit = Convert.ToInt32(reader["Id_wizyta"]),
                                Nr_rejestracyjny = reader["Nr_rejestracyjny"].ToString(),
                                Numer_telefonu = reader["Nr_telefonu"].ToString(),
                                Samochod = reader["Samochod"].ToString(),
                                Klient = reader["Klient"].ToString(),
                                Pracownik = reader["Pracownik"].ToString(),
                                Przebieg_start = Convert.ToInt32(reader["Przebieg_start"]),
                                Data_start = Convert.ToDateTime(reader["Data_start"]),
                                Paliwo_start = Convert.ToDouble(reader["Poziom_paliwa_start"]),
                                Opis_klienta = reader["Opis_klienta"].ToString()
                            };
                            if (!Convert.IsDBNull(reader["Firma"]))
                            {
                                visit.Firma = reader["Firma"].ToString();
                            }
                            else
                            {
                                visit.Firma = null;
                            }
                            if (!Convert.IsDBNull(reader["Nip"]))
                            {

                                visit.nip = Convert.ToInt64(reader["Nip"]);
                            }
                            else
                            {
                                visit.nip = null;
                            }
                            if (!Convert.IsDBNull(reader["Data_koniec"]))
                            {
                                visit.Data_koniec = Convert.ToDateTime(reader["Data_koniec"]);
                            }
                            else
                            {
                                visit.Data_koniec = null;
                            }
                            if (!Convert.IsDBNull(reader["Czy_zakonczona"]))
                            {
                                visit.Czy_zakonczona = Convert.ToBoolean(reader["Czy_zakonczona"]);
                            }
                            else
                            {
                                visit.Czy_zakonczona = null;
                            }
                            visits.Add(visit);
                        }
                    }
                }
                connection.Close();
            }
            return visits;

        }
        public List<VisitModel> getVisitById(int idVisit)
        {
            List<VisitModel> visit = new List<VisitModel>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "pokazWizytaById";
                    command.Parameters.AddWithValue("@VisitId", idVisit);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            VisitModel newVisit = new VisitModel()
                            {
                                Id_visit = Convert.ToInt32(reader["Id_wizyta"]),
                                Nr_rejestracyjny = reader["Nr_rejestracyjny"].ToString(),
                                Numer_telefonu = reader["Nr_telefonu"].ToString(),
                                Samochod = reader["Samochod"].ToString(),
                                Klient = reader["Klient"].ToString(),
                                Pracownik = reader["Pracownik"].ToString(),
                                Przebieg_start = Convert.ToInt32(reader["Przebieg_start"]),
                                Data_start = Convert.ToDateTime(reader["Data_start"]),
                                Paliwo_start = Convert.ToDouble(reader["Poziom_paliwa_start"]),
                                Opis_klienta = reader["Opis_klienta"].ToString()
                            };
                            if ((reader["Firma"]).ToString() != "Brak")
                            {
                                newVisit.Firma = reader["Firma"].ToString();
                                newVisit.nip = Convert.ToInt64(reader["Nip"]);
                            }
                            else
                            {
                                newVisit.Firma = null;
                                newVisit.nip = null;
                            }
                            if (!Convert.IsDBNull(reader["Data_koniec"]))
                            {
                                newVisit.Data_koniec = Convert.ToDateTime(reader["Data_koniec"]);
                            }
                            else
                            {
                                newVisit.Data_koniec = null;
                            }
                            if (!Convert.IsDBNull(reader["Czy_zakonczona"]))
                            {
                                newVisit.Czy_zakonczona = Convert.ToBoolean(reader["Czy_zakonczona"]);
                            }
                            else
                            {
                                newVisit.Czy_zakonczona = null;
                            }
                            visit.Add(newVisit);
                        }
                    }
                }
                connection.Close();
            }
            return visit;

        }
        public List<CompanyModel> getCompanies()
        {
            List<CompanyModel> companies = new List<CompanyModel>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "pokazFirma";
                    command.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            CompanyModel newCompany = new CompanyModel()
                            {
                                Id_frima = Convert.ToInt32(reader["Id_firma"]),
                                nazwa_firma = reader["Nazwa"].ToString(),
                                miasto = reader["Miasto"].ToString(),
                                kod_pocztowy = reader["Kod_pocztowy"].ToString(),
                                ulica = reader["Ulica"].ToString(),
                                numer_domu = reader["Nr_domu"].ToString(),
                                nip = Convert.ToInt64(reader["Nip"]),
                                regon = Convert.ToInt64(reader["Regon"]),

                            };
                            //numer_lokalu = Convert.ToInt32(reader["Nr_lokalu"]),
                            if (!Convert.IsDBNull(reader["Nr_lokalu"]))
                            {
                                newCompany.numer_lokalu = Convert.ToInt32(reader["Nr_lokalu"]);
                            }
                            else
                            {
                                newCompany.numer_lokalu = null;
                            }
                            if (newCompany.nazwa_firma != "Brak" && newCompany.numer_domu != "0")
                            {
                                companies.Add(newCompany);
                            }

                        }
                    }
                }
                connection.Close();
            }
            return companies;
        }

        public List<CompanyModel> getCompaniesForAppointment()
        {
            List<CompanyModel> companies = new List<CompanyModel>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "pokazFirma";
                    command.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            CompanyModel newCompany = new CompanyModel()
                            {
                                Id_frima = Convert.ToInt32(reader["Id_firma"]),
                                nazwa_firma = reader["Nazwa"].ToString(),
                                miasto = reader["Miasto"].ToString(),
                                kod_pocztowy = reader["Kod_pocztowy"].ToString(),
                                ulica = reader["Ulica"].ToString(),
                                numer_domu = reader["Nr_domu"].ToString(),
                                nip = Convert.ToInt64(reader["Nip"]),
                                regon = Convert.ToInt64(reader["Regon"]),

                            };
                            //numer_lokalu = Convert.ToInt32(reader["Nr_lokalu"]),
                            if (!Convert.IsDBNull(reader["Nr_lokalu"]))
                            {
                                newCompany.numer_lokalu = Convert.ToInt32(reader["Nr_lokalu"]);
                            }
                            else
                            {
                                newCompany.numer_lokalu = null;
                            }
                            companies.Add(newCompany);

                        }
                    }
                }
                connection.Close();
            }
            return companies;
        }

        public List<CompanyModel> getCompanyById(int idCompany)
        {
            List<CompanyModel> companyId = new List<CompanyModel>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "SELECT * FROM widokFirma WHERE Id_firma = @Id_firma";
                    command.Parameters.AddWithValue("@Id_firma", idCompany);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            CompanyModel newCompany = new CompanyModel()
                            {
                                Id_frima = Convert.ToInt32(reader["Id_firma"]),
                                nazwa_firma = reader["Nazwa"].ToString(),
                                miasto = reader["Miasto"].ToString(),
                                kod_pocztowy = reader["Kod_pocztowy"].ToString(),
                                ulica = reader["Ulica"].ToString(),
                                numer_domu = reader["Nr_domu"].ToString(),
                                nip = Convert.ToInt64(reader["Nip"]),
                                regon = Convert.ToInt64(reader["Regon"]),

                            };
                            //numer_lokalu = Convert.ToInt32(reader["Nr_lokalu"]),
                            if (!Convert.IsDBNull(reader["Nr_lokalu"]))
                            {
                                newCompany.numer_lokalu = Convert.ToInt32(reader["Nr_lokalu"]);
                            }
                            else
                            {
                                newCompany.numer_lokalu = null;
                            }
                            companyId.Add(newCompany);
                        }
                    }

                }
                connection.Close();
            }
            return companyId;
        }

        public List<ClientModel> getClients()
        {
            List<ClientModel> clients = new List<ClientModel>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "pokazKlient";


                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ClientModel client = new ClientModel()
                            {
                                Id_klient = reader.GetInt32("Id_klient"),
                                Imie = reader["Imie"].ToString(),
                                Nazwisko = reader["Nazwisko"].ToString(),
                                Numer_telefonu = reader["Nr_telefonu"].ToString(),
                            };
                            clients.Add(client);
                        }
                    }
                }
                connection.Close();
            }
            return clients;
        }

        public List<ClientModel> getClientById(int idClient)
        {
            List<ClientModel> client = new List<ClientModel>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "pokazKlientByid";
                    command.Parameters.AddWithValue("@ClientId", idClient);


                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ClientModel newClient = new ClientModel()
                            {
                                Id_klient = Convert.ToInt32(reader["Id_klient"]),
                                Imie = reader["Imie"].ToString(),
                                Nazwisko = reader["Nazwisko"].ToString(),
                                Numer_telefonu = reader["Nr_telefonu"].ToString(),
                            };
                            client.Add(newClient);
                        }
                    }
                }
                connection.Close();
            }
            return client;


        }
        public List<CarModel> getCars()
        {
            List<CarModel> cars = new List<CarModel>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "pokazSamochod";

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            CarModel car = new CarModel()
                            {
                                Id_samochod = Convert.ToInt32(reader["Id_samochod"]),
                                Marka = reader["Marka"].ToString(),
                                Model = reader["Model"].ToString(),
                                Nr_rejestracyjny = reader["Nr_rejestracyjny"].ToString(),
                                VIN = reader["Vin"].ToString(),
                                Rok = Convert.ToInt32(reader["Rok"]),
                                Miesiac = Convert.ToInt32(reader["Miesiac"]),
                                Moc = Convert.ToInt32(reader["Moc"]),
                                Paliwo = reader["Paliwo"].ToString(),
                            };
                            cars.Add(car);
                        }
                    }
                }
                connection.Close();
            }
            return cars;
        }

        public List<string> getAllFuel()
        {
            List<string> fuelType = new List<string>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "SELECT * FROM Paliwo";

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            fuelType.Add(reader["Nazwa"].ToString());
                        }
                    }
                }
                connection.Close();
            }
            return fuelType;
        }

        public List<CarModel> getCarById(int idCar)
        {
            List<CarModel> car = new List<CarModel>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "pokazSamochodById";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Samochod_Id", idCar);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            CarModel newCar = new CarModel()
                            {
                                Id_samochod = Convert.ToInt32(reader["Id_samochod"]),
                                Marka = reader["Marka"].ToString(),
                                Model = reader["Model"].ToString(),
                                Nr_rejestracyjny = reader["Nr_rejestracyjny"].ToString(),
                                VIN = reader["Vin"].ToString(),
                                Rok = Convert.ToInt32(reader["Rok"]),
                                Miesiac = Convert.ToInt32(reader["Miesiac"]),
                                Moc = Convert.ToInt32(reader["Moc"]),
                                Paliwo = reader["Paliwo"].ToString(),
                            };
                            car.Add(newCar);
                        }
                    }
                }
                connection.Close();
            }
            return car;
        }
        public List<WorkerModel> getWorkers()
        {
            List<WorkerModel> workers = new List<WorkerModel>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "SELECT * FROM Pracownik";

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            WorkerModel worker = new WorkerModel()
                            {
                                Id_pracownik = reader.GetInt32("Id_pracownik"),
                                Pracownik_imie = reader["Imie"].ToString(),
                                Pracownik_nazwisko = reader["Nazwisko"].ToString(),
                                Pracownik_nr_telefonu = reader["Nr_telefonu"].ToString(),
                                Pracownik_email = reader["Email"].ToString()
                            };
                            workers.Add(worker);
                        }
                    }
                }
            }

            return workers;
        }

        public WorkerModel getWorker(int id)
        {
            WorkerModel worker = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "SELECT * FROM widokPracownik WHERE Id_pracownik = @Id";
                    command.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            worker = new WorkerModel()
                            {
                                Id_pracownik = reader.GetInt32(reader.GetOrdinal("Id_pracownik")),
                                Pracownik_imie = reader["Imie"].ToString(),
                                Pracownik_nazwisko = reader["Nazwisko"].ToString(),
                                Pracownik_nr_telefonu = reader["Nr_telefonu"].ToString(),
                                Pracownik_email = reader["Email"].ToString(),
                                Pracownik_haslo = reader["Haslo"].ToString()
                                
                            };
                        }
                    }
                }
            }

            return worker;
        }
    }
}
