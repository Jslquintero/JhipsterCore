namespace JhipsterCore.Infrastructure.Configuration;

public class SecuritySettings
{
    public Authentication Authentication { get; set; }
    public Cors Cors { get; set; }
    public bool EnforceHttps { get; set; }
}

public class Authentication
{
    public OAuth2 OAuth2 { get; set; }
}

public class OAuth2
{
    public Provider Provider { get; set; }
}

public class Provider
{
    public string IssuerUri { get; set; }
    public string LogOutUri { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
}

public class Cors
{
    public string AllowedOrigins { get; set; }
    public string AllowedMethods { get; set; }
    public string AllowedHeaders { get; set; }
    public string ExposedHeaders { get; set; }
    public bool AllowCredentials { get; set; }
    public int MaxAge { get; set; }
}
