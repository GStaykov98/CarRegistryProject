using CarRegistryProject.Data;
using CarRegistryProject.Models;
using CarRegistryProject.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRegistryProject.Services
{
    public class ComprehensiveInsuranceService
    {
        public ComprehensiveInsurance CreateInsurance(int carId, DateOnly startDate, DateOnly endDate, string policyNumber)
        {
            policyNumber = (policyNumber ?? "").Trim();

            if (endDate < startDate)
            {
                throw new ArgumentException("End date cannot be before start date.");
            }

            using var db = new AppDbContext();

            var car = db.Cars
                .Include(i => i.ComprehensiveInsurance)
                .FirstOrDefault(c => c.Id == carId);

            if (car == null)
            {
                throw new InvalidOperationException("Car not found.");
            }

            if (car.ComprehensiveInsurance != null)
            {
                throw new InvalidOperationException("Car already has comprehensive insurance.");
            }

            var insurance = new ComprehensiveInsurance
            {
                CarId = carId,
                StartDate = startDate,
                EndDate = endDate,
                PolicyNumber = policyNumber
            };

            db.ComprehensiveInsurances.Add(insurance);
            db.SaveChanges();

            return insurance;
        }

        public ComprehensiveInsurance UpdateInsurance(int carId, DateOnly startDate, DateOnly endDate, string policyNumber)
        {
            policyNumber = (policyNumber ?? "").Trim();

            if (endDate < startDate)
            {
                throw new ArgumentException("End date cannot be before start date.");
            }

            using var db = new AppDbContext();

            var insurance = db.ComprehensiveInsurances.FirstOrDefault(i => i.CarId == carId);
            if (insurance == null)
            {
                throw new InvalidOperationException("Insurance not found.");
            }

            insurance.StartDate = startDate;
            insurance.EndDate = endDate;
            insurance.PolicyNumber = policyNumber;

            db.SaveChanges();

            return insurance;
        }

        public void RemoveInsurance(int carId)
        {
            using var db = new AppDbContext();

            var insurance = db.ComprehensiveInsurances.FirstOrDefault(i => i.CarId == carId);
            if (insurance == null) return;

            db.ComprehensiveInsurances.Remove(insurance);
            db.SaveChanges();
        }

        public bool IsInsuranceActive(int carId)
        {
            using var db = new AppDbContext();

            var insurance = db.ComprehensiveInsurances
                .AsNoTracking()
                .FirstOrDefault(c => c.CarId == carId);

            if (insurance == null) return false;

            var today = DateOnly.FromDateTime(DateTime.Now);
            return insurance.StartDate <= today && today <= insurance.EndDate;
        }
    }
}
