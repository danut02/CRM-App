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
   
    public partial class NotificationWindow : Window
    {
        public string User;
        DbConnect dbConnect = new DbConnect();
        public NotificationWindow(string user)
        {
            
            InitializeComponent();
            NewNotifBtn.Visibility = Visibility.Hidden;
            dgrid1.Visibility = Visibility.Hidden;
            this.Closed += MyWindow_Closed;
            NewElements.Visibility = Visibility.Hidden;
            User = user;
        }

      
       
        public void MyWindow_Closed(object sender, EventArgs e)
        {
            Window2 window2 = Application.Current.Windows.OfType<Window2>().FirstOrDefault();

            if (window2 != null)
            {
               
                window2.NotificationImage.Visibility = Visibility.Visible;
            }



        }

        private async void LoadGrid(object sender, RoutedEventArgs e)

        {
            await Task.Delay(3000);
            LoadingImg.Visibility = Visibility.Hidden;
            
            dgrid1.Visibility=Visibility.Visible;

          
            dgrid1.ItemsSource = dbConnect.GetNotif(User);

            NewNotifBtn.Visibility = Visibility.Visible;
        }

        

        private void ButtonNotif_Click(object sender, RoutedEventArgs e)
        {
            dgrid1.Visibility = Visibility.Hidden;
            //var newNotif = (Border)NewNotifBtn.Template.FindName("NewNotif", NewNotifBtn);
            //var newBtnText = (TextBlock)NewNotifBtn.Template.FindName("NewBtnText", NewNotifBtn);
           
            SaveBtn.Visibility = Visibility.Visible;
            NewNotifBtn.Visibility = Visibility.Hidden;
           
            NewElements.Visibility = Visibility.Visible;            
        }

        private void Save_click(object sender, RoutedEventArgs e)
        {
            string patternTitle = @"^[A-Z][a-z]+(\s[A-Za-z]+){0,4}$"; 
            string patternDesc = @"^[A-Z][a-z]+(\s[A-Za-z]+){0,10}$";
            Regex regTitle = new Regex(patternTitle);
            Regex regDesc = new Regex(patternDesc);    
             
           
           
            
            if (!string.IsNullOrEmpty(title_input.Text) && !string.IsNullOrEmpty(desc_input.Text) && Calendar_Date.SelectedDate != null
               && regTitle.IsMatch(title_input.Text) && regDesc.IsMatch(desc_input.Text))
            {
                //MessageBox.Show("Uploaded");
                SaveBtn.Visibility = Visibility.Hidden;
                NewNotifBtn.Visibility = Visibility.Visible;
                NewElements.Visibility = Visibility.Hidden;
                dgrid1.Visibility = Visibility.Visible;

                dbConnect.AddNotification(title_input.Text, desc_input.Text, Calendar_Date.SelectedDate.Value,User);
                dgrid1.ItemsSource = dbConnect.GetNotif(User);

            }
            else
            
            MessageBox.Show("Failed - empty fields / wrong syntax (Trebuie sa inceapa cu litera mare)");
        }
    }
}
