using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WarsztatMVC.Models
{
    public class WorkerLoginModel
    {
        public int Id_pracownik_login { get; set; }

        public int Id_pracownik { get; set; }

        [Required(ErrorMessage = "Adres email jest wymagany")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string email { get; set; }

        [Required(ErrorMessage = "Hasło jest wymagane")]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string password { get; set; }

        public bool isAdmin { get; set; }
        public WorkerLoginModel() 
        {

        }

    }
}
