using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace WarsztatMVC.Models
{
    public class WorkerModel
    {

        public int Id_pracownik { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane.")]
        [StringLength(50, ErrorMessage = "Bląd")]
        [DisplayName("Imię")]
        [RegularExpression(@"^[a-zA-ZęóąśłżźćńĘÓĄŚŁŻŹĆŃ]+$", ErrorMessage = "Imię może zawierać tylko litery.")]
        public string Pracownik_imie { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane.")]
        [StringLength(50, ErrorMessage = "Bląd")]
        [DisplayName("Nazwisko")]
        [RegularExpression(@"^[a-zA-ZęóąśłżźćńĘÓĄŚŁŻŹĆŃ-]+$", ErrorMessage = "Nazwisko może zawierać tylko litery.")]
        public string Pracownik_nazwisko { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane.")]
        [StringLength(12, MinimumLength = 9, ErrorMessage = "Numer telefonu musi mieć 9 liczb")]
        [DisplayName("Numer telefonu")]
        [RegularExpression("^[0-9\\s-/]+$", ErrorMessage = "Numer telefonu może zawierać tylko cyfry, spację, znak '-' i '/'.")]
        public string Pracownik_nr_telefonu { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane.")]
        [EmailAddress(ErrorMessage = "Podaj poprawny adres e-mail.")]
        public string Pracownik_email { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane.")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "Hasło musi mieć od 1 do 20 znaków.")]
        public string? Pracownik_haslo {  get; set; }
    }
}
