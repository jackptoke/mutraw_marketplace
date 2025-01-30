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
        var cred = new Credential
        {
            Username = credential.Username,
            Password = credential.Password
        };
        
        var result = await credentialRepo.AuthenticateAsync(cred);
        if (result)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, cred.Username),
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
    public async Task<IActionResult> Register([FromBody] Credential credential)
    {
        await credentialRepo.CreateAsync(credential);
        return Created();
    }

    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetAllAsync()
    {
        var credentials = await credentialRepo.GetAllAsync();
        var credDtos = credentials.Select(cred => new CredentialDto
        {
            Username = cred.Username,
        }).ToList();
        return Ok(credDtos);
    }
    
    [HttpPut("{username}")]
    [Authorize(Policy = "OwnerOrAdminPolicy")]
    public async Task<IActionResult> Update([FromRoute]string username, [FromBody] CredentialUpdateDto credential)
    {
        // Check the current password
        var cred = new Credential
        {
            Username = username,
            Password = credential.OldPassword
        };
        var result = await credentialRepo.AuthenticateAsync(cred);
        // If failed, return Unauthorised.
        if (!result) return Unauthorized();
        // If success, update the password.
        var newCred = new Credential
        {
            Username = credential.Username,
            Password = credential.NewPassword
        };
        await credentialRepo.UpdateAsync(newCred);
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