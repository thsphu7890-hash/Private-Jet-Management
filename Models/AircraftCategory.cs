using System.ComponentModel.DataAnnotations;

namespace JetAdminSystem.Models
{
    public class AircraftCategory
    {
        [Key]
        public int CategoryId { get; set; } // Khóa chính

        [Required(ErrorMessage = "Tên phân loại không được để trống")]
        [StringLength(100)]
        public string CategoryName { get; set; } // Vd: VIP Airliner, Light Jet

        [StringLength(500)]
        public string? Description { get; set; } // Mô tả chi tiết về loại chuyên cơ này

        // --- Navigation Properties ---

        // Một phân loại có thể chứa nhiều máy bay (Quan hệ 1-N)
        // Khởi tạo List trống để tránh lỗi NullReferenceException khi truy cập
        public virtual ICollection<Aircraft> Aircrafts { get; set; } = new List<Aircraft>();
    }
}