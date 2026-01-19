using CarRegistryProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRegistryProject.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Person> People => Set<Person>();
        public DbSet<Car> Cars => Set<Car>();
        public DbSet<Insurance> Insurances => Set<Insurance>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dataFolder = Path.Combine(AppContext.BaseDirectory, "AppData");
            Directory.CreateDirectory(dataFolder);

            var dbPath = Path.Combine(dataFolder, "app.db");
            optionsBuilder.UseSqlite($"Data Source ={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>()
                .HasMany(p => p.Cars)
                .WithOne(c => c.Person!)
                .HasForeignKey(c => c.PersonId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Car>()
                .HasOne(c => c.Insurance)
                .WithOne(i => i.Car)
                .HasForeignKey<Insurance>(i => i.CarId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Person>()
                .HasIndex(p => p.GovernmentId)
                .IsUnique();

            modelBuilder.Entity<Car>()
                .HasIndex(c => c.RegistrationNumber)
                .IsUnique();

            modelBuilder.Entity<Insurance>()
                .HasIndex(i => i.CarId)
                .IsUnique();

            var dateOnlyConverter = new ValueConverter<DateOnly, string>(
                d => d.ToString("yyyy-MM-dd"),
                s => DateOnly.ParseExact(s, "yyyy-MM-dd")
                );

            modelBuilder.Entity<Insurance>()
                .Property(i => i.StartDate)
                .HasConversion(dateOnlyConverter);

            modelBuilder.Entity<Insurance>()
                .Property(i => i.EndDate)
                .HasConversion(dateOnlyConverter);

        }


    }
}
