using CifParser;
using CifParser.Records;
using System;
using System.IO;
using System.Linq;

namespace TimetableLoaderTest
{
    internal static class ParserHelper
    {
        public static ICifRecord[] ParseRecords(string data)
        {
            var input = new StringReader(data);

            var parser = new Parser();
            var records = parser.Read(input).ToArray();
            return records;
        }
    }
}
