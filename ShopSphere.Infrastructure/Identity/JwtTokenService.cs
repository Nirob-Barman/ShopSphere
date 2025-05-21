
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ShopSphere.Application.DTOs.Auth;
using ShopSphere.Application.Interfaces;
using ShopSphere.Application.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ShopSphere.Infrastructure.Identity
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly JwtSettings _jwtSettings;

        public JwtTokenService(IOptions<JwtSettings> jwtOptions)
        {
            _jwtSettings = jwtOptions.Value;
        }

        public string GenerateAccessToken(JwtUserInfo user, IList<string> roles, out DateTime expiresAt)
        {
            // PAYLOAD: This is where claims are added — they represent the payload of the token.
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.UserName!)
            };

            // Add user roles to the claims (part of the payload)
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            // SECRET KEY: Used to sign the token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key!));

            // This sets the signing algorithm (HS256)
            // This info becomes part of the JWT **header**
            // The actual digital signature is generated using this key and algorithm
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Set expiration time (used in the payload as "exp" claim)
            expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: creds);

            // ENCODE AND RETURN: Converts the token into a string — base64url(header).base64url(payload).base64url(signature)
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(bytes);
        }
    }
}
