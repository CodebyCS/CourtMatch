using System.ComponentModel.DataAnnotations;

namespace Identity.API.DTOs;

public class UserRegisterRequest
{
    [Required]
    public string FullName {get; set;} = string.Empty;

    [Required]
    [EmailAddress]
    public string Email {get; set;} = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password {get; set;} = string.Empty;
}

public class UserSearchResult
{
    public string Id {get; set;} = string.Empty;
    public string FullName {get; set;} = string.Empty;
    public string Email {get; set;} = string.Empty;
}