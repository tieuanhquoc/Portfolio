using System.ComponentModel.DataAnnotations;

namespace MainApp.Models;

public class LoginModel
{
    [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
    public string Password { get; set; }
}