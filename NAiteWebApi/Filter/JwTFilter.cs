using Microsoft.AspNetCore.Mvc.Filters;
using NAiteWebApi.Repository.Contracts;
using System.Security.Claims;
using NAiteEntities.Models;
using Microsoft.EntityFrameworkCore;

namespace NAiteWebApi.Filter
{
    public class JwTFilter : IAuthorizationFilter, IAuthFilter
    {
        private NAiteContext _context = null!;

        public JwTFilter(NAiteContext context)
        {
            _context = context;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userId = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId is null)
                return;

            var user = _context.Users.Where(a => a.Id.Equals(userId)).AsNoTracking().FirstOrDefault();
            if (user is null)
                return;

            ApiContext.CurrentUser = user;
        }
    }
}
