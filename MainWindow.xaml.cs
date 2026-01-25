using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CarRegistryProject.Services;
using CarRegistryProject.Models;
using System.Collections.Generic;

namespace CarRegistryProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly PersonService _personService = new PersonService();
        private readonly CarService _carService = new CarService();
        private Person? _selectedPerson;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            var results = _personService.SearchPeople(SearchBox.Text);
            PeopleGrid.ItemsSource = results;
        }

        private void PeopleGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PeopleGrid.SelectedItem is not Person p) return;

            _selectedPerson = _personService.GetPersonById(p.Id);

            if (_selectedPerson != null)
            {
                this.Title = $"Selected: {_selectedPerson.Name} ({_selectedPerson.GovernmentId})";
            }
        }

        private void AddTestPerson_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _personService.CreatePerson(
                    "John Test",
                    DateTime.Now.Ticks.ToString(),
                    "1234569"
                    );

                MessageBox.Show("Test person added.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void PeopleGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (PeopleGrid.SelectedItem is not Person p) return;

            var window = new PersonProfileWindow(p.Id);
            window.ShowDialog();
        }
    }
}