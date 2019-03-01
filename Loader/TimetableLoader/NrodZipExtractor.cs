using System.IO;
using System.IO.Compression;

namespace TimetableLoader
{
    internal class NrodZipExtractor : IExtractor
    {
        public TextReader ExtractCif(string file)
        {           
            var fileStream = File.OpenRead(file);
            var decompressionStream = new GZipStream(fileStream, CompressionMode.Decompress);
            return new StreamReader(decompressionStream);
        }
    }
}