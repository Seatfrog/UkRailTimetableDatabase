using CifParser;
using CifParser.Records;
using System;
using System.IO;
using System.Linq;
using CifParser.RdgRecords;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using NSubstitute.Core;
using Serilog;
using TimetableLoader;

namespace TimetableLoaderTest
{
    internal static class ParserHelper
    {
        private static readonly Factory _factory = 
            new Factory(Substitute.For<IConfiguration>(), new Options(), Substitute.For<ILogger>());

        private static readonly TtisParserFactory _ttisFactory = 
            new TtisParserFactory(Substitute.For<ILogger>());
        
        public static IRecord[] ParseRecords(string data)
        {
            var input = new StringReader(data);

            var parser = _factory.CreateParser();
            var records = parser.Read(input).ToArray();
            return records;
        }
        
        public static Station[] ParseStationRecords(string data)
        {
            var input = new StringReader(data);

            var parser = _ttisFactory.CreateStationParser(0);
            var records = parser.Read(input).Cast<Station>().ToArray();
            return records;
        }
    }
}
