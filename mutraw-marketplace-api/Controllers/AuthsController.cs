using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using mutraw_marketplace_api.Dtos;
using mutraw_marketplace_api.Models;
using mutraw_marketplace_api.Repositories;

namespace mutraw_marketplace_api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class AuthsController(ICredentialRepo credentialRepo, IConfiguration configuration) : ControllerBase
{

    [HttpPost("authenticate")]
    public async Task<IActionResult> Authenticate([FromBody] CredentialCreateDto credential)
    {
        var cred = new Employee
        {
            UserName = credential.Username,
            Email = credential.Email,
            PasswordHash = credential.Password,
            FirstName = credential.FirstName,
            LastName = credential.LastName,
            DateOfBirth = credential.DateOfBirth
        };
        
        var result = await credentialRepo.AuthenticateAsync(cred);
        if (result)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, cred.UserName),
                new(ClaimTypes.Email, cred.Email),
                new(ClaimTypes.Role, "User"),
                // new Claim("Admin", "true"),
                // new Claim("Manager", "true")
            };
            var expiresAt = DateTime.Now.AddDays(1);
            
            return Ok(new
            {
                access_token = GenerateJwtToken(claims, expiresAt),
                expires_at = expiresAt,
                message = "Successfully authenticated"
            });
        }
        return Unauthorized(new { message = "Username or password is incorrect" });
    }

    private string GenerateJwtToken(IEnumerable<Claim> claims, DateTime expiresAt)
    {
        var secret = Encoding.ASCII.GetBytes(configuration.GetValue<string>("SecretKey") ?? "");
        var jwt = new JwtSecurityToken(
            claims: claims, 
            notBefore: DateTime.Now,
            expires: expiresAt, 
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(secret),
                SecurityAlgorithms.HmacSha256Signature
            ));
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] Employee employee)
    {
        await credentialRepo.CreateAsync(employee);
        return Created();
    }

    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetAllAsync()
    {
        var credentials = await credentialRepo.GetAllAsync();
        var credDtos = credentials.Select(cred => new CredentialDto
        {
            Username = cred.UserName ?? "",
        }).ToList();
        return Ok(credDtos);
    }
    
    [HttpPut("{username}/password")]
    [Authorize(Policy = "OwnerOrAdminPolicy")]
    public async Task<IActionResult> UpdatePassword([FromRoute]string username, [FromBody] CredentialPasswordUpdateDto credential)
    {
        // Check the current password
        var cred = new Employee
        {
            UserName = username,
            PasswordHash = credential.OldPassword
        };
        var result = await credentialRepo.AuthenticateAsync(cred);
        // If failed, return Unauthorised.
        if (!result) return Unauthorized();
        // If success, update the password.
        var newCred = new Employee
        {
            UserName = credential.Username,
            PasswordHash = credential.NewPassword
        };
        await credentialRepo.UpdateAsync(newCred);
        return NoContent();
    }
    
    [HttpPut("{username}")]
    [Authorize(Policy = "OwnerOrAdminPolicy")]
    public async Task<IActionResult> Update([FromRoute]string username, [FromBody] CredentialUpdateDto credential)
    {
        // Check the current password
        var employee = await credentialRepo.GetByUsernameAsync(username);
        
        if(employee == null) return NotFound();
        
        // If success, update the password.
        employee.FirstName = credential.FirstName;
        employee.LastName = credential.LastName;
        employee.DateOfBirth = credential.DateOfBirth;
        employee.Email = credential.Email;
        
        await credentialRepo.UpdateAsync(employee);
        return NoContent();
    }

    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> Delete([FromRoute] string username)
    {
        await credentialRepo.DeleteAsync(username);
        return NoContent();
    }
}