using CifParser.Records;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using CifParser;
using Serilog;

namespace TimetableLoader
{
    internal interface ILoader
    {
        void Load(IEnumerable<ICifRecord> records);
    }

    internal class BulkLoader : ILoader
    {
        private readonly ILogger _logger;
        private readonly IEnumerable<IRecordLoader> _recordLoaders;
        private readonly Sequence _sequence;

        public BulkLoader(IEnumerable<IRecordLoader> recordLoaders, Sequence sequence, ILogger logger)
        {
            _recordLoaders = recordLoaders;
            _sequence = sequence;
            _logger = logger;
        }

        public void Load(IEnumerable<ICifRecord> records)
        {            
            foreach (var record in records)
            {
                try
                {
                    var handled = false;
                    
                    foreach (var loader in _recordLoaders)
                    {
                        if (loader.Add(record))
                        {
                            handled = true;
                            break;
                        }
                     }

                    if (!handled)
                    {
                        _sequence.GetNext();    // Bump the sequence to keep ids and lines in sync
                        _logger.Warning("Unknown record {recordType} : {record}", record.GetType(), record);
                    }
                }
                catch (Exception e)
                {
                    _logger.Error(e, "Error loading record {recordType} : {record}", record.GetType(), record);
                    throw;    // Initially blowup as want to learn about errors.
                    //TODO Change to continue processing when know more about errors
                }              
            }

            foreach (var loader in _recordLoaders)
            {
                loader.Load(null);
            }
        }
    }
}