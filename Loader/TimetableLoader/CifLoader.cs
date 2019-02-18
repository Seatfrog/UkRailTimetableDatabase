using CifParser;

namespace TimetableLoader
{
    internal class CifLoader
    {
        private IExtractor _extractor;
        private IParser _parser;
        private ILoader _loader;

        internal CifLoader(IExtractor extractor, IParser parser, ILoader loader)
        {
            _extractor = extractor;
            _parser = parser;
            _loader = loader;
        }

        public void Run(Options options)
        {
            var reader = _extractor.ExtractCif(options.TimetableFile);
            var records = _parser.Read(reader);
            _loader.Load(records);
        }
    }
}