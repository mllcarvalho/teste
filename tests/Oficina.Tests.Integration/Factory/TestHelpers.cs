using System;
using System.IO;
using System.Net.Http;
using Oficina.Tests.Integration.Factory;

namespace Oficina.Tests.Integration.Factory
{
    public static class TestHelpers
    {
        public static (CustomWebApplicationFactory Factory, HttpClient Client, string DbFilePath) CreateFactoryWithTempFileDb()
        {
            var dbFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.db");
            Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", $"Data Source={dbFile};");

            var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            return (factory, client, dbFile);
        }
    }
}
