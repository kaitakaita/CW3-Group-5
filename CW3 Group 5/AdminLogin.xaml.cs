using System.Windows;
using System.Windows.Controls;

namespace CW3_Group_5
{
    public partial class AdminLogin : Window
    {
        // Hardcoded admin credentials
        private const string adminEmail = "admin@example.com";
        private const string adminPassword = "admin123";

        public AdminLogin()
        {
            InitializeComponent();
        }

        private void AdminLogin_Click(object sender, RoutedEventArgs e)
        {
            string emailInput = AdminEmailTextBox.Text.Trim();
            string passwordInput = AdminPasswordBox.Password;

            if (emailInput == adminEmail && passwordInput == adminPassword)
            {
                MessageBox.Show("Login successful!", "Admin Login", MessageBoxButton.OK, MessageBoxImage.Information);
                // You can open another window here if needed in the future
            }
            else
            {
                MessageBox.Show("Invalid email or password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AdminEmailTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            EmailPlaceholder.Visibility = string.IsNullOrEmpty(AdminEmailTextBox.Text) ? Visibility.Visible : Visibility.Hidden;
        }

        private void AdminPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordPlaceholder.Visibility = string.IsNullOrEmpty(AdminPasswordBox.Password) ? Visibility.Visible : Visibility.Hidden;
        }
    }
}



