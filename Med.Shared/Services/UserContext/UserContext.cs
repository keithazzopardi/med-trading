using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Med.Shared.Services.UserContext;

public class UserContext : IUserContext
{
    public long UserId { get; }

    public const string UserIdClaim = ClaimTypes.NameIdentifier;

    public UserContext(IHttpContextAccessor context)
    {
        if (context.HttpContext.User.Identity.IsAuthenticated)
        {
            this.UserId = long.Parse(GetClaimValueByType(context.HttpContext.User.Claims, UserIdClaim));
        }
    }


    private static string GetClaimValueByType(IEnumerable<Claim> claims, string claimType)
    {
        if (claims == null)
            throw new ArgumentNullException(nameof(claims));

        if (string.IsNullOrEmpty(claimType))
            throw new ArgumentNullException(nameof(claimType));

        string normalizedClaimType = UriClaimTypeToClaimType(claimType);
        return claims.FirstOrDefault(x => x.Type == normalizedClaimType)?.Value;
    }

    private static string UriClaimTypeToClaimType(string claimType)
    {
        if (Uri.TryCreate(claimType, UriKind.Absolute, out var uri))
            return uri.Segments.LastOrDefault() ?? claimType;
        else
            return claimType;
    }
}

