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
                try
                {
                    foreach (var loader in _recordLoaders)
                    {
                        if(loader.Add(record))
                            break;
                    }
                    
                    _logger.Warning("Unknown record {recordType} : {record}", record.GetType(), record);
                }
                catch (Exception e)
                {
                    _logger.Error(e, "Error loading record {recordType} : {record}", record.GetType(), record);
                    throw;    // Initially blowup as want to learn about errors.
                    //TODO Change to continue processing when know more about errors
                }              
            }
        }
    }
}