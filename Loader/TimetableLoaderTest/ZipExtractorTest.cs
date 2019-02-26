using System;
using System.IO;
using TimetableLoader;
using Xunit;

namespace TimetableLoaderTest
{
    public class ZipExtractorTest
    {
        public const string cifGzipFile = @".\Data\toc-update-tue.CIF.gz";
        public const string rdgZipFile = @".\Data\ttis144.zip";
               
        [Fact]
        public void CanReadCifFile()
        {
            var extractor = new ZipExtractor();

            using (var reader = extractor.ExtractCif(cifGzipFile))
            {
                var first = reader.ReadLine();
                Assert.NotEmpty(first);
            }
        }
       
        [Fact]
        public void CanReadRdgZipFile()
        {
            var extractor = new ZipExtractor();

            using (var reader = extractor.ExtractRdg(rdgZipFile, ".MSN"))
            {
                var first = reader.ReadLine();
                Assert.NotNull(first);
            }
        }
    }
}