using System.ComponentModel.DataAnnotations;

namespace Common.DTOs.UserModel
{
    public class LoginInput
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class ResetPasswordInput
    {
        [StringLength(255)]
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }
        [StringLength(255)]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
    }

    public class ChangePasswordInput
    {
        [StringLength(255)]
        [Required(ErrorMessage = "CurrentPassword is required")]
        public string CurrentPassword { get; set; }
        [StringLength(255)]
        [Required(ErrorMessage = "NewPassword is required")]
        public string NewPassword { get; set; }
    }
}