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

        public void ApplyExpiryAndRollover(int carId)
        {
            using var db = new AppDbContext();
            var today = DateOnly.FromDateTime(DateTime.Today);

            var insurance = db.Insurances.FirstOrDefault(i => i.CarId == carId);
            if (insurance == null) return;

            if (insurance.EndDate >= today) return;

            if (insurance.HasRenewal &&
                insurance.FutureStartDate.HasValue &&
                insurance.FutureStartDate.Value <= today &&
                !string.IsNullOrWhiteSpace(insurance.FuturePolicyNumber) &&
                insurance.FutureInstallmentPlan.HasValue)
            {
                insurance.StartDate = insurance.FutureStartDate.Value;
                insurance.EndDate = insurance.StartDate.AddYears(1).AddDays(-1);
                insurance.PolicyNumber = insurance.FuturePolicyNumber;
                insurance.InstallmentPlan = insurance.FutureInstallmentPlan.Value;

                insurance.HasRenewal = false;
                insurance.FutureStartDate = null;
                insurance.FuturePolicyNumber = null;
                insurance.FutureInstallmentPlan = null;

                db.SaveChanges();
                return;
            }

            db.Insurances.Remove(insurance);
            db.SaveChanges();
        }

        public void ScheduleRenewal(int carId, DateOnly futureStart, string futurePolicyNumber, InstallmentPlan futureInstallmentPlan)
        {
            futurePolicyNumber = (futurePolicyNumber ?? "").Trim();
            if (futurePolicyNumber.Length == 0)
            {
                throw new ArgumentException("Future policy number is required.");
            }

            using var db = new AppDbContext();

            var insurance = db.Insurances.FirstOrDefault(x => x.CarId == carId);
            if (insurance == null)
            {
                throw new InvalidOperationException("No current insurance exists for this car.");
            }

            if (futureStart <= insurance.EndDate)
            {
                throw new InvalidProgramException("Future start date must be after the currend end date.");
            }

            insurance.HasRenewal = true;
            insurance.FutureStartDate = futureStart;
            insurance.FuturePolicyNumber = futurePolicyNumber;
            insurance.FutureInstallmentPlan = futureInstallmentPlan;

            db.SaveChanges();
        }

        public void CancelScheduledRenewal(int _carId)
        {
            using var db = new AppDbContext();

            var insurance = db.Insurances.FirstOrDefault(x => x.CarId == _carId);
            if (insurance == null) return;

            insurance.HasRenewal = false;
            insurance.FutureStartDate = null;
            insurance.FuturePolicyNumber = null;
            insurance.FutureInstallmentPlan = 0;

            db.SaveChanges();
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
