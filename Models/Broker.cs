using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace JetAdminSystem.Models
{
    public class Broker
    {
        [Key]
        public int BrokerId { get; set; }

        [Required(ErrorMessage = "Tên công ty môi giới là bắt buộc")]
        [StringLength(150)]
        public string BrokerName { get; set; }

        [StringLength(500)]
        public string? AvatarUrl { get; set; } // <--- THÊM DÒNG NÀY

        [StringLength(100)]
        public string? ContactPerson { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [Range(0, 100)]
        [Column(TypeName = "decimal(5,2)")]
        public decimal CommissionRate { get; set; }

        public string? Address { get; set; }

        [JsonIgnore] // Tránh lỗi vòng lặp khi trả về JSON
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}