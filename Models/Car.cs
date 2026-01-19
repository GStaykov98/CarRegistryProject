using CarRegistryProject.Models.Enums;

namespace CarRegistryProject.Models
{
    public class Car
    {
        public string Brand { get; set; } = "";
        public string Model { get; set; } = "";
        public string RegistrationNumber { get; set; } = "";
        public int CarTicketNumber { get; set; }
        public int PolicyNumber { get; set; }
        public DateOnly InsuranceStartDate { get; set; }
        public DateOnly InsuranceEndDate { get; set; }
        public InstallmentPlan InstallmentPlan { get; set; } = InstallmentPlan.Yearly;
    }
}
