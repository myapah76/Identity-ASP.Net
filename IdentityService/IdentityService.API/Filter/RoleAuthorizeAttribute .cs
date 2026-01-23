using IdentityService.Application.Abstrations;
using IdentityService.Application.AppExceptions;
using IdentityService.Application.Constants;
using IdentityService.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace IdentityService.API.Filter
{
    public class RoleAuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly string[] _roles;

        public RoleAuthorizeAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var _cache = context.HttpContext.RequestServices.GetService<IMemoryCache>();
            var user = context.HttpContext.User;
            //check user login or not?
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                throw new UnauthorizedAccessException(Message.UserMessage.InvalidAccessToken);
            }
            //take userId
            var userId = user.FindFirstValue(JwtRegisteredClaimNames.Sid)!.ToString();
            if (userId == null)
            {
                throw new UnauthorizedAccessException(Message.UserMessage.InvalidAccessToken);
            }
            var userService = context.HttpContext.RequestServices.GetService<IUserService>();
            if (userService == null)
            {
                throw new Exception();
            }
            var roleList = _cache!.Get<List<Role>>(Common.SystemCache.AllRoles);
            var userInDB = await userService.GetByIdAsync(Guid.Parse(userId))
                ?? throw new ForbidenException(Message.UserMessage.NotFound);
            var userRole = roleList!.FirstOrDefault(r => r.Id == userInDB.Role!.Id)!.Name;

            if (userRole == null || !_roles.Contains(userRole))
            {
                throw new ForbidenException(Message.UserMessage.DoNotHavePermission);
            }
        }
    }
}
