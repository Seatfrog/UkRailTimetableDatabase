using System.Data.SqlClient;
using CifParser;

namespace TimetableLoader
{
    public interface IFileLoader
    {
        void Run(Options options);
    }

    internal class CifLoader : IFileLoader
    {
        private IFactory _factory;

        internal CifLoader(IFactory factory)
        {
            _factory = factory;
        }

        public void Run(Options options)
        {
            using (var db = _factory.CreateDatabase())
            {
                db.OpenConnection();
                LoadCif(options, db);
                if (options.IsRdgZip)
                {
                    var stationLoader = _factory.CreateStationLoader(db);
                    stationLoader.Run(options);
                }
            }
        }

        private void LoadCif(Options options, IDatabase db)
        {
            var reader = _factory.CreateExtractor().ExtractCif(options.TimetableArchiveFile);
            var records = _factory.CreateParser().Read(reader);
            var loader = db.CreateCifLoader();
            loader.Load(records);
        }
    }
}