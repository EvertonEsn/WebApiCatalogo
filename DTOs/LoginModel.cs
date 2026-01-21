using System.ComponentModel.DataAnnotations;

namespace APICatalogo.DTOs;

public class LoginModel
{
    [Required(ErrorMessage = "User Name Is Required")]
    public string? Username { get; set; }
	
    [Required(ErrorMessage = "Password Is Required")]
    public string? Password { get; set; }
}