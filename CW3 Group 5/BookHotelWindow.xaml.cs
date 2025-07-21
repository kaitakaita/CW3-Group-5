using System; // Added for DateTime
using System.Collections.Generic;
using CW3_Group_5.Models; // Ensure your Hotel model is correctly defined here
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CW3_Group_5
{
    public partial class BookHotelWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // --- Database Connection String ---
        private readonly string connectionString = @"Server=localhost\SQLEXPRESS;Database=HotelBookingDB;Trusted_Connection=True;"; // Ensure this matches your DB setup

        // Assuming you'll have a way to get the current logged-in user's ID
        // For demonstration, I'll use a placeholder. You need to replace this.
        public int CurrentUserId { get; set; } = 2; // IMPORTANT: Replace with actual logged-in UserID (e.g., from App.cs or Login)

        public List<string> Destinations { get; } = new List<string> { "Manila", "Cebu", "Tagaytay", "Bulacan" };
        public List<string> GuestOptions { get; } = new List<string> { "1 Guest", "2 Guests", "3 Guests", "4 Guests", "5 Guests" };
        public List<string> SortOptions { get; } = new List<string> { "Highest Rated", "Lowest Price", "Most Pax" };
        public List<string> FilterOptions { get; } = new List<string> { "All Hotels", "2 Pax", "3 Pax", "4 Pax", "5 Pax" };

        private string _selectedDestination = "Manila";
        public string SelectedDestination
        {
            get => _selectedDestination;
            set
            {
                _selectedDestination = value;
                OnPropertyChanged(nameof(SelectedDestination));
                UpdateFilteredHotels();
            }
        }

        private string _selectedGuests = "2 Guests";
        public string SelectedGuests
        {
            get => _selectedGuests;
            set
            {
                _selectedGuests = value;
                OnPropertyChanged(nameof(SelectedGuests));
            }
        }

        private string _selectedSort = "Highest Rated";
        public string SelectedSort
        {
            get => _selectedSort;
            set
            {
                _selectedSort = value;
                OnPropertyChanged(nameof(SelectedSort));
                UpdateFilteredHotels();
            }
        }

        private string _selectedFilter = "All Hotels";
        public string SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                _selectedFilter = value;
                OnPropertyChanged(nameof(SelectedFilter));
                UpdateFilteredHotels();
            }
        }

        // IMPORTANT: These Hotel objects are static dummy data.
        // In a real application, you'd likely fetch hotels/rooms from the database.
        private List<Hotel> hotels = new List<Hotel>
        {
            // Ensure HotelID_DB, RoomTypeID_DB, RoomID_DB match your DB's initial INSERTs for Rooms/Hotels
            new Hotel { Name = "Sunrise Inn", Location = "Manila", Rate = 1500, Pax = 2, StarRating = 4.5, HotelID_DB = 1, RoomTypeID_DB = 1, RoomID_DB = 1 },
            new Hotel { Name = "Ocean View", Location = "Manila", Rate = 2200, Pax = 4, StarRating = 4.8, HotelID_DB = 1, RoomTypeID_DB = 2, RoomID_DB = 2 },
            new Hotel { Name = "Metro Stay", Location = "Manila", Rate = 1800, Pax = 3, StarRating = 4.3, HotelID_DB = 2, RoomTypeID_DB = 1, RoomID_DB = 3 },
            new Hotel { Name = "City Center Hotel", Location = "Cebu", Rate = 2000, Pax = 2, StarRating = 4.0, HotelID_DB = 2, RoomTypeID_DB = 3, RoomID_DB = 4 },
            // Add more as needed, linking to actual HotelID, RoomTypeID, RoomID from your DB if you want to use them directly.
            // For a real app, you'd dynamically load these from your database's Hotel and Room tables.
        };

        private List<Hotel> _filteredHotels;
        public List<Hotel> FilteredHotels
        {
            get => _filteredHotels;
            set
            {
                _filteredHotels = value;
                OnPropertyChanged(nameof(FilteredHotels));
            }
        }

        public BookHotelWindow()
        {
            DataContext = this;
            InitializeComponent();
            UpdateFilteredHotels();
        }

        private void UpdateFilteredHotels()
        {
            var filtered = hotels.Where(h => h.Location == SelectedDestination);

            if (SelectedFilter != "All Hotels")
            {
                if (int.TryParse(SelectedFilter.Split(' ')[0], out int pax))
                    filtered = filtered.Where(h => h.Pax == pax);
            }

            filtered = SelectedSort switch
            {
                "Highest Rated" => filtered.OrderByDescending(h => h.StarRating),
                "Lowest Price" => filtered.OrderBy(h => h.Rate),
                "Most Pax" => filtered.OrderByDescending(h => h.Pax),
                _ => filtered
            };

            FilteredHotels = filtered.ToList();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (DestinationComboBox.SelectedItem is string dest)
            {
                SelectedDestination = dest;
            }
            UpdateFilteredHotels();
        }

        private void BookNow_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Hotel selectedHotel)
            {
                // You need to get CheckIn and CheckOut dates from your UI, e.g., DatePicker controls
                // For this example, I'll use placeholders. Replace with actual UI element values.
                // Example: DateTime checkInDate = YourCheckInDatePicker.SelectedDate.GetValueOrDefault();
                // Example: DateTime checkOutDate = YourCheckOutDatePicker.SelectedDate.GetValueOrDefault();
                DateTime checkInDate = DateTime.Now.Date.AddDays(7); // Placeholder
                DateTime checkOutDate = DateTime.Now.Date.AddDays(10); // Placeholder

                // Input validation for dates
                if (checkInDate == null || checkOutDate == null || checkInDate >= checkOutDate)
                {
                    MessageBox.Show("Please select valid check-in and check-out dates.", "Date Selection Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Crucial: Check if the RoomID_DB is valid before proceeding
                if (selectedHotel.RoomID_DB == 0) // Assuming 0 means not set/invalid
                {
                    MessageBox.Show("Selected hotel does not have a valid RoomID for booking. Please check your data setup.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int newBookingId = -1; // Initialize with an invalid ID

                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        string insertBookingQuery = @"
                            INSERT INTO Booking (UserID, RoomID, CheckInDate, CheckOutDate, TotalPrice, BookingStatus)
                            OUTPUT INSERTED.BookingID -- This is crucial to get the generated BookingID
                            VALUES (@UserID, @RoomID, @CheckInDate, @CheckOutDate, @TotalPrice, @BookingStatus)";

                        using (SqlCommand command = new SqlCommand(insertBookingQuery, connection))
                        {
                            command.Parameters.AddWithValue("@UserID", CurrentUserId); // Use the logged-in user's ID
                            command.Parameters.AddWithValue("@RoomID", selectedHotel.RoomID_DB); // Use the actual RoomID from selected hotel/room
                            command.Parameters.AddWithValue("@CheckInDate", checkInDate);
                            command.Parameters.AddWithValue("@CheckOutDate", checkOutDate);
                            command.Parameters.AddWithValue("@TotalPrice", selectedHotel.Rate); // Use the hotel's rate as total price
                            command.Parameters.AddWithValue("@BookingStatus", "Pending"); // Initial status

                            // ExecuteScalar returns the first column of the first row returned by the query
                            newBookingId = (int)command.ExecuteScalar();
                        }
                    }

                    if (newBookingId > 0)
                    {
                        MessageBox.Show($"Booking created successfully! Booking ID: {newBookingId}", "Booking Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Now, pass the *actual* newBookingId to the PaymentWindow
                        PaymentWindow paymentWindow = new PaymentWindow(selectedHotel.Rate, newBookingId);
                        paymentWindow.ShowDialog(); // Use ShowDialog to make it modal

                        // You might want to refresh the hotel list or update booking status visually here
                        // after the payment window closes.
                    }
                    else
                    {
                        MessageBox.Show("Failed to create booking. No Booking ID was returned.", "Booking Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (SqlException sqlEx)
                {
                    MessageBox.Show($"Database error during booking: {sqlEx.Message}\nError Code: {sqlEx.Number}", "Booking Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An unexpected error occurred during booking: {ex.Message}", "Booking Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a hotel to book.");
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}