using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarRegistryProject.Models.Enums;

namespace CarRegistryProject.Models
{
    public class Insurance
    {
        public int Id { get; set; }

        public int CarId { get; set; }
        public Car Car { get; set; }

        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public InstallmentPlan InstallmentPlan { get; set; }
        public string PolicyNumber { get; set; } = "";
    }
}
