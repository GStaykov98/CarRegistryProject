using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarRegistryProject.Data;
using CarRegistryProject.Models;
using CarRegistryProject.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace CarRegistryProject.Services
{
    public class InsuranceService
    {
        public Insurance CreateInsurance(int carId, DateOnly startDate, DateOnly endDate, InstallmentPlan installmentPlan, string policyNumber)
        {
            policyNumber = (policyNumber ?? "").Trim();

            if (endDate < startDate)
            {
                throw new ArgumentException("End date cannot be before start date.");
            }

            if (policyNumber.Length == 0)
            {
                throw new ArgumentException("Policy number is required.");
            }

            using var db = new AppDbContext();

            var car = db.Cars
                .Include(i => i.Insurance)
                .FirstOrDefault(c => c.Id == carId);

            if (car == null)
            {
                throw new InvalidOperationException("Car not found.");
            }

            if (car.Insurance != null)
            {
                throw new InvalidOperationException("Car already has insurance.");
            }

            var insurance = new Insurance
            {
                CarId = carId,
                StartDate = startDate,
                EndDate = endDate,
                InstallmentPlan = installmentPlan,
                PolicyNumber = policyNumber
            };

            db.Insurances.Add(insurance);
            db.SaveChanges();

            return insurance;
        }

        public Insurance UpdateInsurance(int carId, DateOnly startDate, DateOnly endDate, InstallmentPlan installmentPlan, string policyNumber)
        {
            policyNumber = (policyNumber ?? "").Trim();

            if (endDate < startDate)
            {
                throw new ArgumentException("End date cannot be before start date.");
            }

            using var db = new AppDbContext();

            var insurance = db.Insurances.FirstOrDefault(i => i.CarId == carId);
            if (insurance == null)
            {
                throw new InvalidOperationException("Insurance not found.");
            }

            insurance.StartDate = startDate;
            insurance.EndDate = endDate;
            insurance.InstallmentPlan = installmentPlan;
            insurance.PolicyNumber = policyNumber;

            db.SaveChanges();

            return insurance;
        }

        public void RemoveInsurance(int carId)
        {
            using var db = new AppDbContext();

            var insurance = db.Insurances.FirstOrDefault(i => i.CarId == carId);
            if (insurance == null)
            {
                return;
            }

            db.Insurances.Remove(insurance);
            db.SaveChanges();
        }

        public bool IsInsuranceActive(int carId)
        {
            using var db = new AppDbContext();

            var insurance = db.Insurances
                .AsNoTracking()
                .FirstOrDefault(c => c.CarId == carId);

            if (insurance == null) return false;

            var today = DateOnly.FromDateTime(DateTime.Now);
            return insurance.StartDate <= today && today <= insurance.EndDate;
        }
    }
}
