namespace CarRegistryProject.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string GovernmentId { get; set; } = "";
        public string PhoneNumber { get; set; } = "";

        public List<Car> Cars { get; set; } = new();
    }
}
