using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace WarsztatMVC.Models
{
    public class AppointmentModel
    {
        public IEnumerable<CarModel> CarViewModel { get; set; }
        public IEnumerable<ClientModel> ClientViewModel { get; set; }
        public IEnumerable<CompanyModel> CompanyViewModel { get; set; }
        public IEnumerable<VisitModel> VisitViewModel { get; set; }
        public IEnumerable<DataRepairModel> DataRepairViewModel { get; set; }

    }
}
