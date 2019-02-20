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
        

        public ScheduleLoader(ScheduleHeaderLoader scheduleRecordsLoader, ILogger logger)
        {
            _logger = logger;            
            _schedules = scheduleRecordsLoader;
        }

        /// <summary>
        /// Create the DataTable to load the records into
        /// </summary>
        public void CreateDataTable()
        {
            _schedules.CreateDataTable();
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
            
            _schedules.Add(id, schedule.GetScheduleDetails(), extraData);

            var skip = (extraData == null) ? 1 : 2;
            Add(id, schedule.Records.Skip(skip));
        }
        private void Add(ScheduleId id, IEnumerable<ICifRecord> records)
        {
            
        }
        
        /// <summary>
        /// Load the DataTable into the database
        /// </summary>
        /// <param name="transaction"></param>
        public void Load(SqlTransaction transaction)
        {
            _schedules.Load(transaction);
        }
    }
}