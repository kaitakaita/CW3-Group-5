using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CW3_Group_5
{
    public partial class BookHotelWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public class Hotel
        {
            public string Name { get; set; }
            public string Location { get; set; }
            public decimal Rate { get; set; }
            public int Pax { get; set; }
            public double StarRating { get; set; }
        }

        // Data sources for ComboBoxes
        public List<string> Destinations { get; } = new List<string> { "Manila", "Cebu", "Tagaytay", "Bulacan" };
        public List<string> GuestOptions { get; } = new List<string> { "1 Guest", "2 Guests", "3 Guests", "4 Guests", "5 Guests" };
        public List<string> SortOptions { get; } = new List<string> { "Highest Rated", "Lowest Price", "Most Pax" };
        public List<string> FilterOptions { get; } = new List<string> { "All Hotels", "2 Pax", "3 Pax", "4 Pax", "5 Pax" };

        // Selected values
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

        // Hotel data
        private List<Hotel> hotels = new List<Hotel>
        {
            // Manila
            new Hotel { Name = "Sunrise Inn", Location = "Manila", Rate = 1500, Pax = 2, StarRating = 4.5 },
            new Hotel { Name = "Ocean View", Location = "Manila", Rate = 2200, Pax = 4, StarRating = 4.8 },
            new Hotel { Name = "Metro Stay", Location = "Manila", Rate = 1800, Pax = 3, StarRating = 4.3 },

            // Cebu
            new Hotel { Name = "City Center Hotel", Location = "Cebu", Rate = 2000, Pax = 2, StarRating = 4.0 },
            new Hotel { Name = "Beachside Resort", Location = "Cebu", Rate = 2500, Pax = 5, StarRating = 4.7 },
            new Hotel { Name = "Cebu Grand", Location = "Cebu", Rate = 2100, Pax = 3, StarRating = 4.2 },

            // Tagaytay
            new Hotel { Name = "Taal Vista", Location = "Tagaytay", Rate = 3000, Pax = 2, StarRating = 4.6 },
            new Hotel { Name = "Skyline Hotel", Location = "Tagaytay", Rate = 2800, Pax = 4, StarRating = 4.4 },
            new Hotel { Name = "Tagaytay Suites", Location = "Tagaytay", Rate = 2500, Pax = 3, StarRating = 4.5 },

            // Bulacan
            new Hotel { Name = "Bulacan Paradise", Location = "Bulacan", Rate = 1700, Pax = 2, StarRating = 4.1 },
            new Hotel { Name = "Garden Hotel", Location = "Bulacan", Rate = 1600, Pax = 3, StarRating = 4.0 },
            new Hotel { Name = "Bulacan Royal", Location = "Bulacan", Rate = 1800, Pax = 4, StarRating = 4.3 }
        };

        // Filtered hotel list for display
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

            // Filter by Pax if not "All Hotels"
            if (SelectedFilter != "All Hotels")
            {
                if (int.TryParse(SelectedFilter.Split(' ')[0], out int pax))
                    filtered = filtered.Where(h => h.Pax == pax);
            }

            // Sort
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

        private void LocationButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                SelectedDestination = btn.Content.ToString();
                DestinationComboBox.SelectedItem = SelectedDestination;
                UpdateFilteredHotels();
            }
        }

        // PropertyChanged helper
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}