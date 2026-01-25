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
    public class CarService
    {
        public Car AddCar(int personId, string brand, string model, string registrationNumber, int carTicketNumber)
        {
            brand = (brand ?? "").Trim();
            model = (model ?? "").Trim();
            registrationNumber = (registrationNumber ?? "").Trim();

            if (brand.Length == 0)
            {
                throw new ArgumentException("Make is required.");
            }

            if (model.Length == 0)
            {
                throw new ArgumentException("Model is required.");
            }

            if (registrationNumber.Length == 0)
            {
                throw new ArgumentException("Registration number is required.");
            }

            using AppDbContext db = new AppDbContext();

            if (!db.People.Any(p => p.Id == personId))
            {
                throw new InvalidOperationException("Person not found.");
            }

            if (db.Cars.Any(c => c.RegistrationNumber == registrationNumber))
            {
                throw new InvalidOperationException("Car with this registration number already exists.");
            }

            Car car = new Car
            {
                PersonId = personId,
                Brand = brand,
                Model = model,
                RegistrationNumber = registrationNumber,
                CarTicketNumber = carTicketNumber
            };

            db.Cars.Add(car);
            db.SaveChanges();
            return car;
        }

        public Car? GetCarById(int id)
        {
            using var db = new AppDbContext();

            return db.Cars
                .Include(c => c.Insurance)
                .Include(c => c.ComprehensiveInsurance)
                .FirstOrDefault(c => c.Id == id);
        }
    }
}
