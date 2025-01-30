using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CRM_App
{
    /// <summary>
    /// Interaction logic for AddNewClient.xaml
    /// </summary>
    public partial class AddNewClient : Window
    {
        DbConnect dbConnect = new DbConnect();
        public event Action ClientAdded;
        string phoneNrPattern = @"^0[0-9]{9}$";
        string fullNamePattern = @"^[A-Z][a-z]+ [A-Z][a-z]+( [A-Z][a-z]+)?$";
        public AddNewClient()
        {
            InitializeComponent();
            
            
        }



        private void Save_Click(object sender, RoutedEventArgs e)
        {
            
            Regex regPhone = new Regex(phoneNrPattern);
            Regex regFullName = new Regex(fullNamePattern);

            // MessageBox.Show(name_input.Text + " " + status_input.Text + " " + address_input.Text + " " + phone_input.Text);
            if (!string.IsNullOrEmpty(name_input.Text) && !string.IsNullOrEmpty(status_input.Text) && !string.IsNullOrEmpty(address_input.Text)
             && !string.IsNullOrEmpty(phone_input.Text) && regPhone.IsMatch(phone_input.Text) && regFullName.IsMatch(name_input.Text))
            {
                dbConnect.UploadCustomer(name_input.Text, status_input.Text, address_input.Text, phone_input.Text,OL_input.Text);
                ClientAdded?.Invoke();
                this.Close();
            }

            else
                MessageBox.Show("Failed");
          
        }

        private void EnterPressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Regex regPhone = new Regex(phoneNrPattern);
                Regex regFullName = new Regex(fullNamePattern);

                // MessageBox.Show(name_input.Text + " " + status_input.Text + " " + address_input.Text + " " + phone_input.Text);
                if (!string.IsNullOrEmpty(name_input.Text) && !string.IsNullOrEmpty(status_input.Text) && !string.IsNullOrEmpty(address_input.Text)
                 && !string.IsNullOrEmpty(phone_input.Text) && regPhone.IsMatch(phone_input.Text) && regFullName.IsMatch(name_input.Text))
                {
                    dbConnect.UploadCustomer(name_input.Text, status_input.Text, address_input.Text, phone_input.Text,OL_input.Text);
                    ClientAdded?.Invoke();
                    this.Close();
                }

                else
                    MessageBox.Show("Failed");

            }
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            Window2 window2 = Application.Current.Windows.OfType<Window2>().FirstOrDefault();

            if (window2 != null)
            {
                window2.AddCl.Visibility = Visibility.Visible;

            }
        }
    }
}
