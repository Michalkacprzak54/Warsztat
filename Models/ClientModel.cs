using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WarsztatMVC.Models
{
    public class ClientModel
    {

        [DisplayName("ID")]
        public int Id_klient { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane.")]
        [StringLength(50, ErrorMessage = "Bląd")]
        [DisplayName("Imię")]
        [RegularExpression(@"^[a-zA-ZęóąśłżźćńĘÓĄŚŁŻŹĆŃ]+$", ErrorMessage = "Imię może zawierać tylko litery.")]
        public string Imie { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane.")]
        [StringLength(50, ErrorMessage = "Bląd")]
        [DisplayName("Nazwisko")]
        [RegularExpression(@"^[a-zA-ZęóąśłżźćńĘÓĄŚŁŻŹĆŃ-]+$", ErrorMessage = "Nazwisko może zawierać tylko litery.")]
        public string Nazwisko { get; set; }
        [Required(ErrorMessage = "To pole jest wymagane.")]
        [StringLength(12, MinimumLength = 9, ErrorMessage = "Numer telefonu musi mieć 9 liczb")]
        [DisplayName("Numer telefonu")]
        [RegularExpression("^[0-9\\s-/]+$", ErrorMessage = "Numer telefonu może zawierać tylko cyfry, spację, znak '-' i '/'.")]
        public string Numer_telefonu { get; set; }

        public string? ImieNazwiskoNr_tel => $"{Imie} {Nazwisko} {Numer_telefonu}";

        public ClientModel()
        {

        }
    }
}
