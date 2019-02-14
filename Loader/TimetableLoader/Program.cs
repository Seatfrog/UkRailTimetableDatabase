using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.IO;

namespace TimetableLoader
{
    class Program
    {
        static void Main(string[] args)
        {
            ConfigureLogging();

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var connString = config["connection"];
            Console.WriteLine($"connection: {connString}");
            Console.ReadLine();
        }

        private static void ConfigureLogging()
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Debug()
                .Destructure.ByTransforming<CifParser.Records.Tiploc>(
                    r => new { r.Code, r.Action })
                .WriteTo.Console()
                .WriteTo.File(@"TimetableLoader-.log",
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u4}] {Message:lj} {Exception} {Properties}{NewLine}",
                    rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Log.Information("Configured logging");
        }
    }
}
