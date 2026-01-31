using System.Configuration;
using System.Data;
using System.Windows;
using CarRegistryProject.Data;

namespace CarRegistryProject
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            using var db = new AppDbContext();
            db.Database.EnsureCreated();

            var today = DateOnly.FromDateTime(DateTime.Today);

            var expired = db.Insurances.Where(i => i.EndDate < today).ToList();
        }
    }

}
