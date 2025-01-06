using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WarsztatMVC.Models
{
    public class RangeYearAttribute : RangeAttribute
    {
        public RangeYearAttribute(int startYear) : base(startYear, DateTime.Now.Year)
        {
            ErrorMessage = "Nieprawidłowy rok";
        }
    }
    public class CarModel
    {
        [DisplayName("ID")]
        public int Id_samochod { get; set; }

        [DisplayName("Marka")]
        [Required(ErrorMessage = "To pole jest wymagane.")]
        [StringLength(50, ErrorMessage = "Marka nie może być dłuższa niż 50 znaków.")]
        [RegularExpression(@"^[a-zA-ZęóąśłżźćńĘÓĄŚŁŻŹĆŃ0-9\s]+$", ErrorMessage = "Marka może zawierać tylko litery i/lub cyfry.")]
        public string Marka { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane.")]
        [DisplayName("Model")]
        [StringLength(50, ErrorMessage = "Model nie może być dłuższy niż 50 znaków.")]
        [RegularExpression(@"^[a-zA-ZęóąśłżźćńĘÓĄŚŁŻŹĆŃ0-9\s]+$", ErrorMessage = "Model może zawierać tylko litery i/lub cyfry.")]
        public string Model { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane.")]
        [DisplayName("Numer rejestracyjny")]
        [StringLength(10, ErrorMessage = "Numer rejestracyjny może mieć max 10 znaków.")]
        [RegularExpression("^[0-9A-Za-z ]+$", ErrorMessage = "Numer rejestracyjny może zawierać tylko litery i/lub cyfry.")]
        public string Nr_rejestracyjny { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane.")]
        [DisplayName("Numer VIN")]
        [RegularExpression("^[0-9A-Za-z]+$", ErrorMessage = "Numer VIN może zawierać tylko litery i/lub cyfry.")]
        [StringLength(17, MinimumLength = 17, ErrorMessage = "Vin musi mieć 17 znaków.")]

        public string VIN { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane.")]
        [DisplayName("Rok produkcji")]
        [RangeYear(1900)]
        public int Rok { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane.")]
        [DisplayName("Miesiąc produkcji")]
        [Range(1, 12, ErrorMessage = "Nieprawidłowy miesiąc")]
        public int Miesiac { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane.")]
        [DisplayName("Moc")]
        [Range(1, 2500, ErrorMessage = "Nie można wpisać liczby ujemnej")]
        public int Moc {  get; set; }

        [Required(ErrorMessage = "To pole jest wymagane.")]
        [DisplayName("Paliwo")]
        
        public string Paliwo { get; set; }


        public string? MarkaModelNr_rejestracyjny => $"{Marka} {Model} - {Nr_rejestracyjny}";

        public CarModel()
        {

        }

    }
}
