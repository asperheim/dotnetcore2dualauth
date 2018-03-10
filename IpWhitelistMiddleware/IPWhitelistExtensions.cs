using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;

public static class IPWhitelistExtensions {
    public static AuthenticationBuilder AddIpWhitelistAndBearer(this AuthenticationBuilder builder, Action<IPWhitelistOptions> whiteListOptions, Action<JwtBearerOptions> jwtBearerOptions)
    {
        builder.Services.AddSingleton(ServiceDescriptor.Singleton<IPostConfigureOptions<IPWhitelistOptions>, IPWhitelistPostConfigureOptions>());
        builder.AddJwtBearer(jwtBearerOptions);

        return builder.AddScheme<IPWhitelistOptions, IPWhiteListAuthenticationHandler>(IPWhiteListWithJwtBearerDefaults.AuthenticationScheme, whiteListOptions);
    }
}
