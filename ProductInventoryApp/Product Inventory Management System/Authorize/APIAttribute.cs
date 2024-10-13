using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Product_Inventory_Management_System.Authorize
{
    public class APIAttribute : ActionFilterAttribute
    {
        private readonly string[] _roles;

        public APIAttribute(params string[] roles)
        {
            _roles = roles; 
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            var token = request.Cookies["AuthToken"];

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    // Get configuration from the request services (dependency injection)
                    var configuration = filterContext.HttpContext.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration;
                    JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

                    var tokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = true,
                        ValidateIssuer = false,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),  // Use the configuration value
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(120),
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"]
                    };

                    var tokenHandler = new JwtSecurityTokenHandler();
                    SecurityToken securityToken;
                    var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

                    var roleClaim = principal.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;

                    //// Check if the user has the required roles
                    var userRoles2 = principal.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();

                    var userRoles1 = principal.FindAll("role").Select(r => r.Value).ToList();
                    var userRoles = principal.FindAll("http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Select(r => r.Value).ToList();
                    var claims = principal.Claims.Select(c => new { c.Type, c.Value }).ToList();
                    //var userRoles = User.FindAll("http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Select(r => r.Value).ToList();
                    //var userRolesJti = securityToken.Claims.First(claim => claim.Type == "Jti").Value;

                    var userRoles3 = principal.FindFirst(ClaimTypes.Role);

                    var validatedJWTToken = (JwtSecurityToken)securityToken;
                    //var jku = jwtToken.Claims.First(claim => claim.Type == "jku").Value;
                    //var userName = validatedJWTToken.Claims.First(claim => claim.Type == "Jti").Value;

                    //if (!_roles.Any(role => userRoles.Contains(role)))
                    //{
                    //    // User does not have the required role
                    //    filterContext.Result = new ForbidResult(); // Return 403 Forbidden
                    //    return;
                    //}
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    filterContext.Result = new UnauthorizedResult(); // Set 401 Unauthorized response
                }
            }
            else
            {
                filterContext.Result = new UnauthorizedResult(); // Set 401 Unauthorized response if token is missing
            }
        }
    }
}
