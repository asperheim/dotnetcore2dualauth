using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http;

public static class JWKSProvider
{
    static readonly HttpClient client = new HttpClient();

    public static IEnumerable<SecurityKey> GetKeySet(string jwksUrl) => new JsonWebKeySet(client.GetStringAsync(jwksUrl).Result).GetSigningKeys();
}
