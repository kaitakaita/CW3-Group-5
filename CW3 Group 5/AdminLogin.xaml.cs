using System;
using System.Data.SqlClient;
using System.Windows;

namespace CW3_Group_5
{
    public partial class AdminLogin : Window
    {
        public AdminLogin()
        {
            InitializeComponent();
        }

        private void AdminLogin_Click(object sender, RoutedEventArgs e)
        {
            string email = AdminEmailTextBox.Text;
            string password = AdminPasswordBox.Password;

            // ⚠️ Replace with your actual connection string
            string connectionString = "Data Source=DESKTOP-8VS8BG\\SQL2022TRAINING;Initial Catalog=DatabaseDB;Integrated Security=True";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"SELECT UserID FROM Users 
                                     WHERE Email = @Email AND PasswordHash = @Password AND RoleID = 1"; // RoleID = 1 means Admin

                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password); // ⚠️ Consider hashing this in real use

                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        int adminID = Convert.ToInt32(result);

                        // Log admin login
                        string logQuery = @"INSERT INTO AdminActionLog (AdminID, ActionType, Description, Timestamp)
                                            VALUES (@AdminID, 'Login', 'Admin successfully logged in.', @Timestamp)";
                        SqlCommand logCmd = new SqlCommand(logQuery, connection);
                        logCmd.Parameters.AddWithValue("@AdminID", adminID);
                        logCmd.Parameters.AddWithValue("@Timestamp", DateTime.Now);
                        logCmd.ExecuteNonQuery();

                        MessageBox.Show("Login successful!", "Admin Login", MessageBoxButton.OK, MessageBoxImage.Information);
                        // TODO: Navigate to Admin Dashboard here
                    }
                    else
                    {
                        MessageBox.Show("Invalid credentials or not an admin.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

