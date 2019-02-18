using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using CifParser;
using Serilog;

namespace TimetableLoader
{
    interface IFactory
    {
        IEnumerable<IRecordLoader> CreateLoaders();
        CifLoader Create();

    }

    internal class Factory : IFactory
    {
        private readonly SqlConnection _connection;
 
        internal Factory(SqlConnection connection)
        {
            _connection = connection;
        }

        public IEnumerable<IRecordLoader> CreateLoaders()
        {
            var locationLoader = new LocationLoader(_connection, new Sequence());
            locationLoader.CreateDataTable();
            
            return new IRecordLoader[]
            {
                locationLoader
            };
        }

        public CifLoader Create()
        {
            return new CifLoader(
                new ZipExtractor(), 
                new ScheduleConsolidator(new Parser()),
                new BulkLoader(CreateLoaders(), Log.Logger));
        }
    }
}
