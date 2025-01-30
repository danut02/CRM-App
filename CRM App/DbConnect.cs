using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using Google.Protobuf.WellKnownTypes;
using MySqlX.XDevAPI;
using Microsoft.VisualBasic;
using System.Windows;
using System.Security.Cryptography.X509Certificates;


namespace CRM_App
{
    class DbConnect
    {
       public int cntOutdated=0;
       public SqlConnection Connection()
        {
           string connString = "Data Source=192.168.0.182,4401;Initial Catalog=dbCRM;User ID=sa;Password=1234;Connect Timeout=30";
           SqlConnection conn = new SqlConnection(connString);
           return conn;

        }

        [Obsolete]
        public List<User> GetUsers()
        {
           

            List<User> usersList = new List<User>();
            using (SqlConnection conn = Connection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM UsersKofalm", conn);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        User user = new User
                        {
                            ID = reader["UserID"] != DBNull.Value ? (Guid)reader["UserID"] : Guid.Empty,
                            Name = reader["Username"].ToString(),
                            Password = reader["Pass"].ToString()
                            
                        };
                        usersList.Add(user);
                    }
                }
            }
            return usersList;
        }
       
        public string GetUserEmail(string user)
        {
            string email=String.Empty;
            using (SqlConnection conn = Connection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT Email FROM UsersKofalm WHERE Username=@Username",conn);
                cmd.Parameters.AddWithValue("@Username", user);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while(reader.Read())
                    {

                        email = reader["Email"].ToString();
                    }


                }

            }

            return email;
         }
        public Guid GetUserID(string username)
        {
            Guid userID = Guid.Empty;
            using (SqlConnection conn = Connection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT UserID FROM UsersKofalm WHERE Username=@Username", conn);
                cmd.Parameters.AddWithValue("@Username", username);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        
                        userID = reader["UserID"] != DBNull.Value ? (Guid)reader["UserID"] : Guid.Empty;
                    }
                }
            }
            return userID;

        }
        public bool CheckUser(string username, string password)
        {
            bool isValid = false;
            using (SqlConnection conn = Connection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM UsersKofalm WHERE Username COLLATE Latin1_General_BIN = @Username AND Pass COLLATE Latin1_General_BIN = @Password", conn);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Password", password);

                int userCount = (int)cmd.ExecuteScalar(); 

                if (userCount > 0)
                {
                    isValid = true; 
                }
            }
            return isValid;
        }

        public int CountClients()
        {
            using (SqlConnection conn = Connection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM ClientsKofalm",conn);
                int getClNumber=(int)cmd.ExecuteScalar();

                return getClNumber;
            }
        }

        public List<Client> GetClients()
        {
            List<Client> clientsList = new List<Client>();
            using (SqlConnection conn = Connection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM ClientsKofalm", conn);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    
                    
                    while (reader.Read())
                    {
                        Client client = new Client
                        {

                            ClientID = reader["ClientID"] != DBNull.Value ? (Guid)reader["ClientID"] : Guid.Empty,
                            ClientName = reader["FullName"].ToString(),
                            ReqStatus = reader["ReqStatus"].ToString(),
                            ClientAddress = reader["ClientAddress"].ToString(),
                            PhoneNumber = reader["PhoneNumber"].ToString(),
                            OLClient = reader["OL"].ToString()

                        };
                        clientsList.Add(client);
                    }
                }
            }

            return clientsList;

        }
        public List<Notification> GetNotif(string user)
        {
            List<Notification> notifications = new List<Notification>();

            
            using (SqlConnection conn = Connection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM NotificationsKofalm WHERE UserID = @UserID", conn);
                cmd.Parameters.AddWithValue("@UserID",GetUserID(user));
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                       
                        Notification notif = new Notification
                        {

                           Title = reader["Title"].ToString(),
                           Desc = reader ["Description"].ToString(),
                           DueDate = (DateTime) reader["DueDate"],
                           OutDated = reader["Outdated"].ToString()
                          


                        };
                        notifications.Add(notif);
                    }
                }

                
                notifications.ForEach(notification =>
                {
                    if (notification.DueDate > DateTime.Now)
                        notification.OutDated = "No";
                    else
                        notification.OutDated = "Yes";

                    if (notification.OutDated == "Yes")
                        cntOutdated++;
                });


                return notifications;
            }

        }
        public void UpdateStatus(string status,Guid ID)
        {
            using (SqlConnection conn = Connection())
            {
                conn.Open();

                

               
                SqlCommand cmd = new SqlCommand("UPDATE ClientsKofalm SET ReqStatus = @Status WHERE ClientID = @ClientID", conn);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@ClientID", ID);
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Status updated succesfully!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Failed to update.", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        
        public void UpdateAddress(string addr, Guid ID)
        {
            using (SqlConnection conn = Connection())
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("UPDATE ClientsKofalm SET ClientAddress = @Address WHERE ClientID = @ClientID", conn);
                cmd.Parameters.AddWithValue("@Address", addr);
                cmd.Parameters.AddWithValue("@ClientID", ID);
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Address updated succesfully");
                }
                else
                {
                    MessageBox.Show("Failed to update address.");
                }
            }
        }

        public void UpdatePhoneNr(string phone, Guid ID)
        {
            using (SqlConnection conn = Connection())
            {
                conn.Open();

                SqlCommand checkCmd = new SqlCommand("SELECT PhoneNumber FROM ClientsKofalm WHERE ClientID = @ClientID", conn);
                checkCmd.Parameters.AddWithValue("@ClientID", ID);
                var currentPhoneNr = checkCmd.ExecuteScalar()?.ToString();

                if (currentPhoneNr == phone)
                {
                    MessageBox.Show("Failed");
                    return;
                }


                SqlCommand cmd = new SqlCommand("UPDATE ClientsKofalm SET PhoneNumber = @PhoneNr WHERE ClientID = @ClientID", conn);
                cmd.Parameters.AddWithValue("@PhoneNr", phone);
                cmd.Parameters.AddWithValue("@ClientID", ID);
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Phone number updated succesfully");
                }
                else
                {
                    MessageBox.Show("Failed to update phone number");
                }
            }
        }

        public void UpdateOL(string OL, Guid ID)
        {

            using (SqlConnection conn = Connection())
            {
                conn.Open();
                SqlCommand checkCmd = new SqlCommand("SELECT OL FROM ClientsKofalm WHERE ClientID = @ClientID", conn);
                checkCmd.Parameters.AddWithValue("@ClientID", ID);
                var currentOL = checkCmd.ExecuteScalar()?.ToString();
                if (currentOL == OL)
                {
                    MessageBox.Show("Failed");
                    return;
                }
                SqlCommand cmd = new SqlCommand("UPDATE ClientsKofalm SET OL = @NrOL WHERE ClientID = @ClientID", conn);
                cmd.Parameters.AddWithValue("@NrOL", OL);
                cmd.Parameters.AddWithValue("@ClientID", ID);
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    MessageBox.Show("OL updated succesfully");
                }
                else
                {
                    MessageBox.Show("Failed to update OL");
                }
            }

        }
        public void AddNotification(string Title,string Desc,DateTime DueDate,string user)
        {
            using (SqlConnection conn = Connection())
            {

                Guid userID = GetUserID(user);
                  conn.Open();
                  SqlCommand cmd = new SqlCommand("INSERT INTO NotificationsKofalm(Title,Description,DueDate,Outdated,UserID)" +
                      " VALUES (@title,@desc,@dueDate,'',@userID)",conn);
                cmd.Parameters.AddWithValue("@title", Title);
                cmd.Parameters.AddWithValue("@desc", Desc);
                cmd.Parameters.AddWithValue("@dueDate", DueDate);
                cmd.Parameters.AddWithValue("@userID", userID);
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                    MessageBox.Show("Uploaded");
                else
                    MessageBox.Show("Failed");
            }


        }

        public void UploadCustomer(string name,string status, string address,string phoneNr,string OL)
        {

            using (SqlConnection conn = Connection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO ClientsKofalm(FullName,ReqStatus,ClientAddress,PhoneNumber,OL) VALUES (@name,@status,@address,@phoneNr,@ol)",conn);
                cmd.Parameters.AddWithValue("@name",name);
                cmd.Parameters.AddWithValue("@status",status);
                cmd.Parameters.AddWithValue("@address", address);
                cmd.Parameters.AddWithValue("@phoneNr",phoneNr);
                cmd.Parameters.AddWithValue("@ol", OL);
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                    MessageBox.Show("Uploaded");
                else
                    MessageBox.Show("Failed");
            }
        }

    }
}
