using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography.X509Certificates;

namespace TimetableLoader
{
    public interface IExtractor
    {
        TextReader ExtractCif(string file);
        TextReader ExtractRdg(string file, string extension = ZipExtractor.RdgCifExtension);
    }

    internal class ZipExtractor : IExtractor
    {
        public const string RdgCifExtension = ".MCA";

        public TextReader ExtractCif(string file)
        {
            var fileStream = File.OpenRead(file);
            var decompressionStream = new GZipStream(fileStream, CompressionMode.Decompress);
            return new StreamReader(decompressionStream);
        }

        /// <summary>
        /// Extract from an RDG Timetable extract
        /// </summary>
        /// <param name="file">RDG timtable zip archive - ttisnnn.zip </param>
        /// <param name="extension">The file inside the archive to extract</param>
        /// <returns>A reader to read the file</returns>
        public TextReader ExtractRdg(string file, string extension = RdgCifExtension)
        {
            var archive = ZipFile.OpenRead(file);

            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                if (entry.FullName.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
                {
                    var s = entry.Open();
                    return new StreamReader(s);
                }
            }

            return null;
        }
    }
}