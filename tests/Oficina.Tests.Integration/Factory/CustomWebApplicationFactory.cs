using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Oficina.Api;
using Oficina.Tests.Integration.Auth;

namespace Oficina.Tests.Integration.Factory
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            Environment.SetEnvironmentVariable("USERSEED__PASSWORD", "Test@12345!");
            Environment.SetEnvironmentVariable("JWT__KEY", "TestJwtSecretKey12345678901234567890");

            builder.ConfigureAppConfiguration((context, config) =>
            {
                var testConfig = new Dictionary<string, string>
                {
                    ["Seed:DefaultPassword"] = "Test@12345!",
                    ["Seed:Enabled"] = "true",
                    ["ConnectionStrings:DefaultConnection"] = "Data Source=:memory:;Mode=Memory;Cache=Shared"
                };

                config.AddInMemoryCollection(testConfig);
            });

            builder.ConfigureTestServices(services =>
            {
                // Configurar autenticação de teste
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
            });
        }
    }
}