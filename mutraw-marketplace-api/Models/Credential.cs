using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace mutraw_marketplace_api.Models;

public class Credential
{
    public long Id { get; set; }
    [Required]
    [Display(Name = "Username")]
    public string Username { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
}