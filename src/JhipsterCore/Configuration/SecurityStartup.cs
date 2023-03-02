using System;
using JhipsterCore.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace JhipsterCore.Configuration;

public static class SecurityStartup
{

    public static IServiceCollection AddSecurityModule(this IServiceCollection services)
    {
        //TODO Retrieve the signing key properly (DRY with TokenProvider)
        var opt = services.BuildServiceProvider().GetRequiredService<IOptions<SecuritySettings>>();
        var securitySettings = opt.Value;
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
        })
        .AddCookie()
        .AddOpenIdConnect(options =>
        {
            options.Authority = securitySettings.Authentication.OAuth2.Provider.IssuerUri;
            options.ClientId = securitySettings.Authentication.OAuth2.Provider.ClientId;
            options.ClientSecret = securitySettings.Authentication.OAuth2.Provider.ClientSecret;
            options.SaveTokens = true;
            options.ResponseType = OpenIdConnectResponseType.Code;
            options.RequireHttpsMetadata = false; // dev only
            options.GetClaimsFromUserInfoEndpoint = true;
            options.Scope.Add("openid");
            options.Scope.Add("profile");
            options.Scope.Add("email");
            options.CallbackPath = new PathString("/login/oauth2/code/oidc");
            options.ClaimActions.MapJsonKey("role", "roles", "role");
            options.ClaimActions.MapJsonKey("role", "groups", "role");
        });

        services.AddAuthorization();
        return services;
    }

    public static IApplicationBuilder UseApplicationSecurity(this IApplicationBuilder app,
        SecuritySettings securitySettings)
    {
        app.UseCors(CorsPolicyBuilder(securitySettings.Cors));
        app.UseAuthentication();
        app.UseAuthorization();
        if (securitySettings.EnforceHttps)
        {
            app.UseHsts();
            app.UseHttpsRedirection();
        }
        return app;
    }

    private static Action<CorsPolicyBuilder> CorsPolicyBuilder(Cors config)
    {
        //TODO implement an url based cors policy rather than global or per controller
        return builder =>
        {
            if (!config.AllowedOrigins.Equals("*"))
            {
                if (config.AllowCredentials)
                {
                    builder.AllowCredentials();
                }
                else
                {
                    builder.DisallowCredentials();
                }
            }

            builder.WithOrigins(config.AllowedOrigins)
                .WithMethods(config.AllowedMethods)
                .WithHeaders(config.AllowedHeaders)
                .WithExposedHeaders(config.ExposedHeaders)
                .SetPreflightMaxAge(TimeSpan.FromSeconds(config.MaxAge));
        };
    }
}
