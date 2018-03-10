using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Net.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Text.Encodings.Web;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Claims;

namespace dotnetcoreoauth2test
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddMvc();
            services.AddAuthentication(auth => {
                auth.DefaultScheme = IPWhiteListDefaults.AuthenticationScheme;
                //auth.AddScheme<IPWhiteListAuthenticationHandler>("", "");
                auth.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;

            })
                    .AddIpWhitelist(options =>
                                    options
                                    //.WhitelistedSubnets = new[] { "192.168.0.0/16", "127.0.0.0/8" }//, "::1/128" }
                                    .WhitelistedSubnets = new[] { "192.168.0.0/16", "127.0.0.0/8", "::1/128" }
                                    .ToList())
                    .AddJwtBearer(options =>
                                  options.TokenValidationParameters = new TokenValidationParameters
                                  {
                                      ValidateIssuer = true,
                                      ValidateAudience = false,
                                      ValidateLifetime = true,
                                      ValidateIssuerSigningKey = true,
                                      ValidIssuer = "https://sts.windows.net/41c3ed9c-4e9e-4336-a82a-6447bd9136d3/",
                                      IssuerSigningKeys = JWKSProvider
                    .GetKeySet("https://login.microsoftonline.com/common/discovery/keys")
                                  });
                    
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //var options = new JwtBearerOptions
            //{
            //    Audience = "https://contacts.mycompany.com/",
            //    Authority = "https://bk-samples.auth0.com/"
            //};

            app.UseAuthentication();

            app.UseMvc();

        }
    }


}

public static class IPWhiteListDefaults
{
    public const string AuthenticationScheme = "IPWhitelist";
    public const string AuthenticationSchemeWithOAuth2 = "IPWhitelist,Bearer";

}

public static class JWKSProvider
{
    static readonly HttpClient client = new HttpClient();

    public static IEnumerable<SecurityKey> GetKeySet(string jwksUrl) => new JsonWebKeySet(client.GetStringAsync(jwksUrl).Result).GetSigningKeys();
}

public static class IPWhitelistExtensions {
    public static AuthenticationBuilder AddIpWhitelist(this AuthenticationBuilder builder, Action<IPWhitelistOptions> options)
    {
        builder.Services.AddSingleton(ServiceDescriptor.Singleton<IPostConfigureOptions<IPWhitelistOptions>, IPWhitelistPostConfigureOptions>());

        return builder.AddScheme<IPWhitelistOptions, IPWhiteListAuthenticationHandler>(IPWhiteListDefaults.AuthenticationScheme, options);
    }
}



public class IPWhiteListAuthenticationHandler : AuthenticationHandler<IPWhitelistOptions>
{

    public IPWhiteListAuthenticationHandler(IOptionsMonitor<IPWhitelistOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[] { new Claim(ClaimTypes.Name, "Desc from ip table") };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        await Task.Run(() => { });

        var inWhitelistedSubnet = Options
            .WhitelistedSubnets
            .Any(x => (IPNetwork
                       .Parse(x)
                       .Contains(Context.Connection.RemoteIpAddress)));

        if (inWhitelistedSubnet) return AuthenticateResult.Success(ticket);

        //return AuthenticateResult.Fail("IP not in whitelist");
        return AuthenticateResult.NoResult();
    }
}

public class IPNotInWhitelistException : Exception
{
    public IPNotInWhitelistException()
    {
    }

    public IPNotInWhitelistException(string message) : base(message)
    {
    }

    public IPNotInWhitelistException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected IPNotInWhitelistException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}

public class IPWhitelistOptions : AuthenticationSchemeOptions
{
    public List<String> WhitelistedSubnets { get; set; }
}

public class IPWhitelistPostConfigureOptions : IPostConfigureOptions<IPWhitelistOptions>
{
    public void PostConfigure(string name, IPWhitelistOptions options)
    {

    }
}