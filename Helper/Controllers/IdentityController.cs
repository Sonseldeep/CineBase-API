using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Helper.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class IdentityController : ControllerBase
{
    private const string TokenSecret = "ForTheLoveOfGodChangeThisInProductionAndLoadSecurely";
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(8);

    [HttpPost("token")]

    public IActionResult GenerateToken(
        [FromBody] TokenGenerationRequest request)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(TokenSecret);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.CreateVersion7().ToString()),
            new(JwtRegisteredClaimNames.Sub, request.Email),
            new(JwtRegisteredClaimNames.Email, request.Email),
            new("userid", request.UserId.ToString()),
        };
        
        claims.AddRange(request.CustomClaims
            .Select(claimPair => new { claimPair, jsonElement = (JsonElement)claimPair.Value })
            .Select(@t => new
            {
                @t,
                valueType = @t.jsonElement.ValueKind switch
                {
                    JsonValueKind.True or JsonValueKind.False => ClaimValueTypes.Boolean,
                    JsonValueKind.Number => ClaimValueTypes.Double,
                    _ => ClaimValueTypes.String
                }
            })
            .Select(@t => new Claim(@t.@t.claimPair.Key, @t.@t.claimPair.Value.ToString()!, @t.valueType)));
        
            var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.Add(TokenLifetime),
                    IssuedAt = DateTime.UtcNow,
                    Issuer = "https://localhost:7001",
                    Audience = "https://localhost:5001",
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                    
                };
                
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);
            return Ok(jwt);
                
        
    }
}