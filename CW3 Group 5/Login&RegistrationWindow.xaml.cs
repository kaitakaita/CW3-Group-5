using System;
using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;
using System.Security.Cryptography; // ADD THIS FOR HASHING
using System.Text;                  // ADD THIS FOR HASHING

namespace CW3_Group_5
{
    public partial class Login_RegistrationWindow : Window
    {
        private readonly string connectionString =
            @"Server=localhost\SQLEXPRESS;Database=HotelBookingDB;Trusted_Connection=True;";

        public Login_RegistrationWindow()
        {
            InitializeComponent();
        }

        // Optional, remove if not needed
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e) { }
        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e) { }

        private void EmailTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            EmailPlaceholder.Visibility = string.IsNullOrWhiteSpace(EmailTextBox.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordPlaceholder.Visibility = string.IsNullOrWhiteSpace(PasswordBox.Password)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            SignUpWindow signUpWindow = new SignUpWindow();
            signUpWindow.Show();
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // This button click is currently empty
        }

        private void ReturnHome_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both email and password.", "Sign In Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // --- HASH THE INPUT PASSWORD FOR COMPARISON ---
            string hashedPassword = ComputeSha256Hash(password);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT UserID, RoleID, FirstName FROM Users WHERE Email = @Email AND PasswordHash = @PasswordHash";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@PasswordHash", hashedPassword); // Compare with the hashed input password

                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int userId = reader.GetInt32(0);
                                int roleId = reader.GetInt32(1);
                                string firstName = reader.GetString(2); // Assuming FirstName is retrieved from the Users table

                                MessageBox.Show($"Login successful! Welcome {firstName}. Your RoleID is {roleId}.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                                BookHotelWindow bookHotelWindow = new BookHotelWindow();
                                bookHotelWindow.CurrentUserId = userId; // Pass the logged-in user's ID
                                bookHotelWindow.Show();
                                this.Close();
                            }
                            else
                            {
                                MessageBox.Show("Invalid email or password.", "Sign In Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Database error: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unexpected error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // --- ADD THIS HELPER METHOD FOR PASSWORD HASHING ---
        private string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        // ---------------------------------------------------
    }
}