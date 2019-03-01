using System.IO;

namespace TimetableLoader
{
    public interface IExtractor
    {
        TextReader ExtractCif(string file);
    }
}