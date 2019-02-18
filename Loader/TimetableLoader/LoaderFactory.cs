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
        public string ConnectionString => _config["connection"];    
        
        internal Factory(IConfiguration config)
        {
            _config = config;
        }    
        
        public IExtractor CreateExtractor() => new ZipExtractor();
        public IParser CreateParser() => new ScheduleConsolidator(new Parser());
        public SqlConnection CreateConnection() => new SqlConnection(ConnectionString);

        public ILoader CreateLoader(SqlConnection connection)
        {
            return new BulkLoader(
                CreateLoaders(connection),
                Log.Logger);
        }

        private IEnumerable<IRecordLoader> CreateLoaders(SqlConnection connection)
        {
            var locationLoader = new LocationLoader(connection, new Sequence());
            locationLoader.CreateDataTable();
            
            return new IRecordLoader[]
            {
                locationLoader
            };
        }

        public CifLoader Create()
        {
            return new CifLoader(this);
        }
    }
}
