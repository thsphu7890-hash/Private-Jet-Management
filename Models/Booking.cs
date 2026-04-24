using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JetAdminSystem.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        // --- Liên kết thực thể ---
        [Required]
        public int PassengerId { get; set; }

        [Required]
        public int ScheduleId { get; set; }

        // BrokerId để null nếu khách làm việc trực tiếp với hãng
        public int? BrokerId { get; set; }

        // --- Thông tin vận hành nội bộ ---

        [Required]
        public int SeatCount { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal BasePrice { get; set; } // Giá gốc tại thời điểm đặt

        [Column(TypeName = "decimal(18,2)")]
        public decimal Surcharge { get; set; } // Phụ phí (ăn uống, xe đưa đón sân bay)

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; } // = BasePrice + Surcharge

        [Required]
        [StringLength(50)]
        // Status: Pending (Chờ duyệt), Confirmed (Đã xác nhận), InFlight (Đang bay), Completed, Cancelled
        public string Status { get; set; } = "Pending";

        [Required]
        public DateTime BookingDate { get; set; } = DateTime.Now;

        public string? InternalNotes { get; set; } // Ghi chú nội bộ cho nhân viên điều hành

        // --- Navigation Properties ---
        [ForeignKey("PassengerId")]
        public virtual Passenger? Passenger { get; set; }

        [ForeignKey("ScheduleId")]
        public virtual FlightSchedule? Schedule { get; set; }

        [ForeignKey("BrokerId")]
        public virtual Broker? Broker { get; set; }

        // Quan hệ 1-1 với Billing để kiểm soát dòng tiền
        public virtual Billing? Billing { get; set; }
    }
}