using Microsoft.Extensions.Options;

public class IPWhitelistPostConfigureOptions : IPostConfigureOptions<IPWhitelistOptions>
{
    public void PostConfigure(string name, IPWhitelistOptions options)
    {
    }
}