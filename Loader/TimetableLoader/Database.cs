using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace TimetableLoader
{
    public interface IDatabase : IDisposable
    {
        void OpenConnection();
        IDatabaseLoader CreateCifLoader();
        IDatabaseLoader CreateStationLoader();
    }

    internal class Database : IDatabase
    {
        private readonly string _connectionString;
        private readonly ILogger _logger;
        private SqlConnection _connection;      

        internal Database(string connectionString, ILogger logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        public IDatabaseLoader CreateCifLoader()
        {
            var sequence = new Sequence();

            return new BulkLoader(
                CreateLoaders(_connection, sequence),
                sequence,
                _logger);
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

        public IDatabaseLoader CreateStationLoader()
        {
            var sequence = new Sequence();
            var loader = new StationLoader(_connection, sequence, _logger);
            return new BulkLoader(new[] {loader}, sequence, _logger);
        }
        
        public void Dispose()
        {
            if (_connection == null) 
                return;
            
            _connection.Close();
            _connection = null;
        }

        public void OpenConnection()
        {
            _connection = new SqlConnection(_connectionString);
            _connection.Open();
        }
    }
}