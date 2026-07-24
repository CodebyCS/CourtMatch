using System.ComponentModel.DataAnnotations;

namespace Identity.API.DTOs;

public class UserLoginRequest
{
    [Required]
    [EmailAddress]
    public string Email {get; set;} = string.Empty;

    [Required]
    public string Password {get; set;} = string.Empty;
}

public class UserLoginResponse
{
    public string Token {get; set;} = string.Empty;
    public DateTime Expiration {get; set;}
}