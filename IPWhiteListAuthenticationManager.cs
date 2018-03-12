using System.Collections.Generic;
using System.Threading.Tasks;

public class IPWhiteListAuthenticationManager : IIPWhiteListAuthenticationManager
{
    public async Task<IEnumerable<string>> GetWhitelistedSubnets()
    {
        return await Task.FromResult(new[] { "192.168.0.0/16" });
    }
}