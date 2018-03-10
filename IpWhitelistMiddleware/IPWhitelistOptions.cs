using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;

public class IPWhitelistOptions : AuthenticationSchemeOptions
{
    public List<String> WhitelistedSubnets { get; set; }
}
