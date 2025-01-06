using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;

namespace WarsztatMVC.Models
{
    public class ActivityModel
    {
        public int Id_czynnosc { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane.")]
        [StringLength(50, ErrorMessage = "Bląd")]
        public string Nazwa { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Cena netto musi być większa niż 0.")]
        public decimal? Cena_netto { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Roboczogodziny muszą być większe niż 0.")]
        public decimal? Roboczogodziny { get; set; }

        public bool? IsLinkedToRepair { get; set; }
    }
}
