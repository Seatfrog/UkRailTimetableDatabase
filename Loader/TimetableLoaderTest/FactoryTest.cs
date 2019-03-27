using CifExtractor;
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
            var config = Substitute.For<ILoaderConfig>();
            config.IsRdgZip.Returns(false);
            
            var factory = new Factory(config, Substitute.For<ILogger>());
            
            var extractor = factory.CreateExtractor();

            Assert.IsType<NrodZipExtractor>(extractor);
        }
        
        [Fact]
        public void ConstructRdgExtractor()
        {
            var config = Substitute.For<ILoaderConfig>();
            config.IsRdgZip.Returns(true);
            
            var factory = new Factory(config, Substitute.For<ILogger>());
            
            var extractor = factory.CreateExtractor();

            Assert.IsType<RdgZipExtractor>(extractor);
        }
    }
}