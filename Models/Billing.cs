using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JetAdminSystem.Models
{
    [Table("Billings")]
    public class Billing
    {
        [Key]
        public int BillingId { get; set; }

        [Required]
        public int BookingId { get; set; }

        // --- Chi tiết tiền nông ---

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal SubTotal { get; set; } // Tiền dịch vụ gốc

        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxAmount { get; set; } // Tiền thuế (thường là 10%)

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal GrandTotal { get; set; } // Tổng tiền cuối cùng khách phải trả

        [Column(TypeName = "decimal(18,2)")]
        public decimal BrokerCommission { get; set; } // Tiền hoa hồng cho môi giới

        // --- Trạng thái và phương thức ---

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = "Bank Transfer";

        [Required]
        [StringLength(50)]
        public string PaymentStatus { get; set; } = "Unpaid";

        public string? TransactionReference { get; set; } // Mã giao dịch / Ảnh hóa đơn

        public string? InvoiceImageUrl { get; set; } // Link ảnh từ Cloudinary

        public DateTime? PaymentDate { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // --- Liên kết thực thể ---
        [ForeignKey("BookingId")]
        public virtual Booking? Booking { get; set; }
    }
}