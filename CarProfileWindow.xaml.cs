using System.Windows;
using CarRegistryProject.Models;
using CarRegistryProject.Models.Enums;
using CarRegistryProject.Services;

namespace CarRegistryProject
{
    /// <summary>
    /// Interaction logic for CarProfileWindow.xaml
    /// </summary>
    public partial class CarProfileWindow : Window
    {
        private readonly int _carId;
        private readonly CarService _carService = new CarService();
        private readonly InsuranceService _insuranceService = new InsuranceService();
        private Car? _car;
        public CarProfileWindow(int carId)
        {
            InitializeComponent();
            _carId = carId;

            InstallmentPlanBox.ItemsSource = Enum.GetValues(typeof(InstallmentPlan));
            InstallmentPlanBox.SelectedIndex = 0;

            FutureInstallmentPlanBox.ItemsSource = Enum.GetValues(typeof(InstallmentPlan));
            FutureInstallmentPlanBox.SelectedIndex = 0;

            LoadCar();
        }

        private void LoadCar()
        {
            _insuranceService.ApplyExpiryAndRollover(_carId);
            _car = _carService.GetCarById(_carId);

            if (_car == null)
            {
                MessageBox.Show("Car not found.");
                Close();
                return;
            }

            Title = $"{_car.Brand} {_car.Model} ({_car.RegistrationNumber})";
            CarHeaderText.Text = $"{_car.Brand} {_car.Model}";
            CarInfoText.Text = $"Reg. No: {_car.RegistrationNumber} | Ticket #: {_car.CarTicketNumber}";

            if (_car.Insurance == null)
            {
                InsuranceText.Text = "No insurance";
                InsuranceStartDatePicker.SelectedDate = null;
                InsurancePolicyNumberBox.Text = "";
                InstallmentPlanBox.SelectedIndex = 0;
            }
            else
            {
                InsuranceText.Text = $"Start: {_car.Insurance.StartDate}\nEnd: {_car.Insurance.EndDate}\nPlan: {_car.Insurance.InstallmentPlan}" +
                    $"\nPolicy number: {_car.Insurance.PolicyNumber}";

                InsuranceStartDatePicker.SelectedDate = null;
                InstallmentPlanBox.SelectedIndex = 0;
                InsurancePolicyNumberBox.Text = "";
            }

            if (_car.Insurance == null || !_car.Insurance.HasRenewal)
            {
                FutureStartDatePicker.SelectedDate = null;
                FuturePolicyNumberBox.Text = "";
                FutureInstallmentPlanBox.SelectedIndex = 0;
            }
            else
            {
                if (_car.Insurance.FutureStartDate.HasValue)
                {
                    FutureStartDatePicker.SelectedDate = _car.Insurance.FutureStartDate.Value.ToDateTime(TimeOnly.MinValue);
                }
                else
                {
                    FutureStartDatePicker.SelectedDate= null;
                }

                FuturePolicyNumberBox.Text = _car.Insurance.FuturePolicyNumber ?? "";

                if (_car.Insurance.FutureInstallmentPlan.HasValue)
                {
                    FutureInstallmentPlanBox.SelectedItem = _car.Insurance.FutureInstallmentPlan.Value;
                }
                else
                {
                    FutureInstallmentPlanBox.SelectedIndex = 0;
                }
            }

            bool hasInsurance = _car.Insurance != null;

            InsuranceStartDatePicker.IsEnabled = !hasInsurance;
            InsurancePolicyNumberBox.IsEnabled = !hasInsurance;
            InstallmentPlanBox.IsEnabled = !hasInsurance;

            bool hasScheduledRenewal = _car?.Insurance != null && _car.Insurance.HasRenewal;

            FutureStartDatePicker.IsEnabled = !hasScheduledRenewal;
            FuturePolicyNumberBox.IsEnabled = !hasScheduledRenewal;
            FutureInstallmentPlanBox.IsEnabled = !hasScheduledRenewal;
        }

        private void CreateInsurance_Click(object sender, RoutedEventArgs e)
        {
            if (_car == null) return;

            if (_car.Insurance != null)
            {
                MessageBox.Show("Insurance already exists. Use scheduled renewal for changes.");
                return;
            }

            if (InsuranceStartDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Select start date.");
                return;
            }

            var start = DateOnly.FromDateTime(InsuranceStartDatePicker.SelectedDate.Value);
            var plan = (InstallmentPlan)InstallmentPlanBox.SelectedItem;
            var policyNumber = (InsurancePolicyNumberBox.Text ?? "").Trim();

            if (policyNumber.Length == 0)
            {
                MessageBox.Show("Policy number is required.");
                return;
            }

            try
            {
                _insuranceService.CreateInsurance(_carId, start, plan, policyNumber);

                LoadCar();
                MessageBox.Show("Insurance saved.");
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void SaveScheduledRenewal_Click(object sender, RoutedEventArgs e)
        {
            if (_car == null || _car.Insurance == null)
            {
                MessageBox.Show("Create a current insurance first.");
                return;
            }

            if (FutureStartDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Select future start date.");
                return;
            }

            if (_car.Insurance.HasRenewal)
            {
                MessageBox.Show("Scheduled renewal already exists. Cancel it first to make changes.");
                return;
            }

            var futureStart = DateOnly.FromDateTime(FutureStartDatePicker.SelectedDate.Value);
            var futurePolicy = (FuturePolicyNumberBox.Text ?? "").Trim();
            var futurePlan = (InstallmentPlan)FutureInstallmentPlanBox.SelectedItem;

            if (futurePolicy.Length == 0)
            {
                MessageBox.Show("Future policy number is required.");
                return;
            }

            try
            {
                _insuranceService.ScheduleRenewal(_carId, futureStart, futurePolicy, futurePlan);
                LoadCar();
                MessageBox.Show("Scheduled renewal saved.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CancelScheduledRenewal_Click(object sender, RoutedEventArgs e)
        {
            if (_car == null || _car.Insurance == null) return;

            _insuranceService.CancelScheduledRenewal(_carId);
            LoadCar();
        }

        private void RemoveInsurance_Click(object sender, RoutedEventArgs e)
        {
            if (_car == null) return;

            if (MessageBox.Show("Remove insurance?", "Confirm", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            _insuranceService.RemoveInsurance(_car.Id);
            LoadCar();
        }
    }
}
