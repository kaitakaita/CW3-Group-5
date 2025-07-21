using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace CW3_Group_5
{
    public partial class PaymentWindow : Window
    {
        private readonly string connectionString = @"Server=localhost\SQLEXPRESS;Database=HotelBookingDB;Trusted_Connection=True;";
        private decimal totalAmount;
        private int bookingId;
        private bool awaitingOtp = false;

        public PaymentWindow(decimal amount, int bookingId)
        {
            InitializeComponent();
            this.totalAmount = amount;
            this.bookingId = bookingId;
            TotalAmountTextBlock.Text = $"Total Amount: ₱{amount:F2}";
            ShowGcashInput();
        }

        private void ShowGcashInput()
        {
            PaymentDetailsGrid.Visibility = Visibility.Visible;
            GcashPanel.Visibility = Visibility.Visible;
            OtpPanel.Visibility = Visibility.Collapsed;
        }

        private void ShowOtpInput()
        {
            PaymentDetailsGrid.Visibility = Visibility.Visible;
            GcashPanel.Visibility = Visibility.Collapsed;
            OtpPanel.Visibility = Visibility.Visible;
        }

        private void ProcessPayment_Click(object sender, RoutedEventArgs e)
        {
            if (!awaitingOtp)
            {
                // Validate GCash number
                string gcashNumber = GcashNumberTextBox.Text.Trim();
                if (!Regex.IsMatch(gcashNumber, @"^\d{11}$"))
                {
                    MessageBox.Show("GCash number must be exactly 11 digits.", "Validation Error");
                    return;
                }
                awaitingOtp = true;
                ShowOtpInput();
                return;
            }
            else
            {
                // Validate OTP
                string otp = OtpTextBox.Text.Trim();
                if (!Regex.IsMatch(otp, @"^\d{6}$"))
                {
                    MessageBox.Show("OTP code must be exactly 6 digits.", "Validation Error");
                    return;
                }

                try
                {
                    string transactionId = GenerateTransactionId();

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        string query = @"INSERT INTO Payment (BookingID, Amount, PaymentMethod, PaymentStatus, TransactionID)
                     VALUES (@BookingID, @Amount, @PaymentMethod, @PaymentStatus, @TransactionID)";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@BookingID", bookingId);
                            command.Parameters.AddWithValue("@Amount", totalAmount);
                            command.Parameters.AddWithValue("@PaymentMethod", "GCash");
                            command.Parameters.AddWithValue("@PaymentStatus", "Paid");
                            command.Parameters.AddWithValue("@TransactionID", transactionId);

                            command.ExecuteNonQuery();

                            UpdateBookingStatus(connection, bookingId, "Paid");
                        }
                    }

                    MessageBox.Show($"Payment Successful!\nTransaction ID: {transactionId}", "Success");
                    ShowReceiptDetails(transactionId);
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Payment processing error: {ex.Message}", "Error");
                }
            }
        }

        private string GenerateTransactionId()
        {
            return $"TXN{DateTime.Now:yyyyMMddHHmmssfff}";
        }

        private void UpdateBookingStatus(SqlConnection connection, int bookingId, string status)
        {
            string updateQuery = "UPDATE Bookings SET Status = @Status WHERE BookingID = @BookingID";
            using (SqlCommand command = new SqlCommand(updateQuery, connection))
            {
                command.Parameters.AddWithValue("@Status", status);
                command.Parameters.AddWithValue("@BookingID", bookingId);
                command.ExecuteNonQuery();
            }
        }

        private void ShowReceiptDetails(string transactionId)
        {
            string receiptDetails = $"Receipt Details:\n\n" +
                                  $"Transaction ID: {transactionId}\n" +
                                  $"Date: {DateTime.Now}\n" +
                                  $"Amount: ₱{totalAmount:F2}\n" +
                                  $"Payment Method: GCash\n" +
                                  $"Status: Paid";

            MessageBox.Show(receiptDetails, "Payment Receipt");
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to cancel the payment?", "Confirm Cancel",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private int CreateBooking(Hotel hotel)
        {
            int bookingId = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"INSERT INTO Booking (HotelName, Location, Rate, Pax, BookingStatus, CreatedAt)
                                   OUTPUT INSERTED.BookingID
                                   VALUES (@HotelName, @Location, @Rate, @Pax, @BookingStatus, @CreatedAt)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@HotelName", hotel.Name);
                        command.Parameters.AddWithValue("@Location", hotel.Location);
                        command.Parameters.AddWithValue("@Rate", hotel.Rate);
                        command.Parameters.AddWithValue("@Pax", hotel.Pax);
                        command.Parameters.AddWithValue("@BookingStatus", "Pending");
                        command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                        // ExecuteScalar returns the first column of the first row
                        bookingId = (int)command.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating booking: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return bookingId;
        }

        private void BookNow_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Hotel selectedHotel)
            {
                // First create the booking
                int bookingId = CreateBooking(selectedHotel);
                
                if (bookingId > 0)
                {
                    // Now open payment window with valid bookingId
                    PaymentWindow paymentWindow = new PaymentWindow(selectedHotel.Rate, bookingId);
                    paymentWindow.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Failed to create booking. Please try again.");
                }
            }
            else
            {
                MessageBox.Show("Please select a hotel to book.");
            }
        }
    }
}