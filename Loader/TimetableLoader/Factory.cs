using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using CifParser;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace TimetableLoader
{
    interface IFactory
    {
        CifLoader Create();
        IExtractor CreateExtractor();
        IParser CreateParser();
        ILoader CreateLoader(SqlConnection connection);
        SqlConnection CreateConnection();
    }

    internal class Factory : IFactory
    {
        private readonly IConfiguration _config;
        private readonly ILogger _logger;
        
        public string ConnectionString => _config["connection"];    
        
        internal Factory(IConfiguration config, ILogger logger)
        {
            _config = config;
            _logger = logger;
        }    
        
        public IExtractor CreateExtractor() => new ZipExtractor();
        public IParser CreateParser() => new ScheduleConsolidator(new Parser(_logger), _logger);
        public SqlConnection CreateConnection() => new SqlConnection(ConnectionString);

        public ILoader CreateLoader(SqlConnection connection)
        {
            //HACK Use a single sequence as then the Ids match the line number so making it easier to check the original file
            var sequence = new Sequence();

            return new BulkLoader(
                CreateLoaders(connection, sequence),
                sequence,
                Log.Logger);
        }

        private IEnumerable<IRecordLoader> CreateLoaders(SqlConnection connection, Sequence sequence)
        {            
            var locationLoader = new LocationLoader(connection, sequence, _logger);
            var scheduleLoader = new ScheduleLoader(
                new ScheduleHeaderLoader(connection, sequence, _logger), 
                new ScheduleLocationLoader(connection, sequence, locationLoader, _logger), 
                new ScheduleChangeLoader(connection, sequence, _logger), 
                _logger);   
            var associationLoader = new AssociationLoader(connection, sequence, locationLoader, _logger);
            
            return new IRecordLoader[]
            {
                locationLoader,
                scheduleLoader,
                associationLoader
            };
        }

        public CifLoader Create()
        {
            return new CifLoader(this);
        }
    }
}
