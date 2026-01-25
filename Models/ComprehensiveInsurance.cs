using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRegistryProject.Models
{
    public class ComprehensiveInsurance
    {
        public int Id { get; set; }

        public int CarId { get; set; }
        public Car Car { get; set; }

        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string PolicyNumber { get; set; } = "";
    }
}
