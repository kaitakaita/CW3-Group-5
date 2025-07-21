using System;
using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;

namespace CW3_Group_5
{
    /// <summary>
    /// Interaction logic for SignUpWindow.xaml
    /// </summary>
    public partial class SignUpWindow : Window
    {
        // Declare connectionString once at the class level
        private readonly string connectionString =
            @"Server=localhost\SQLEXPRESS;Database=HotelBookingDB;Trusted_Connection=True;"; //

        public SignUpWindow()
        {
            InitializeComponent();
        }

        private void FullNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FullNamePlaceholder.Visibility = string.IsNullOrWhiteSpace(FullNameTextBox.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
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

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ConfirmPasswordPlaceholder.Visibility = string.IsNullOrWhiteSpace(ConfirmPasswordBox.Password)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            // Get data from UI fields
            string fullName = FullNameTextBox.Text.Trim();
            string email = EmailTextBox.Text.Trim();
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            // --- Basic Input Validation ---
            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Password and Confirm Password do not match.");
                return;
            }

            // --- Splitting Full Name into First and Last Name ---
            string firstName = "";
            string lastName = "";
            string[] nameParts = fullName.Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
            if (nameParts.Length > 0)
            {
                firstName = nameParts[0];
                if (nameParts.Length > 1)
                {
                    lastName = nameParts[1];
                }
            }
            // If only one part, lastName will remain empty. You might want to enforce both.
            // If you want to strictly enforce separate first/last names, it's better to have two separate TextBoxes in XAML.

            // --- Password Handling (IMPORTANT: In production, hash this password!) ---
            // For this demo, we are storing the plain password as PasswordHash,
            // but this is NOT secure for real applications.
            string passwordHash = password; //

            // Default role ID for new users (e.g., 2 for a regular user, 1 for admin)
            int roleId = 2; //

            // --- SQL INSERT Command ---
            string query = "INSERT INTO Users (FirstName, LastName, Email, PasswordHash, RoleID) " +
                           "VALUES (@FirstName, @LastName, @Email, @PasswordHash, @RoleID)"; //

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString)) //
                using (SqlCommand command = new SqlCommand(query, connection)) //
                {
                    // Add parameters to prevent SQL injection and handle data types
                    command.Parameters.AddWithValue("@FirstName", firstName); //
                    command.Parameters.AddWithValue("@LastName", lastName); //
                    command.Parameters.AddWithValue("@Email", email); //
                    command.Parameters.AddWithValue("@PasswordHash", passwordHash); //
                    command.Parameters.AddWithValue("@RoleID", roleId); //

                    connection.Open(); // Open the database connection
                    int result = command.ExecuteNonQuery(); // Execute the INSERT statement

                    if (result > 0)
                    {
                        MessageBox.Show("Sign up successful!"); //
                        // Optionally, navigate to login window or main window
                        Login_RegistrationWindow loginWin = new Login_RegistrationWindow();
                        loginWin.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Sign up failed. Please try again.");
                    }
                }
            }
            catch (SqlException ex)
            {
                // Check for unique constraint violation (e.g., email already exists)
                if (ex.Number == 2627) //
                {
                    MessageBox.Show("Email already exists. Please use a different email address.");
                }
                else
                {
                    MessageBox.Show("Database error: " + ex.Message); //
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unexpected error occurred: " + ex.Message);
            }
        }

        private void ReturnHome_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}