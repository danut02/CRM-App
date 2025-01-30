using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using Google.Protobuf.WellKnownTypes;
using MySqlX.XDevAPI;
using Microsoft.VisualBasic;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Win32;


namespace CRM_App;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    DbConnect dbConnect = new DbConnect();
    public MainWindow()
    {
        InitializeComponent();
      

    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
        
    {
       
        Button1.Background = Brushes.Aqua;
        await Task.Delay(500);
        Button1.Background = Brushes.RoyalBlue;

        String box1 = textBox1.Text;
        String box2 = textBox2.Password.ToString();
       /* String user = @"\b[A-Z]\w{4,11}\b";
        String pass = @"^[0-9A-Za-z]{6,16}$";
        Regex rgUser = new Regex(user);
        Regex rgPass = new Regex(pass); */
        if (dbConnect.CheckUser(box1,box2))
        {
            Window2 win2 = new Window2(box1);
            win2.Show();
            this.Close();
            dbConnect.GetUserID(box1);
            MessageBox.Show("Username: " + box1 + Environment.NewLine + "Password: " + new string('*', box2.Length), "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
           
        }
        else
        {
            MessageBox.Show("Wrong password/username", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);

        }


    }


    private void PassTyping(object sender, KeyEventArgs e)
    {
    }

    private void Enter_Pressed(object sender, KeyEventArgs e)
    {
       
        String box1 = textBox1.Text;
        String box2 = textBox2.Password.ToString();
        if (e.Key==Key.Enter)

        {
       
            if (dbConnect.CheckUser(box1, box2))
            {
                Window2 win2 = new Window2(box1);
                win2.Show();
                this.Close();
                dbConnect.GetUserID(box1);
                MessageBox.Show("Username: " + box1 + Environment.NewLine + "Password: " + new string('*', box2.Length), "Succes", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            else
            {
                MessageBox.Show("Wrong password/username","Failed",MessageBoxButton.OK, MessageBoxImage.Error);

            }

        }
    }

    private void WindowSizeChange(object sender, SizeChangedEventArgs e)
    {
       // Title.Content = e.NewSize.Height.ToString();

        if (e.NewSize.Height > 600)
            Title.FontSize = 40;


        if (e.NewSize.Width < 600)
        {
            textBox1.Width = 200;
            textBox2.Width = 200;
            Title.FontSize = 36 * e.NewSize.Width / 500;

        }
        else
         if (e.NewSize.Width > 1300) { 
            textBox1.Width = 280;
        textBox2.Width   =   280; }
        else
        {
            textBox1.Width = e.NewSize.Width * 0.25;
            textBox2.Width = e.NewSize.Width * 0.25;
           
            Button1.Width = e.NewSize.Width * 0.1;
            Title.FontSize = 36;


        }
       
    }
}