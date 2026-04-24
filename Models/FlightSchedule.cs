using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JetAdminSystem.Models
{
    public class FlightSchedule
    {
        [Key]
        public int ScheduleId { get; set; }

        [Required]
        public int AircraftId { get; set; }

        [Required]
        public int DepartureAirportId { get; set; } // Khóa ngoại tới Airport (Điểm đi)

        [Required]
        public int ArrivalAirportId { get; set; }   // Khóa ngoại tới Airport (Điểm đến)

        [Required]
        public DateTime DepartureTime { get; set; }

        [Required]
        public DateTime ArrivalTime { get; set; }

        [Required]
        [StringLength(50)]
        public string FlightStatus { get; set; } = "Scheduled"; // Scheduled, In-flight, Completed, Cancelled

        // --- Navigation Properties ---

        [ForeignKey("AircraftId")]
        public virtual Aircraft? Aircraft { get; set; }

        [ForeignKey("DepartureAirportId")]
        public virtual Airport? DepartureAirport { get; set; }

        [ForeignKey("ArrivalAirportId")]
        public virtual Airport? ArrivalAirport { get; set; }

        // Một lịch trình có thể có nhiều đơn đặt chỗ (Booking)
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}