namespace CW3_Group_5.Models
{
    public class Hotel
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public decimal Rate { get; set; }
        public int Pax { get; set; }
        public double StarRating { get; set; }

        // --- ADD THESE NEW PROPERTIES ---
        public int HotelID_DB { get; set; }     // Corresponds to HotelID in DB's Hotel table
        public int RoomTypeID_DB { get; set; }  // Corresponds to RoomTypeID in DB's RoomType table
        public int RoomID_DB { get; set; }      // Corresponds to RoomID in DB's Room table (CRUCIAL for Booking)
        // --------------------------------
    }
}