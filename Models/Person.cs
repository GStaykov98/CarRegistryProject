namespace CarRegistryProject.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int PhoneNumber { get; set; }

        public List<Car> Cars { get; set; } = new();
    }
}
