using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;
using System.Text.Encodings.Web;
using System.Net;
using System.Security.Claims;

public class IPWhiteListAuthenticationHandler : AuthenticationHandler<IPWhitelistOptions>
{

    public IPWhiteListAuthenticationHandler(IOptionsMonitor<IPWhitelistOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var bearerResult = Context.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme).Result;

        if (bearerResult != null && bearerResult.Succeeded)
            return AuthenticateResult.Success(bearerResult.Ticket);

        var callerIp = Context.Connection.RemoteIpAddress;

        AuthenticationTicket ticket = await GetClaimsPrincipal(callerIp);

        var inWhitelistedSubnet = Options
            .WhitelistedSubnets
            .Any(x => (IPNetwork
                       .Parse(x)
                       .Contains(callerIp)));

        if (inWhitelistedSubnet) return AuthenticateResult.Success(ticket);

        return AuthenticateResult.Fail(new IPNotInWhitelistException("Schema does not contain caller ip."));
    }

    private Task<AuthenticationTicket> GetClaimsPrincipal(IPAddress callerIp)
    {
        var claims = new[] { new Claim(ClaimTypes.Name, callerIp.ToString()) };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return Task.FromResult(ticket);
    }
}
