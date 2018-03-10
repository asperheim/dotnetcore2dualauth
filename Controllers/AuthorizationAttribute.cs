using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace dotnetcoreoauth2test.Controllers
{
    public class AuthorizationAttribute : Attribute, IAuthorizationFilter, IAuthorizationRequirement
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            //context.Result = new Ok();
            
        }
    }
}