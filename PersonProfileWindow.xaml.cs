using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CarRegistryProject.Models;
using CarRegistryProject.Services;

namespace CarRegistryProject
{
    /// <summary>
    /// Interaction logic for PersonProfileWindow.xaml
    /// </summary>
    public partial class PersonProfileWindow : Window
    {
        private readonly int _personId;
        private readonly PersonService _personService = new PersonService();
        private Person? _person;
        public PersonProfileWindow(int personId)
        {
            InitializeComponent();
            _personId = personId;
            LoadPerson();
        }

        private void LoadPerson()
        {
            _person = _personService.GetPersonById(_personId);

            if (_person == null)
            {
                MessageBox.Show("Person not found.");
                Close();
                return;
            }

            Title = $"{_person.Name} (ID: {_person.Id})";
            HeaderText.Text = $"{_person.Name} | GovID: {_person.GovernmentId} | Phone: {_person.PhoneNumber}";
        }
    }
}
