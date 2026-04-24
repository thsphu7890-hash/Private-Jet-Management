using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace JetAdminSystem.Models
{
    public class Account
    {
        [Key]
        public int AccountId { get; set; }

        [Required(ErrorMessage = "Username là bắt buộc")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password là bắt buộc")]
        // Cấu hình: Chỉ bỏ qua khi ghi (Write - tức là khi Server trả về kết quả JSON)
        // Nhưng vẫn cho phép đọc (Read - tức là khi nhận dữ liệu từ Request)
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Password { get; set; }

        public string Role { get; set; } = "Staff";
    }
}