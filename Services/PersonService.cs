using CarRegistryProject.Data;
using CarRegistryProject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRegistryProject.Services
{
    public class PersonService
    {
        public Person CreatePerson(string name, string governmentId, string phoneNumber)
        {
            name = (name ?? "").Trim();
            governmentId = (governmentId ?? "").Trim();
            phoneNumber = (phoneNumber ?? "").Trim();

            if (name.Length == 0) throw new ArgumentException("Name is required.");
            if (governmentId.Length == 0) throw new ArgumentException("ID is required.");
            if (phoneNumber.Length == 0) throw new ArgumentException("Phone number is required.");

            using var db = new AppDbContext();

            if (db.People.Any(p => p.GovernmentId == governmentId))
            {
                throw new InvalidOperationException("Person with that ID already exists.");
            }

            var person = new Person
            {
                Name = name,
                GovernmentId = governmentId,
                PhoneNumber = phoneNumber
            };

            db.People.Add(person);
            db.SaveChanges();
            return person;
        }

        public List<Person> SearchPeople(string query)
        {
            query = (query ?? "").Trim();

            using var db = new AppDbContext();

            var peopleQuery = db.People
                .Include(p => p.Cars)
                .ThenInclude(c => c.Insurance)
                .Include(p => p.Cars)
                .ThenInclude(c => c.ComprehensiveInsurance)
                .AsQueryable();

            if (query.Length == 0)
            {
                return peopleQuery.OrderBy(p => p.Name).ToList();
            }

            return peopleQuery
                .Where(p =>
                    EF.Functions.Like(p.Name, $"%{query}%") ||
                    EF.Functions.Like(p.GovernmentId, $"%{query}%"))
                .OrderBy(p => p.Name)
                .ToList();
        }

        public Person? GetPersonById(int id)
        {
            using var db = new AppDbContext();

            return db.People
                .Include (p => p.Cars)
                .ThenInclude (c => c.Insurance)
                .Include (p => p.Cars)
                .ThenInclude(c => c.ComprehensiveInsurance)
                .FirstOrDefault(p => p.Id == id);
        }
    }
        
}
