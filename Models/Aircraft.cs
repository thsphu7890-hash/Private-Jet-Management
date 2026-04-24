using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization; // Để xử lý vòng lặp JSON

namespace JetAdminSystem.Models
{
    public class Aircraft
    {
        [Key]
        public int AircraftId { get; set; }

        [Required(ErrorMessage = "Tên chuyên cơ không được để trống")]
        [StringLength(100)]
        public string AircraftName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Model máy bay là bắt buộc")]
        [StringLength(50)]
        public string Model { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số hiệu đuôi (Tail Number) là bắt buộc")]
        [StringLength(20)]
        public string TailNumber { get; set; } = string.Empty;

        public int CategoryId { get; set; }

        [Range(1, 500, ErrorMessage = "Sức chứa phải từ 1 đến 500 khách")]
        public int Capacity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá thuê không được âm")]
        public decimal PricePerHour { get; set; }

        // Link ảnh từ Cloudinary
        public string? ExteriorImageUrl { get; set; }
        public string? InteriorImageUrl { get; set; }

        [Required]
        public string Status { get; set; } = "Ready"; // Nên có giá trị mặc định an toàn

        [NotMapped]
        [JsonIgnore] // Tránh lỗi khi tuần tự hóa JSON gửi về React
        public IFormFile? ExteriorFile { get; set; }

        [NotMapped]
        [JsonIgnore]
        public IFormFile? InteriorFile { get; set; }

        [ForeignKey("CategoryId")]
        public virtual AircraftCategory? Category { get; set; }

        [JsonIgnore] // Ngăn vòng lặp vô tận (Circular Reference) khi lấy dữ liệu kèm lịch bay
        public virtual ICollection<FlightSchedule> FlightSchedules { get; set; } = new List<FlightSchedule>();
    }
}