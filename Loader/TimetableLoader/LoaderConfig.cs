using Microsoft.Extensions.Configuration;

namespace TimetableLoader
{
    public interface ILoaderConfig
    {
        string TimetableArchiveFile { get; }
        bool IsRdgZip { get; }
        string ConnectionString { get; }
    }

    internal class LoaderConfig : ILoaderConfig
    {
        private readonly IConfiguration _appSettings;
        private readonly Options _options;

        internal LoaderConfig(IConfiguration appSettings, Options options)
        {
            _appSettings = appSettings;
            _options = options;
        }

        public string TimetableArchiveFile => _options.TimetableArchiveFile;

        public bool IsRdgZip => _options.IsRdgZip;

        public string ConnectionString
        {
            get
            {
                var connString = _appSettings["connection"];
                if (!string.IsNullOrEmpty(_options.Database))
                {
                    var original = "Database=Timetable";
                    var overrideName = $"Database={_options.Database}";
                    connString = connString.Replace(original, overrideName);
                }
                return connString;
            }
        }
    }
}