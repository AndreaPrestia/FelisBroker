using System.IdentityModel.Tokens.Jwt;
using System.Text;
using FelisBroker.Common.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace FelisBroker.DataSource.Http.Helpers;

public static class HttpAuthHelper
{
    public static bool IsAuthorized(HttpContext ctx, HttpConfiguration config)
    {
        if (config.AuthConfiguration is null || config.AuthConfiguration.AuthScheme == HttpAuthScheme.None) return true;

        var auth = ctx.Request.Headers.Authorization.ToString();
        if (string.IsNullOrWhiteSpace(auth)) return false;

        return config.AuthConfiguration.AuthScheme switch
        {
            HttpAuthScheme.Basic => auth.StartsWith("Basic ") &&
                                    ValidateBasic(auth, config.AuthConfiguration as BasicAuthHttpConfiguration),
            HttpAuthScheme.Jwt => auth.StartsWith("Bearer ") &&
                                  ValidateJwt(auth, config.AuthConfiguration as JwtAuthHttpConfiguration),
            HttpAuthScheme.MTls => ValidateMTls(ctx, config.AuthConfiguration as MTlsAuthHttpConfiguration),
            _ => false
        };
    }

    private static bool ValidateBasic(string authHeader, BasicAuthHttpConfiguration? config)
    {
        if(config is null) return false;
        
        var encoded = authHeader["Basic ".Length..].Trim();
        var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
        var parts = decoded.Split(':');
        var username = parts[0];
        var password = parts[1];
        return string.Equals(config.Username, username, StringComparison.OrdinalIgnoreCase) && string.Equals(config.Password, password, StringComparison.OrdinalIgnoreCase);
    }

    private static bool ValidateJwt(string authHeader, JwtAuthHttpConfiguration? jwtConfig)
    {
        if (jwtConfig is null) return false;
        
        var token = authHeader["Bearer ".Length..].Trim();

        var handler = new JwtSecurityTokenHandler();

        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = !string.IsNullOrWhiteSpace(jwtConfig.Issuer),
            ValidIssuer = jwtConfig.Issuer,
            ValidateAudience = !string.IsNullOrWhiteSpace(jwtConfig.Audience),
            ValidAudience = jwtConfig.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Secret!)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(jwtConfig.DurationInMinutes)
        };

        try
        {
            handler.ValidateToken(token, parameters, out _);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Auth] JWT validation failed: {ex.Message}");
            return false;
        }
    }
    
    private static bool ValidateMTls(HttpContext context, MTlsAuthHttpConfiguration? mTlsConfig)
    {
        if(mTlsConfig is null) return false;
        
        var clientCert = context.Connection.ClientCertificate;

        if (clientCert is null)
            return !mTlsConfig.RequireClientCertificate;

        if (mTlsConfig.AllowedThumbprints is not { Count: > 0 })
            return mTlsConfig.AllowedSubjects is not { Count: > 0 } ||
                   mTlsConfig.AllowedSubjects.Any(s =>
                       clientCert.Subject.Contains(s, StringComparison.OrdinalIgnoreCase));
        
        var thumbprint = clientCert.Thumbprint.Replace(":", "").ToUpperInvariant();
        if (!mTlsConfig.AllowedThumbprints.Any(t => t.Equals(thumbprint, StringComparison.OrdinalIgnoreCase)))
            return false;

        return mTlsConfig.AllowedSubjects is not { Count: > 0 } || mTlsConfig.AllowedSubjects.Any(s => clientCert.Subject.Contains(s, StringComparison.OrdinalIgnoreCase));
    }
}