using CifParser.Records;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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


        public ScheduleLoader(ScheduleHeaderLoader scheduleRecordsLoader, ScheduleLocationLoader locationLoader, ILogger logger)
        {
            _logger = logger;            
            _schedules = scheduleRecordsLoader;
            _locations = locationLoader;
        }

        /// <summary>
        /// Create the DataTable to load the records into
        /// </summary>
        public void CreateDataTable()
        {
            _schedules.CreateDataTable();
            _locations.CreateDataTable();
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
            _locations.Add(databaseId, schedule.Records.Skip(skip));
        }
        
        /// <summary>
        /// Load the DataTable into the database
        /// </summary>
        /// <param name="transaction"></param>
        public void Load(SqlTransaction transaction)
        {
            _schedules.Load(transaction);
            _locations.Load(transaction);
        }
    }
}