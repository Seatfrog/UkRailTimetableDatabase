using CifParser.Records;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.NetworkInformation;
using CifParser;
using Serilog;

namespace TimetableLoader
{
    /// <summary>
    /// Bulk load Schedules
    /// </summary>
    /// <remarks>
    /// Usage:
    /// <list type="number">
    /// <item>
    /// <description>Construct the object.</description>
    /// </item>
    /// <item>
    /// <description>CreateDataTable<see cref="CreateDataTable" /></description>
    /// </item>
    /// <item>
    /// <description>Add<see cref="Add(ICifRecord)" /> the records</description>
    /// </item>
    /// <item>
    /// <description>Load<see cref="Load"/> to upload the records to the database</description>
    /// </item>
    /// </list>
    /// </remarks>
    internal class ScheduleLoader : IRecordLoader
    {
        private readonly ILogger _logger;

        private readonly ScheduleHeaderLoader _schedules;
        private readonly ScheduleLocationLoader _locations;
        private ScheduleChangeLoader _changes;


        public ScheduleLoader(ScheduleHeaderLoader scheduleRecordsLoader, ScheduleLocationLoader locationLoader, ScheduleChangeLoader changeLoader, ILogger logger)
        {
            _logger = logger;            
            _schedules = scheduleRecordsLoader;
            _locations = locationLoader;
            _changes = changeLoader;
        }

        /// <summary>
        /// Create the DataTable to load the records into
        /// </summary>
        public void Initialise()
        {
            _schedules.Initialise();
            _locations.Initialise();
            _changes.Initialise();
        }

        /// <summary>
        /// Add a record to the DataTable
        /// </summary>
        /// <param name="record"></param>
        /// <returns>Database Id</returns>
        public bool Add(ICifRecord record)
        {
            switch (record)
            {
                case Schedule schedule:
                    Add(schedule);
                    return true;
                default:
                    return false;
            }
        }

        private void Add(Schedule schedule)
        {
            var id = schedule.GetId();
            var extraData = schedule.GetScheduleExtraDetails();
            
            var databaseId = _schedules.Add(id, schedule.GetScheduleDetails(), extraData);

            var skip = (extraData == null) ? 1 : 2;
            Add(databaseId, schedule.Records.Skip(skip));
        }
        
        private void Add(long scheduleId, IEnumerable<ICifRecord> records)
        {
            ScheduleChange previous = null;
            
            foreach (var record in records)
            {
                long locationId;
                
                switch (record)
                {
                    case IntermediateLocation location:
                        locationId = _locations.Add(scheduleId, location);
                        if (previous != null)    // Assumes the next location record is the one linked to the change record.
                        {
                            _changes.Add(scheduleId, locationId, previous);
                            previous = null;
                        }
                        break;
                    case OriginLocation origin:
                        locationId = _locations.Add(scheduleId, origin);
                        break;
                    case TerminalLocation terminal:
                        locationId = _locations.Add(scheduleId, terminal);
                        if (previous != null)
                        {
                            _changes.Add(scheduleId, locationId, previous);
                            previous = null;
                        }
                        break;
                    case ScheduleChange change:
                        if(previous != null)
                            _logger.Warning("Unhandled change record {changeRecord} - schedule {id}", previous, scheduleId);
                        previous = change;
                        break;
                    default:
                        _logger.Warning("Unhandled record {recordType}: {record} - schedule {id}", record.GetType(), record, scheduleId);
                        break;
                }
            }
            
            if(previous != null)
                _logger.Warning("Unhandled change record {changeRecord} - schedule {id}", previous, scheduleId);
        }
        
        /// <summary>
        /// Load the DataTable into the database
        /// </summary>
        /// <param name="transaction"></param>
        public void Load(SqlTransaction transaction)
        {
            _schedules.Load(transaction);
            _locations.Load(transaction);
            _changes.Load(transaction);
        }
    }
}