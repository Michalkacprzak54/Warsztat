using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;


namespace WarsztatMVC.Models
{
    public class RepairModel
    {
        public IEnumerable<ActivityModel> ActionViewModel { get; set; }
        public IEnumerable<PartModel> PartViewModel { get; set; }
        public IEnumerable<VisitModel> VisitViewModel { get; set; }
        public IEnumerable<FixModel> FixViewModel{ get; set; }

    }
}
