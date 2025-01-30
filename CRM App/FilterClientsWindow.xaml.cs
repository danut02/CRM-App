using Microsoft.Win32;
using MySqlX.XDevAPI;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace CRM_App
{
    /// <summary>
    /// Interaction logic for FilterClientsWindow.xaml
    /// </summary>
    public partial class FilterClientsWindow : Window
    {
        public List<Client> clientList;
        public string fileExcel;
        public FilterClientsWindow(List<Client> clients,string fileName)
        {

            InitializeComponent();

            GridCl.ItemsSource = clients;
            GridCl.AlternationCount = int.MaxValue;

            clientList = clients;
            fileExcel=fileName;
           // SaveFileDialog saveFileDialog = new SaveFileDialog();

        }

        private void Window_closed(object sender, EventArgs e)
        {
            Window2 window2 = Application.Current.Windows.OfType<Window2>().FirstOrDefault();

            if (window2 != null)
            {

                window2.Cl_InLucru.Visibility=Visibility.Visible;
                window2.Cl_Final.Visibility=Visibility.Visible;
                window2.Cl_programare.Visibility=Visibility.Visible;
                window2.Cl_avize.Visibility=Visibility.Visible;
            }
        }

        private void PrintButton(object sender, RoutedEventArgs e)
        {
            string filePath = $@"C:\FisiereCRM\ClientsList - {fileExcel}.xlsx";

            try
            {
                if (clientList != null && clientList.Count > 0)
                {
                    using (var workbook = new ClosedXML.Excel.XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Lista Clienți");

                      
                        worksheet.Cell(1, 1).Value = "Nr";
                        worksheet.Cell(1, 2).Value = "Nume";
                        worksheet.Cell(1, 3).Value = "Status";
                        worksheet.Cell(1, 4).Value = "Adresă";
                        worksheet.Cell(1, 5).Value = "Telefon";

                        
                        var headerRange = worksheet.Range("A1:E1");
                        headerRange.Style.Font.Bold = true;
                        headerRange.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightGray;
                        headerRange.Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;

                        
                        int currentRow = 2;
                        foreach (var client in clientList)
                        {
                            worksheet.Cell(currentRow, 1).Value = client.NrClient;
                            worksheet.Cell(currentRow, 2).Value = client.ClientName;
                            worksheet.Cell(currentRow, 3).Value = client.ReqStatus;
                            worksheet.Cell(currentRow, 4).Value = client.ClientAddress;
                            worksheet.Cell(currentRow, 5).Value = client.PhoneNumber;

                            currentRow++;
                        }

                     
                        worksheet.Columns().AdjustToContents();

                        
                        workbook.SaveAs(filePath);
                    }

                    MessageBox.Show("Fișierul Excel a fost generat cu succes!" + Environment.NewLine + " Îl găsiți în " +  $@"C:\\FisiereCRM\\ClientsList - {fileExcel}.xlsx", "Succes",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Nu există niciun client", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"A apărut o eroare: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
