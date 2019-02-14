using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace TimetableLoader
{
    interface ILoaderFactory
    {
        IEnumerable<ILoader> CreateLoaders();
    }

    internal class LoaderFactory : ILoaderFactory
    {
        private SqlConnection _connection;
 
        LoaderFactory(SqlConnection connection)
        {
            _connection = connection;
        }

        public IEnumerable<ILoader> CreateLoaders()
        {
            return new ILoader[]
            {
                new LocationLoader(_connection, new Sequence())
            };
        }
    }
}
