using System;
using System.Windows;
using System.Windows.Controls;

namespace CW3_Group_5
{
    /// <summary>
    /// Interaction logic for Login_RegistrationWindow.xaml
    /// </summary>
    public partial class Login_RegistrationWindow : Window
    {
        public Login_RegistrationWindow()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Optional, remove if not needed
        }

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

        }
        private void ReturnHome_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close(); // closes the current window
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;

            if (email == "test123" && password == "test123")
            {
                BookHotelWindow bookHotelWindow = new BookHotelWindow();
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

