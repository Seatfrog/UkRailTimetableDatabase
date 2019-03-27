using System.Data.SqlClient;
using CifExtractor;
using CifParser;

namespace TimetableLoader
{
    internal class MasterStationFileLoader : IFileLoader
    {
        private IArchiveFileExtractor _extractor;
        private IParser _parser;
        private IDatabaseLoader _loader;

        public MasterStationFileLoader(IArchiveFileExtractor extractor, IParser parser, IDatabaseLoader loader)
        {
            _extractor = extractor;
            _parser = parser;
            _loader = loader;
        }
        
        public void Run(ILoaderConfig config)
        {
            var reader =_extractor.ExtractFile(config.TimetableArchiveFile, RdgZipExtractor.StationExtension);
            var records = _parser.Read(reader);
            _loader.Load(records);
        }
    }
}