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

            LoadCar();
        }

        private void LoadCar()
        {
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
                InsuranceEndDatePicker.SelectedDate = null;
                InsurancePolicyNumberBox.Text = "";
                InstallmentPlanBox.SelectedIndex = 0;
            }
            else
            {
                InsuranceText.Text = $"Start: {_car.Insurance.StartDate}\nEnd: {_car.Insurance.EndDate}\nPlan: {_car.Insurance.InstallmentPlan}";

                InsuranceStartDatePicker.SelectedDate = _car.Insurance.StartDate.ToDateTime(TimeOnly.MinValue);

                InsuranceEndDatePicker.SelectedDate = _car.Insurance.EndDate.ToDateTime(TimeOnly.MinValue);

                InstallmentPlanBox.SelectedItem = _car.Insurance.InstallmentPlan;

                InsurancePolicyNumberBox.Text = _car.Insurance.PolicyNumber;
            }
        }

        private void CreateOrUpdateInsurance_Click(object sender, RoutedEventArgs e)
        {
            if (_car == null) return;

            if (InsuranceStartDatePicker.SelectedDate == null ||
                InsuranceEndDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Select start and end dates.");
                return;
            }

            var start = DateOnly.FromDateTime(InsuranceStartDatePicker.SelectedDate.Value);
            var end = DateOnly.FromDateTime(InsuranceEndDatePicker.SelectedDate.Value);
            var plan = (InstallmentPlan)InstallmentPlanBox.SelectedItem;
            var policyNumber = (InsurancePolicyNumberBox.Text ?? "").Trim();

            if (policyNumber.Length == 0)
            {
                MessageBox.Show("Policy number is required.");
                return;
            }

            try
            {
                if (_car.Insurance == null)
                {
                    _insuranceService.CreateInsurance(_carId, start, end, plan, policyNumber);
                }
                else
                {
                    _insuranceService.UpdateInsurance(_carId, start, end, plan, policyNumber);
                }

                LoadCar();
                MessageBox.Show("Insurance saved.");
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
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
