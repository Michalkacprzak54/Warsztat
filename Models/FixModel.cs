using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;


namespace WarsztatMVC.Models
{
    public class FixModel
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "To pole jest wymagane.")]
        public int? Id_czynnosc {  get; set; }
        public string? Czynnosc { get; set; }
        public string? Nr_rejestracyjny { get; set; }
        public int? Id_wizyta { get; set; }
        public string? Samochod { get; set; }
        public string? Czesc { get; set; }
        [Required(ErrorMessage = "To pole jest wymagane.")]
        public int? Id_czesc { get; set; }
        public string? Pracownik { get; set; }

        //[Required]
        public int? Id_pracownik { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane.")]
        public string? Uwagi { get; set; }

    }
}
