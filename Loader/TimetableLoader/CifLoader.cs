using System.Data.SqlClient;
using CifParser;

namespace TimetableLoader
{
    internal class CifLoader
    {
        private IFactory _factory;

        internal CifLoader(IFactory factory)
        {
            _factory = factory;
        }

        public void Run(Options options)
        {
            using (var connection = _factory.CreateConnection())
            {
                connection.Open();
                var reader = _factory.CreateExtractor().ExtractCif(options.TimetableArchiveFile);
                var records = _factory.CreateParser().Read(reader);
                var loader = _factory.CreateLoader(connection);
                loader.Load(records);                
            }
        }
    }
}