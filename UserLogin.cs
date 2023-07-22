using System.ComponentModel.DataAnnotations;

namespace Lesson11.Models;

public class UserLogin
{
    [Required(ErrorMessage = "Please enter User ID")]
    public string UserID { get; set; } = null!;

    [Required(ErrorMessage = "Please enter Password")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;
    public bool RememberMe { get; set; }
}
// 21011435 Damien Foo (for C#)