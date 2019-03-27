using Microsoft.Extensions.Configuration;
using NSubstitute;
using TimetableLoader;
using Xunit;

namespace TimetableLoaderTest
{
    public class LoaderConfigTest
    {
        [Theory]
        [InlineData("TimetableRdg", "Server=(local);Database=TimetableRdg;Trusted_Connection=True;")]
        [InlineData("", "Server=(local);Database=Timetable;Trusted_Connection=True;")]
        [InlineData(null, "Server=(local);Database=Timetable;Trusted_Connection=True;")]
        public void OverrideDatabase(string overrideDb, string expectedConnection)
        {
            var appConfig = Substitute.For<IConfiguration>();
            appConfig["connection"].Returns("Server=(local);Database=Timetable;Trusted_Connection=True;");

            var options = new Options()
            {
                Database = overrideDb
            };
            
            var config = new LoaderConfig(appConfig, options);
            Assert.Equal(expectedConnection, config.ConnectionString);
        }      
    }
}