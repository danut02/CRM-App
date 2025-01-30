using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Linq;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using MySqlX.XDevAPI;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.VisualBasic;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Threading;
using System.IO.Ports;

namespace CRM_App
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        // public string[] tipCerere = {"Aprobata","In lucru","Respinsa"};
        public ObservableCollection<Client> Clients { get; set; }
        public ICommand ClientDetailButton { get; }
        public int marginOffset = 170;
        public bool isPressedStatus=false, isPressedAddress=false, isPressedPhone=false, isPressedOL=false;
        private CollectionViewSource _clientCollectionView;
        public event EventHandler? CanExecuteChanged;
        DbConnect dbConnect = new DbConnect();
        public string Username;
        private DispatcherTimer _timer;
        string subject = "KOFALM";
        public bool isFilterPressed = false;


        public Window2(string user)
        {
            
            InitializeComponent();
            NumeUserCRM.Text = user;
            string userEmail = dbConnect.GetUserEmail(user);
            string destination = userEmail;
            Clients = new ObservableCollection<Client>(dbConnect.GetClients());

            DataContext = this;
            ClientDetailButton = new RelayCommand(ExecuteClientDetail);

            _clientCollectionView = new CollectionViewSource();
            _clientCollectionView.Source = Clients; 
            _clientCollectionView.Filter += ClientFilter;

            ClientiDb.ItemsSource = _clientCollectionView.View;
            dbConnect.GetNotif(user);
            string body = "<h1>Salut</h1> <img src='cid:sigla'/> <h2>" + user + " are " + dbConnect.cntOutdated + " notificari noi </h2>";
            if (dbConnect.cntOutdated == 0)
                NotificationImage.Visibility = Visibility.Hidden;
            else
            {
                NotificationImage.Visibility = Visibility.Visible;
                EmailSender emailSender = new EmailSender();
                emailSender.SendEmail(subject, body, destination);
            }

            //MessageBox.Show(dbConnect.GetUserID(user).ToString());
            Username = user;

            TotalClients.Text="(Numar de clienti : " + dbConnect.CountClients().ToString() + ")";
            InitializePolling();
            ReloadClients();


            GridTable.Visibility = Visibility.Hidden;
            this.ResizeMode = ResizeMode.NoResize;
        }


        private void InitializePolling()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(2); 
            _timer.Tick += (s, e) => CheckForUpdates();
            _timer.Start();
        }
        private void CheckForUpdates()
        {
            var latestCount = dbConnect.CountClients();
            if (latestCount != Clients.Count) 
            {
                ReloadClients();
            }
        }
        
        private void ClientFilter(object sender, FilterEventArgs e)
        {
            if (e.Item is Client client)
            {
                if (string.IsNullOrEmpty(SearchBox.Text) ||  client.ClientName.Contains(SearchBox.Text, StringComparison.OrdinalIgnoreCase) ||
            client.ClientAddress.Contains(SearchBox.Text, StringComparison.OrdinalIgnoreCase))
                {

                    e.Accepted = true;
                    
                }
                else
                {
                    
                    e.Accepted = false;
                }
            }


        }

        
        private void NotifClick(object sender, MouseButtonEventArgs e)
        {
            
            NotificationImage.Visibility = Visibility.Hidden;
            NotificationWindow notificationWindow = new NotificationWindow(Username);
           


            notificationWindow.Show();
        }

        private void WindowSizeChanged(object sender, SizeChangedEventArgs e)
        {

            if(this.ActualWidth<740 && isFilterPressed)
            {

                Cl_Final.Visibility = Visibility.Hidden;
                Cl_InLucru.Visibility = Visibility.Hidden;
                Cl_programare.Visibility = Visibility.Hidden;
                Cl_avize.Visibility = Visibility.Hidden;
            }
            if (this.ActualWidth > 740 && isFilterPressed)
            {
                Cl_Final.Visibility = Visibility.Visible;
                Cl_InLucru.Visibility = Visibility.Visible;
                Cl_programare.Visibility = Visibility.Visible;
                Cl_avize.Visibility = Visibility.Visible;

            }


            if (this.ActualWidth < 670)
            {
               
               
                TotalClients.Visibility = Visibility.Hidden;
            }
            else
            {
                
                
                TotalClients.Visibility = Visibility.Visible;
            }
           if(this.ActualWidth<850)
                GridTable.Margin = new Thickness(this.ActualWidth / 2 - marginOffset, 0,0, 0);
            else
                GridTable.Margin = new Thickness(300, 0, 0, 0);
            if (this.ActualWidth < 720)
            {
                SearchBox.Visibility = Visibility.Hidden;
                SearchBtn.Visibility = Visibility.Hidden;
            }
            else
            {

                SearchBox.Visibility = Visibility.Visible;
                SearchBtn.Visibility = Visibility.Visible;
            }


        }


        private void ExecuteClientDetail(object parameter)
        {
           
            if (parameter is Client client)
            {
                NumeClient.Text = client.ClientName;
                Status.Text=client.ReqStatus;
                Adress.Text=client.ClientAddress;
                NrTelefon.Text = client.PhoneNumber;
                IDcl.Text = client.ClientID.ToString();
                OL_text.Text = client.OLClient; 

                // MessageBox.Show($"Detalii pentru clientul: {client.ClientName}, Telefon: {client.PhoneNumber}");
            }
        }

        private void Edit_Status(object sender, RoutedEventArgs e)
        {
            isPressedStatus = !isPressedStatus;
            Button_Status.Visibility= Visibility.Hidden;
            Dropdown_Status.Visibility = Visibility.Visible;
            Save_Button.Visibility = Visibility.Visible;
            Dropdown_Status.Text = Status.Text;
        }

      

        private async void MouseClick(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;

            if (clickedButton != null)
            {
                if(!isPressedStatus)
                    Button_Status.Visibility = Visibility.Visible;
                if(!isPressedAddress)
                    Btn_Address.Visibility = Visibility.Visible;
                if(!isPressedPhone)
                    EditPhoneTxt.Visibility = Visibility.Visible;
                if(!isPressedOL)
                    EditOL.Visibility = Visibility.Visible;
                isPressedOL = !isPressedOL;
                isPressedStatus = !isPressedStatus;
                isPressedAddress = !isPressedAddress;
                isPressedPhone = !isPressedPhone;
                clickedButton.Background = Brushes.DodgerBlue;


                Box_OL.Visibility = Visibility.Hidden;
                Save_OL.Visibility = Visibility.Hidden;
               

                Dropdown_Status.Visibility=Visibility.Hidden;
                UpdateAddressTxt.Visibility=Visibility.Hidden;
                UpdatePhoneTxt.Visibility=Visibility.Hidden;
                Save_Button2.Visibility=Visibility.Hidden;
                Save_Button3.Visibility = Visibility.Hidden;
                Save_Button.Visibility = Visibility.Hidden;
                
                await Task.Delay(500);


                clickedButton.Background = Brushes.Aqua;
               
            }
            


        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Save_Button.Visibility = Visibility.Hidden;
            Dropdown_Status.Visibility = Visibility.Hidden;
            Status.Text = Dropdown_Status.Text;
            if (NumeClient.Text != "" && Status.Text != "")
            {
                //MessageBox.Show(IDcl.Text);
               
               
                Client selectedClient = Clients.FirstOrDefault(c => c.ClientName == NumeClient.Text && c.ClientID.ToString() == IDcl.Text);
                if (selectedClient != null)
                {
                   
                    dbConnect.UpdateStatus(Status.Text, selectedClient.ClientID);
                    selectedClient.ReqStatus = Status.Text;

                  
                    Status.Text = selectedClient.ReqStatus;
                }
            }

        }

        private void Edit_Address(object sender, RoutedEventArgs e)
        {
            isPressedAddress= !isPressedAddress;
            Btn_Address.Visibility = Visibility.Hidden;
            UpdateAddressTxt.Visibility = Visibility.Visible;
            UpdateAddressTxt.Text = "";
            Save_Button2.Visibility = Visibility.Visible;
        }

        private void EditPhone(object sender, RoutedEventArgs e)
        {
            isPressedPhone= !isPressedPhone;
            EditPhoneTxt.Visibility = Visibility.Hidden;
            Save_Button3.Visibility = Visibility.Visible;
            UpdatePhoneTxt.Visibility = Visibility.Visible;
            UpdatePhoneTxt.Text = "";
           
        }

        private void SearchClick(object sender, RoutedEventArgs e)
        {
            
                _clientCollectionView.View.Refresh();
                          

        }

        private void EnterSearch(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                _clientCollectionView.View.Refresh();
            }
        }

        private void AddClient_Click(object sender, RoutedEventArgs e)
        {
            AddNewClient addNewClient = new AddNewClient();
            addNewClient.ClientAdded += ReloadClients;

            addNewClient.Show();

            AddCl.Visibility = Visibility.Hidden;
        }
        private void ReloadClients()
        {

            Clients.Clear();
            foreach (var client in dbConnect.GetClients())
            {
                Clients.Add(client);
            }
            UpdateTotalClients();
            _clientCollectionView.View.Refresh();
           
        }
        
        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }
        public void UpdateTotalClients()
        {
            TotalClients.Text = "(Numar de clienti : " + dbConnect.CountClients().ToString() + ")";
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            string filePath = @"C:\FisiereCRM\ClientsList.txt"; 

            try
            {
                if (dbConnect.CountClients() != 0)
                {
                    using (StreamWriter writer = new StreamWriter(filePath, false))
                    {

                        writer.WriteLine("Lista Clienți:");
                        writer.WriteLine("==============");

                        foreach (var client in Clients)
                        {

                            writer.WriteLine($"Nume: {client.ClientName}");
                            writer.WriteLine($"Status: {client.ReqStatus}");
                            writer.WriteLine($"Adresă: {client.ClientAddress}");
                            writer.WriteLine($"Telefon: {client.PhoneNumber}");
                            writer.WriteLine("-------------------------------");
                        }
                    }

                    MessageBox.Show("Fișierul a fost generat cu succes!" + Environment.NewLine + " Il gasiti in C:\\FisiereCRM\\ClientsList.txt", "Succes",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                    MessageBox.Show("Nu exista niciun client","Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"A apărut o eroare: {ex.Message}", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FilterCl_Click(object sender, RoutedEventArgs e)
        {
            isFilterPressed = true;
            FilterCl.Visibility = Visibility.Hidden;
            Cl_Final.Visibility = Visibility.Visible;
            Cl_InLucru.Visibility = Visibility.Visible;
            Cl_programare.Visibility = Visibility.Visible;
            Cl_avize.Visibility = Visibility.Visible;
            ClientiDb.Margin = new Thickness(0,40,0,0); 
        }

        private void Cl_InLucru_Click(object sender, RoutedEventArgs e)
        {
            List<Client> allClients = dbConnect.GetClients();
            List<Client> ClientsInWork = new List<Client>();
           

            ClientsInWork = allClients.Where(client => client.ReqStatus.ToLower().Contains("in lucru")).ToList();
            int counter = 1;
            foreach (var client in ClientsInWork)
            {
                client.NrClient = counter++;
            }

            FilterClientsWindow filterClientsWindow = new FilterClientsWindow(ClientsInWork, "In lucru");
                filterClientsWindow.Show();

            Cl_InLucru.Visibility= Visibility.Hidden;


        }

        private void ClFinal_Click(object sender, RoutedEventArgs e)
        {
            
            List<Client> allClients = dbConnect.GetClients();
            List<Client> CompletedClients = new List<Client>();

            CompletedClients = allClients
        .Where(client => client.ReqStatus.Equals("Finalizat", StringComparison.OrdinalIgnoreCase) ||
                         client.ReqStatus.StartsWith("Finalizat -", StringComparison.OrdinalIgnoreCase))
        .ToList();

            int counter = 1;
            foreach (var client in CompletedClients)
            {
                client.NrClient = counter++;
            }
            FilterClientsWindow filterClientsWindow = new FilterClientsWindow(CompletedClients, "Finalizati");
            filterClientsWindow.Show();

            Cl_Final.Visibility= Visibility.Hidden;
        }

        private async void WindowLoaded(object sender, RoutedEventArgs e)
        {
           
            await Task.Delay(4000);


            this.ResizeMode = ResizeMode.CanResizeWithGrip;
            GridTable.Visibility= Visibility.Visible;
            LoadingImg.Visibility= Visibility.Hidden;

        }

        private void Cl_programare_click(object sender, RoutedEventArgs e)
        {
            List<Client> allClients = dbConnect.GetClients();
            List<Client> ClProg= new List<Client> ();
            ClProg = allClients.Where(client => client.ReqStatus.ToLower().Contains("executie")).ToList();
            int counter = 1;
            foreach (var client in ClProg)
            {
                client.NrClient = counter++;
            }
            FilterClientsWindow filterClientsWindow = new FilterClientsWindow(ClProg,"Executie");


            filterClientsWindow.Show();
            Cl_programare.Visibility= Visibility.Hidden;
        }

        private void EditOL_Click(object sender, RoutedEventArgs e)
        {
            Box_OL.Visibility= Visibility.Visible;
            Save_OL.Visibility= Visibility.Visible;
            EditOL.Visibility= Visibility.Hidden;
            Box_OL.Text = OL_text.Text;
        }

        private void SaveOL_Click(object sender, RoutedEventArgs e)
        {
            OL_text.Text = Box_OL.Text;
            Box_OL.Visibility = Visibility.Hidden;
            Save_OL.Visibility = Visibility.Hidden;
            if (NumeClient.Text != "")
            {

                Client selectedClient = Clients.FirstOrDefault(c => c.ClientName == NumeClient.Text && c.ClientID.ToString() == IDcl.Text);
                if (selectedClient != null)
                {
                    dbConnect.UpdateOL(OL_text.Text, selectedClient.ClientID);
                    selectedClient.OLClient = OL_text.Text;

                    OL_text.Text = selectedClient.OLClient;
                }
            }


        }

        private void Cl_avize_click(object sender, RoutedEventArgs e)
        {
            List<Client> allClients = dbConnect.GetClients();
            List<Client> ClAvize = new List<Client>();
            ClAvize = allClients.Where(client => client.ReqStatus.ToLower().Contains("aviz")).ToList();
            int counter = 1;
            foreach (var client in ClAvize)
            {
                client.NrClient = counter++;
            }
            FilterClientsWindow filterClientsWindow = new FilterClientsWindow(ClAvize,"Avize");


            filterClientsWindow.Show();
            Cl_avize.Visibility= Visibility.Hidden;
        }

        private void SaveAddr_Click(object sender, RoutedEventArgs e)
        {
            if (UpdateAddressTxt.Text != "")
            {
                Save_Button2.Visibility = Visibility.Hidden;
                UpdateAddressTxt.Visibility = Visibility.Hidden;
                Adress.Text = UpdateAddressTxt.Text;
                if (NumeClient.Text != "")
                {

                    Client selectedClient = Clients.FirstOrDefault(c => c.ClientName == NumeClient.Text && c.ClientID.ToString() == IDcl.Text);
                    if (selectedClient != null)
                    {
                        dbConnect.UpdateAddress(Adress.Text, selectedClient.ClientID);
                        selectedClient.ClientAddress = Adress.Text;

                        Adress.Text = selectedClient.ClientAddress;

                    }
                }
            }
        }

        private void SavePhone_Click(object sender, RoutedEventArgs e)
        {
            string checkNr = @"^0[0-9]{9}$";
            Regex RegPhone = new Regex(checkNr);
            if (RegPhone.IsMatch(UpdatePhoneTxt.Text))
            {
                NrTelefon.Text = UpdatePhoneTxt.Text;
                UpdatePhoneTxt.Visibility = Visibility.Hidden;
                Save_Button3.Visibility = Visibility.Hidden;
                if (NumeClient.Text != "")
                {

                    Client selectedClient = Clients.FirstOrDefault(c => c.ClientName == NumeClient.Text && c.ClientID.ToString() == IDcl.Text);
                    if (selectedClient != null)
                    {
                        dbConnect.UpdatePhoneNr(NrTelefon.Text, selectedClient.ClientID);
                        selectedClient.PhoneNumber = NrTelefon.Text;

                        NrTelefon.Text = selectedClient.PhoneNumber;
                    }
                }

            }
            else
                MessageBox.Show("Failed", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            
            
        }
    }
    }
