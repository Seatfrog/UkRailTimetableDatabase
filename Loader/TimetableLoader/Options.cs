using System.Collections.Generic;
using CommandLine;

namespace TimetableLoader
{
    public class Options
    {
        [Option('i', "input", Required = true, HelpText = "Timetable Cif file")]
        public string TimetableFile { get; set; }
    }
}