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
        public void ExtractCifFromGZip()
        {
            var extractor = new ZipExtractor();

            using (var reader = extractor.ExtractCif(cifGzipFile))
            {
                Assert.NotNull(reader);
            }
        }
       
        [Fact]
        public void CanReadCifFile()
        {
            var extractor = new ZipExtractor();

            using (var reader = extractor.ExtractCif(cifGzipFile))
            {
                Assert.NotNull(reader.ReadLine());
            }
        }
        
        [Fact]
        public void ExtractFileFromRdgZip()
        {
            var extractor = new ZipExtractor();

            using (var reader = extractor.ExtractRdg(rdgZipFile, ".MSN"))
            {
                Assert.NotNull(reader);
            }
        }
       
        [Fact]
        public void CanReadRdgZipFile()
        {
            var extractor = new ZipExtractor();

            using (var reader = extractor.ExtractRdg(rdgZipFile, ".MSN"))
            {
                Assert.NotNull(reader.ReadLine());
            }
        }
    }
}