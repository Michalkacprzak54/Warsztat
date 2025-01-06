using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WarsztatMVC.Models;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.AspNetCore.Authorization;

namespace WarsztatMVC.Controllers
{

    [AllowAnonymous]
    public class WorkerLoginController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string? _connectionString;

        public WorkerLoginController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetSection("ConnectionString")["Polaczenie"];
        }

        [HttpGet]
        public ActionResult Index ()
        {
            if (TempData["WorkerLogin"] != null)
            {
                string message = (string)TempData["user"];
                ViewData["Message"] = message;

            }
            if (Request.Cookies["CookieUserID"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }


        [HttpPost]
        public ActionResult Index(WorkerLoginModel user)
        {
            if(ModelState.IsValid)
            {
                MD5 md5 = MD5.Create();
                var byte_data = md5.ComputeHash(Encoding.UTF8.GetBytes(HttpContext.Request.Form["password"]));
                string password = Convert.ToHexString(byte_data);
                string email = HttpContext.Request.Form["email"];

                user = LoginService(email, password.ToLower());

                if(user !=  null)
                {
                    CookieOptions options = new CookieOptions();
                    options.Expires = DateTime.Now.AddMinutes(60);
                    Response.Cookies.Append("CookieUserID", user.Id_pracownik.ToString(), options);
                    Response.Cookies.Append("CookieIsAdmin", user.isAdmin.ToString(), options);
                    Response.Cookies.Append("CookieWorkerEmail", user.email.ToString(), options);
                }
                else
                {
                    string data = "Logowanie nie powiodło się!";
                    TempData["WorkerLogin"] = data;
                    
                }
                return RedirectToAction("Index");

            }
            else
            {
                Debug.WriteLine(ModelState.Values.SelectMany(v => v.Errors));
            }
            return View();
        }

        public ActionResult Logout()
        {
            Response.Cookies.Delete("CookieUserID");
            Response.Cookies.Delete("CookieIsAdmin");
            Response.Cookies.Delete("CookieWorkerEmail");
            return RedirectToAction("Index");

        }


        private WorkerLoginModel LoginService(string email, string password)
        {
            WorkerLoginModel user;

            using(SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using(SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "SELECT * FROM pracownikLogin WHERE Email = @email AND Haslo = @password;";
                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@password", password);
                    using(SqlDataReader reader = command.ExecuteReader())
                    {
                        if(reader.Read())
                        {
                            user = new WorkerLoginModel()
                            {
                                Id_pracownik_login = Convert.ToInt32(reader["ID"]),
                                Id_pracownik = Convert.ToInt32(reader["Id_pracownik"]),
                                email = reader["Email"].ToString(),
                                password = reader["Haslo"].ToString(),
                                isAdmin = Convert.ToBoolean(reader["czy_admin"])
                            };
                        }
                        else
                        {
                            user = null; 
                        }
                    }

                }

                connection.Close();

            }
            return user;
        }

       
    }
}
