using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace WarsztatMVC.Models
{
    public class DataRepairModel
    {
        public int? Id_naprawa {  get; set; }
        public string? Czynnosc {  get; set; }
        public string? Czesc {  get; set; }
        public int? Id_wizyta { get; set; }
        public string? Pracownik {  get; set; }


        [Required(ErrorMessage = "To pole jest wymagane.")]
        public string? Uwagi {  get; set; }

    }
}
