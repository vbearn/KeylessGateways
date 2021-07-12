using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.IdentityModel.Tokens;

namespace KeylessGateways.Identity.Infrastructure
{
    public interface IJwtAuthManager
    {
        JwtAuthResult GenerateTokens(string username, Claim[] claims, DateTime now);
        JwtAuthResult Refresh( string accessToken, DateTime now);
        (ClaimsPrincipal, JwtSecurityToken) DecodeJwtToken(string token);
    }

    public class JwtAuthManager : IJwtAuthManager
    {
        private readonly JwtTokenConfig _jwtTokenConfig;
        private readonly byte[] _secret;

        public JwtAuthManager(JwtTokenConfig jwtTokenConfig)
        {
            _jwtTokenConfig = jwtTokenConfig;
            _secret = Encoding.ASCII.GetBytes(jwtTokenConfig.Secret);
        }


        public JwtAuthResult GenerateTokens(string username, Claim[] claims, DateTime now)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key =  _secret ;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(token);

            return new JwtAuthResult
            {
                UserName = username,
                AccessToken = accessToken,
                ExpireAt = now.AddMinutes(_jwtTokenConfig.RefreshTokenExpiration)
            };
        }

        public JwtAuthResult Refresh( string accessToken, DateTime now)
        {
            var (principal, jwtToken) = DecodeJwtToken(accessToken);
            if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature))
            {
                throw new SecurityTokenException("Invalid token");
            }

            var userName = principal.Identity?.Name;

            return GenerateTokens(userName, principal.Claims.ToArray(), now); // need to recover the original claims
        }

        public (ClaimsPrincipal, JwtSecurityToken) DecodeJwtToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new SecurityTokenException("Invalid token");
            }
            var principal = new JwtSecurityTokenHandler()
                .ValidateToken(token,
                    new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(_secret),
                        ValidateAudience = false,
                        ValidateLifetime = true,
                    },
                    out var validatedToken);
            return (principal, validatedToken as JwtSecurityToken);
        }

    }

    public class JwtAuthResult
    {
        public string UserName { get; set; }    
        public string AccessToken { get; set; }

        public DateTime ExpireAt { get; set; }
    }

}
