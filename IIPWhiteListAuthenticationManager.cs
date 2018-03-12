using System.Collections.Generic;
using System.Threading.Tasks;

public interface IIPWhiteListAuthenticationManager
{
    Task<IEnumerable<string>> GetWhitelistedSubnets();
}