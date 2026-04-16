using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using Serilog;
using ETS.Domain.Common;

namespace ETS.Infrastructure.Authentication
{
    internal sealed class JwtBearerOptionsSetup : IConfigureNamedOptions<JwtBearerOptions>
    {
        private readonly AuthenticationOptions _authenticationOptions;

        public JwtBearerOptionsSetup(IOptions<AuthenticationOptions> authenticationOptions)
        {
            _authenticationOptions = authenticationOptions.Value;
        }

        public void Configure(JwtBearerOptions options)
        {
            options.Audience = _authenticationOptions.Audience;
            options.MetadataAddress = _authenticationOptions.MetadataUrl;
            options.RequireHttpsMetadata = _authenticationOptions.RequireHttpsMetadata;
            options.SaveToken = true;

            var key = CreateRsaSecurityKey(_authenticationOptions.IssuerKey);

            options.TokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = key,
                SaveSigninToken = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                RequireExpirationTime = true,
                ValidIssuer = _authenticationOptions.Issuer,
                ValidAudience = _authenticationOptions.Audience,
                ClockSkew = TimeSpan.FromMinutes(1)
            };

            options.Events = CreateJwtBearerEvents();
        }

        public void Configure(string? name, JwtBearerOptions options)
        {
            Configure(options);
        }  
        

        private static RsaSecurityKey CreateRsaSecurityKey(string issuerKey)
        {
            var rsa = RSA.Create();

            if(issuerKey.TrimStart().StartsWith("-----BEGIN", StringComparison.OrdinalIgnoreCase))
            {
                // PEM format - replace literal \n escapes from JSON config with actual newlines
                var pemKey = issuerKey.Replace("\\n", "\n");
                rsa.ImportFromPem(pemKey);
            }
            else if(issuerKey.TrimStart().StartsWith("<", StringComparison.OrdinalIgnoreCase))
            {
                // XML format (legacy keys)
                rsa.FromXmlString(issuerKey);
            }
            else
            {
                throw new InvalidOperationException(
                    "Unsupported RSA key format. The IssuerKey must be in PEM (-----BEGIN PUBLIC KEY-----) or XML (<RSAKeyValue>) format.");
            }

            return new RsaSecurityKey(rsa);
        }

        /// <summary>
        /// Creates standard JWT bearer events for logging authentication outcomes
        /// </summary>
        /// <returns></returns>
        private static JwtBearerEvents CreateJwtBearerEvents() => new()
        {
            OnAuthenticationFailed = context =>
            {
                Log.Warning("Token validation failed: {Error}", context.Exception.Message.SanitizeForLogging());
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var userId = context.Principal?.FindFirst("sub")?.Value;
                Log.Information("Token validated for user {UserId}", userId?.SanitizeForLogging());
                return Task.CompletedTask;
            }            
        };


    }
}
