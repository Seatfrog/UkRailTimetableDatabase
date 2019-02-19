using CifParser;
using CifParser.Records;
using System;
using System.IO;
using System.Linq;
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
            new Factory(Substitute.For<IConfiguration>(), Substitute.For<ILogger>());
        
        public static ICifRecord[] ParseRecords(string data)
        {
            var input = new StringReader(data);

            var parser = _factory.CreateParser();
            var records = parser.Read(input).ToArray();
            return records;
        }
    }
}
