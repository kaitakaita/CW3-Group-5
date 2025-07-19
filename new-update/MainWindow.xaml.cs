using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Media.Imaging;

namespace CW3_Group_5
{
    public partial class MainWindow : Window
    {
        private readonly List<string> imagePaths = new List<string>
        {
            "/Images/HotelImg1.jpg",
            "/Images/HotelImg2.jpg",
            "/Images/HotelImg3.jpg"
        };

        private int currentImageIndex = 0;
        private DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
            StartSlideshow();
        }

        private void StartSlideshow()
        {
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            timer.Tick += Timer_Tick;
            timer.Start();
            ShowImage(currentImageIndex);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            currentImageIndex = (currentImageIndex + 1) % imagePaths.Count;
            ShowImage(currentImageIndex);
        }

        private void ShowImage(int index)
        {
            Uri uri = new Uri(imagePaths[index], UriKind.Relative);
            SlideshowImage.Source = new BitmapImage(uri);
        }

        private void BookNow_Click(object sender, RoutedEventArgs e)
        {
            Login_RegistrationWindow loginWindow = new Login_RegistrationWindow();
            loginWindow.Show();
            this.Close(); // Optional
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}






