using System;
using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient; // <--- Add this using directive for SQL Server interaction

namespace CW3_Group_5
{
    /// <summary>
    /// Interaction logic for Login_RegistrationWindow.xaml
    /// </summary>
    public partial class Login_RegistrationWindow : Window
    {
        // Declare connectionString once at the class level
        private readonly string connectionString =
            @"Server=localhost\SQLEXPRESS;Database=HotelBookingDB;Trusted_Connection=True;"; //

        public Login_RegistrationWindow()
        {
            InitializeComponent();
        }

        // Optional, remove if not needed
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Optional, remove if not needed
        }

        // Optional, remove if not needed
        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            // Optional, remove if not needed
        }

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
            this.Close(); // Optional: close login/registration window
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // This button click is currently empty
        }

        private void ReturnHome_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close(); // closes the current window
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text.Trim(); // Trim whitespace
            string password = PasswordBox.Password;   // Get password

            // --- Basic Validation ---
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both email and password.", "Sign In Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // SQL query to check if the email and password (hash) exist in the Users table
                    // IMPORTANT: This assumes PasswordHash stores the plain password for now.
                    // In a real application, you'd hash the 'password' input here and compare hashes.
                    string query = "SELECT UserID, RoleID FROM Users WHERE Email = @Email AND PasswordHash = @PasswordHash"; //

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Add parameters to prevent SQL injection
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@PasswordHash", password); // Compare with plain password stored in DB for now

                        connection.Open(); // Open the database connection

                        // Use ExecuteReader to read data (UserID and RoleID)
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read()) // If a row is returned, credentials are valid
                            {
                                int userId = reader.GetInt32(0); // Get UserID (first column)
                                int roleId = reader.GetInt32(1); // Get RoleID (second column)

                                MessageBox.Show("Login successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                                // TODO: Based on RoleID, you might open different windows or enable different features
                                // For now, let's open BookHotelWindow
                                BookHotelWindow bookHotelWindow = new BookHotelWindow();
                                bookHotelWindow.Show();
                                this.Close(); // Close the current login window
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
    }
}