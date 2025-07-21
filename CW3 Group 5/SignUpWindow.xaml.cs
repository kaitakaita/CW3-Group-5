using System;
using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;
using System.Security.Cryptography; // ADD THIS FOR HASHING
using System.Text;                  // ADD THIS FOR HASHING

namespace CW3_Group_5
{
    public partial class SignUpWindow : Window
    {
        private readonly string connectionString =
            @"Server=localhost\SQLEXPRESS;Database=HotelBookingDB;Trusted_Connection=True;";

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
            string fullName = FullNameTextBox.Text.Trim();
            string email = EmailTextBox.Text.Trim();
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

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

            // --- HASH THE PASSWORD BEFORE STORING ---
            string passwordHash = ComputeSha256Hash(password);

            int roleId = 2; // Default role ID for new users

            string query = "INSERT INTO Users (FirstName, LastName, Email, PasswordHash, RoleID) " +
                           "VALUES (@FirstName, @LastName, @Email, @PasswordHash, @RoleID)";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FirstName", firstName);
                    command.Parameters.AddWithValue("@LastName", lastName);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@PasswordHash", passwordHash); // Store the hashed password
                    command.Parameters.AddWithValue("@RoleID", roleId);

                    connection.Open();
                    int result = command.ExecuteNonQuery();

                    if (result > 0)
                    {
                        MessageBox.Show("Sign up successful!");
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
                if (ex.Number == 2627) // Unique constraint violation (e.g., email already exists)
                {
                    MessageBox.Show("Email already exists. Please use a different email address.");
                }
                else
                {
                    MessageBox.Show("Database error: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unexpected error occurred: " + ex.Message);
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

        private void ReturnHome_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}