
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Data.SqlClient;

namespace TimetableLoaderTest
{
    public class IntegrationFixture
    {
        private IConfiguration _config;

        public IntegrationFixture()
        {
            ConfigureLogging();

            _config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
        }

        public SqlConnection CreateConnection() => new SqlConnection(_config["connection"]);

        private static void ConfigureLogging()
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Debug()
                .Destructure.ByTransforming<CifParser.Records.Tiploc>(
                    r => new { r.Code, r.Action })
                .WriteTo.Console()
                .CreateLogger();

            Log.Information("Configured logging");
        }
    }
}