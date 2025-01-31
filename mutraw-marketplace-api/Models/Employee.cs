using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace mutraw_marketplace_api.Models;

public class Employee : IdentityUser
{
    [Required]
    [MinLength(2)]
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;
    [Required]
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
}