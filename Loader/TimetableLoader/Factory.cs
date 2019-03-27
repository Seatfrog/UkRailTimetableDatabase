using CifExtractor;
using CifParser;
using Serilog;

namespace TimetableLoader
{
    public interface IFactory
    {
        IExtractor CreateExtractor();
        IParser CreateParser();
        IDatabase CreateDatabase();
        IFileLoader CreateStationLoader(IDatabase db);
    }

    internal class Factory : IFactory
    {
        private readonly ILoaderConfig _config;
        private readonly ILogger _logger;
        private readonly IParserFactory _factory;
        private readonly TtisParserFactory _ttisFactory;

        internal Factory(ILoaderConfig config, ILogger logger)
        {
            _config = config;
            _logger = logger;
            _factory = new ConsolidatorFactory(_logger);
            _ttisFactory = new TtisParserFactory(_logger);
        }

        public IFileLoader CreateCifLoader()
        {
            return new CifLoader(this);
        }
        
        public IExtractor CreateExtractor() =>
            _config.IsRdgZip ? (IExtractor) new RdgZipExtractor(_logger) : new NrodZipExtractor();

        public IParser CreateParser() => _factory.CreateParser();
        
        public IDatabase CreateDatabase() => new Database(_config.ConnectionString, _logger);

        public IFileLoader CreateStationLoader(IDatabase db)
        {
            var extractor = new RdgZipExtractor(_logger);
            var parser = _ttisFactory.CreateStationParser();
            var loader = db.CreateStationLoader();

            return new MasterStationFileLoader(extractor, parser, loader);
        }
    }
}
