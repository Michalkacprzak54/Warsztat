using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;

namespace WarsztatMVC.Models
{
    public class PartModel
    {
        public int? Id_czesc {  get; set; }

        //[Required(ErrorMessage = "To pole jest wymagane.")]
        [StringLength(50, ErrorMessage = "Bląd")]
        public string? Czynnosc { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane.")]
        [StringLength(50, ErrorMessage = "Bląd")]
        public string? Czesc { get; set; }

        //[Required(ErrorMessage = "To pole jest wymagane.")]
        [StringLength(50, ErrorMessage = "Bląd")]
        public string? Producent { get; set; }

        //[Required(ErrorMessage = "To pole jest wymagane.")]
        [StringLength(50, ErrorMessage = "Bląd")]
        public string? Nr_czesc { get; set; }

        //[Required(ErrorMessage = "To pole jest wymagane.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Cena netto musi być większa niż 0.")]
        public decimal? Cena_netto { get; set; }

        public bool? IsLinkedToRepair { get; set; }


    }
}
