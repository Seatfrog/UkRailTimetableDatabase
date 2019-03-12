using System;
using System.IO;
using NSubstitute;
using Serilog;
using TimetableLoader;
using Xunit;

namespace TimetableLoaderTest
{
    public class NrodZipExtractorTest
    {
        public const string cifGzipFile = @".\Data\toc-update-tue.CIF.gz";
               
        [Fact]
        public void CanReadCifFile()
        {
            var extractor = new NrodZipExtractor();

            using (var reader = extractor.ExtractCif(cifGzipFile))
            {
                var first = reader.ReadLine();
                Assert.NotEmpty(first);
            }
        }
    }
    
    public class RdgZipExtractorTest
    {
        public const string rdgZipFile = @".\Data\ttis144.zip";
               
        [Fact]
        public void CanReadCifFile()
        {
            var extractor = new RdgZipExtractor(Substitute.For<ILogger>());

            using (var reader = extractor.ExtractCif(rdgZipFile))
            {
                var first = reader.ReadLine();
                Assert.NotEmpty(first);
            }
        }
       
        [Fact]
        public void CanReadStationFile()
        {
            var extractor = new RdgZipExtractor(Substitute.For<ILogger>());

            using (var reader = extractor.ExtractFile(rdgZipFile, RdgZipExtractor.StationExtension))
            {
                var first = reader.ReadLine();
                Assert.NotNull(first);
            }
        }
    }
}