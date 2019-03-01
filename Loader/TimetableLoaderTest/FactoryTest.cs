using Microsoft.Extensions.Configuration;
using NSubstitute;
using Serilog;
using TimetableLoader;
using Xunit;

namespace TimetableLoaderTest
{
    public class FactoryTest
    {
        [Fact]
        public void ConstructNrodExtractor()
        {
            var options = new Options()
            {
                IsRdgZip = false
            };
            var factory = new Factory(Substitute.For<IConfiguration>(), options, Substitute.For<ILogger>());
            
            var extractor = factory.CreateExtractor();

            Assert.IsType<NrodZipExtractor>(extractor);
        }
        
        [Fact]
        public void ConstructRdgExtractor()
        {
            var options = new Options()
            {
                IsRdgZip = true
            };
            var factory = new Factory(Substitute.For<IConfiguration>(), options, Substitute.For<ILogger>());
            
            var extractor = factory.CreateExtractor();

            Assert.IsType<RdgZipExtractor>(extractor);
        }
    }
}