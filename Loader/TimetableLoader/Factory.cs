using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using CifParser;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace TimetableLoader
{
    public interface IFactory
    {
        IFileLoader CreateCifLoader();
        IExtractor CreateExtractor();
        IParser CreateParser();
        IDatabase CreateDatabase();
        IFileLoader CreateStationLoader(IDatabase db);
    }

    internal class Factory : IFactory
    {
        private readonly IConfiguration _config;
        private readonly Options _options;
        private readonly ILogger _logger;
        private readonly IParserFactory _factory;
        private readonly TtisParserFactory _ttisFactory;

        internal Factory(IConfiguration config, Options options, ILogger logger)
        {
            _config = config;
            _options = options;
            _logger = logger;
            _factory = new ConsolidatorFactory(_logger);
            _ttisFactory = new TtisParserFactory(_logger);
        }

        public IFileLoader CreateCifLoader()
        {
            return new CifLoader(this);
        }
        
        public IExtractor CreateExtractor() =>
            _options.IsRdgZip ? (IExtractor) new RdgZipExtractor(_logger) : new NrodZipExtractor();

        public IParser CreateParser() => _factory.CreateParser();
        
        public IDatabase CreateDatabase() => new Database(_config, _logger);

        public IFileLoader CreateStationLoader(IDatabase db)
        {
            var extractor = new RdgZipExtractor(_logger);
            var parser = _ttisFactory.CreateStationParser();
            var loader = db.CreateStationLoader();

            return new MasterStationFileLoader(extractor, parser, loader);
        }
    }
}
