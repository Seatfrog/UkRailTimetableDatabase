using System.Data.SqlClient;
using CifParser;

namespace TimetableLoader
{
    public interface IFileLoader
    {
        void Run(ILoaderConfig config);
    }

    internal class CifLoader : IFileLoader
    {
        private IFactory _factory;

        internal CifLoader(IFactory factory)
        {
            _factory = factory;
        }

        public void Run(ILoaderConfig config)
        {
            using (var db = _factory.CreateDatabase())
            {
                db.OpenConnection();
                LoadCif(config.TimetableArchiveFile, db);
                if (config.IsRdgZip)
                {
                    var stationLoader = _factory.CreateStationLoader(db);
                    stationLoader.Run(config);
                }
            }
        }

        private void LoadCif(string file, IDatabase db)
        {
            var reader = _factory.CreateExtractor().ExtractCif(file);
            var records = _factory.CreateParser().Read(reader);
            var loader = db.CreateCifLoader();
            loader.Load(records);
        }
    }
}