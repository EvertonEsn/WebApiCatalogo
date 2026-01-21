using System.ComponentModel.DataAnnotations;

namespace APICatalogo.DTOs;

public class RegisterModel
{
    [Required(ErrorMessage = "User Name Is Required")]
    public string? Username { get; set; }
    
    [EmailAddress]
    [Required(ErrorMessage = "Email Is Required")]
    public string? Email { get; set; }
    
    [Required(ErrorMessage = "Password Is Required")]
    public string? Password { get; set; }
}