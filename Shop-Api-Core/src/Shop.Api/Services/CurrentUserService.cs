using System.Security.Claims;
using Shop.Application.Common.Interfaces;

namespace Shop.Api.Services;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string CognitoSub
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? user?.FindFirstValue("sub")
                ?? string.Empty;
        }
    }
}
