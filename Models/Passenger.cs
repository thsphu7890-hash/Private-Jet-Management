using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JetAdminSystem.Models
{
    public class Passenger
    {
        [Key]
        public int PassengerId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên hành khách")]
        [StringLength(100)]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số Passport không được để trống")]
        [StringLength(20)]
        [Display(Name = "Số hộ chiếu")]
        public string PassportNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn ngày hết hạn hộ chiếu")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày hết hạn Passport")]
        public DateTime PassportExpiry { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập quốc tịch")]
        [StringLength(50)]
        [Display(Name = "Quốc tịch")]
        public string Nationality { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập ngày sinh")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public DateTime DateOfBirth { get; set; }

        // --- PHẦN HÌNH ẢNH (Lưu URL từ Cloudinary) ---

        [Display(Name = "Ảnh chân dung")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Bản scan Passport")]
        public string? PassportScanUrl { get; set; }

        // --- THÔNG TIN LIÊN HỆ ---

        [StringLength(15)]
        [Phone(ErrorMessage = "Số điện thoại không đúng định dạng")]
        [Display(Name = "Số điện thoại")]
        public string? PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        // --- NAVIGATION PROPERTY ---

        // Một hành khách có thể có nhiều đơn đặt chỗ (Bookings)
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}