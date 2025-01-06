using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WarsztatMVC.Models
{
    public class CompanyModel
    {
        [DisplayName("ID")]
        public int? Id_frima { get; set; }


        [Required(ErrorMessage = "To pole jest wymagane.")]
        [DisplayName("Nazwa firmy")]
        [StringLength(50, ErrorMessage = "Nazwa firmy nie może być dłuższa niż 50 znaków")]
        public string? nazwa_firma { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane.")]
        [DisplayName("Miasto")]
        [StringLength(50, ErrorMessage = "Nazwa miasta nie może być dłuższa niż 50 znaków")]
        [RegularExpression(@"^[a-zA-ZęóąśłżźćńĘÓĄŚŁŻŹĆŃ\s-]+$", ErrorMessage = "Nazwa miasta może zawierać tylko litery, spacje i myślniki.")]
        public string? miasto { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane.")]
        [DisplayName("Kod pocztowy")]
        [StringLength(10, ErrorMessage = "Kod pocztowy nie może być dłuższy niż 10 znaków")]
        [RegularExpression("^[0-9-]+$", ErrorMessage = "Kod pocztowy może zawierać tylko cyfry i znak '-'.")]
        public string? kod_pocztowy { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane.")]
        [DisplayName("Ulica")]
        [StringLength(50, ErrorMessage = "Nazwa ulicy nie może być dłuższa niż 50 znaków")]
        [RegularExpression(@"^[a-zA-ZęóąśłżźćńĘÓĄŚŁŻŹĆŃ\s-]+$", ErrorMessage = "Nazwa ulicy może zawierać tylko litery, spacje i myślniki.")]
        public string? ulica { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane.")]
        [DisplayName("Numer domu")]
        [RegularExpression("^[0-9A-Za-z]+$", ErrorMessage = "Numer domu może zawierać tylko litery i/lub cyfry.")]
        public string? numer_domu { get; set; }

        [DisplayName("Numer lokalu")]
        public int? numer_lokalu { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane.")]
        [DisplayName("NIP")]
        [RegularExpression("^[0-9]{10}$", ErrorMessage = "NIP musi składać się z 10 cyfr.")]
        public long? nip { get; set; }


        [Required(ErrorMessage = "To pole jest wymagane.")]
        [DisplayName("REGON")]
        [RegularExpression("^[0-9]{9}$", ErrorMessage = "REGON musi składać się z 9 cyfr.")]
        public long? regon { get; set; }

        public string? NazwaNipRegon => $"{nazwa_firma}  {nip} {regon}";

    }

}
