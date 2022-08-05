using System;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

namespace MEI.Web.Authorization
{
    public class Demo_MinimumAgeRequirement
        : IAuthorizationRequirement
    {
        public int MinimumAge { get; }

        public Demo_MinimumAgeRequirement(int minimumAge)
        {
            MinimumAge = minimumAge;
        }
    }

    public class Demo_MinimumAgeRequirementHandler
        : AuthorizationHandler<Demo_MinimumAgeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, Demo_MinimumAgeRequirement requirement)
        {
            if (!context.User.HasClaim(c => c.Type == ClaimTypes.DateOfBirth && c.Issuer == "http://contoso.com"))
            {
                return Task.CompletedTask;
            }

            var dateOfBirth = Convert.ToDateTime(
                context.User.FindFirst(c => c.Type == ClaimTypes.DateOfBirth && c.Issuer == "http://contoso.com").Value);

            int calculatedAge = DateTime.Today.Year - dateOfBirth.Year;

            if (dateOfBirth > DateTime.Today.AddYears(-calculatedAge))
            {
                calculatedAge--;
            }

            if (calculatedAge >= requirement.MinimumAge)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
