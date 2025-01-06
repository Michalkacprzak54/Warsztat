using Humanizer;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WarsztatMVC.Models
{
    public class VisitModel
    {
        [DisplayName("ID")]
        public int? Id_visit { get; set; }

        [DisplayName("Samochod")]
        public string? Samochod { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane.")]
        [DisplayName("Dane samochodu")]
        public string? Nr_rejestracyjny{ get; set; }

        [DisplayName("Firma")]
        public string? Firma { get; set; }

        //[Required(ErrorMessage = "To pole jest wymagane.")]
        [DisplayName("Dane firmy")]
        public long? nip { get; set; }

        [DisplayName("Klient")]
        public string? Klient { get; set; }
        [DisplayName("Dane klienta")]
        [Required(ErrorMessage = "To pole jest wymagane.")]
        public string? Numer_telefonu { get; set; }

        [DisplayName("Pracownik")]
        public string? Pracownik { get; set; }

        [DisplayName("Przebieg początkowy")]
        [Required(ErrorMessage = "To pole jest wymagane.")]
        public int? Przebieg_start { get; set; }

        [DisplayName("Data początkowa")]
        [Required(ErrorMessage = "To pole jest wymagane.")]
        public DateTime? Data_start { get; set; }

        [DisplayName("Data końcowa")]
        public DateTime? Data_koniec { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane.")]
        [DisplayName("Paliwo na początku")]
        [Range(0, 1, ErrorMessage = "Wartość musi być między 0 a 1")]
        [RegularExpression(@"^\d+(\.\d{0,2})?([,]\d{1,2})?$", ErrorMessage = "Dozwolone są liczby z maksymalnie dwoma miejscami po przecinku.")]
        [DisplayFormat(DataFormatString = "{0:0.##}", ApplyFormatInEditMode = true)]
        public double? Paliwo_start { get; set; }

        [DisplayName("Czy zakończona")]
        public bool? Czy_zakonczona { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane.")]
        [DisplayName("Opis klienta")]
        public string? Opis_klienta { get; set; }


        public VisitModel()
        {
          
        }

    }
}
