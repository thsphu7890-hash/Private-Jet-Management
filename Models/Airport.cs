using System.ComponentModel.DataAnnotations;

namespace JetAdminSystem.Models
{
    public class Airport
    {
        [Key]
        public int AirportId { get; set; }

        [Required(ErrorMessage = "Tên sân bay không được để trống")]
        [StringLength(200)]
        public string AirportName { get; set; } // Ví dụ: Sân bay quốc tế Nội Bài

        [Required(ErrorMessage = "Mã IATA là bắt buộc")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Mã IATA phải đúng 3 ký tự")]
        public string IATACode { get; set; } // Ví dụ: HAN, SGN, DAD

        [Required]
        [StringLength(100)]
        public string City { get; set; } // Thành phố: Hà Nội, TP.HCM

        [Required]
        [StringLength(100)]
        public string Country { get; set; } // Quốc gia: Việt Nam
    }
}