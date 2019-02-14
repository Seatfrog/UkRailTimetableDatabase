using CifParser.Records;
using System;
using System.Collections.Generic;
using System.Text;
using CifParser;
using Serilog;

namespace TimetableLoader
{
    internal class BulkLoader
    {
        private ILogger _logger;

        private IEnumerable<ILoader> _recordLoaders;

        public BulkLoader(IEnumerable<ILoader> recordLoaders, ILogger logger)
        {
            _recordLoaders = recordLoaders;
            _logger = logger;
        }

        public void Load(IEnumerable<ICifRecord> records)
        {            
            foreach (var record in records)
            {
                foreach (var loader in _recordLoaders)
                {
                    if(loader.Add(record))
                        break;
                }
                
                _logger.Warning("Unknown record {recordType} : {record}", record.GetType(), record);
            }
        }
    }
}