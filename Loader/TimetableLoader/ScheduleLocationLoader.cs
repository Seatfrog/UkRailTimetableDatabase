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
    /// Bulk load Schedule Locations LO, LI, LT
    /// </summary>
    internal class ScheduleLocationLoader : RecordLoaderBase
    {
        private readonly IDatabaseIdLookup _lookup;

        protected override string TableName => "ScheduleLocations";
        internal ScheduleLocationLoader(SqlConnection connection, Sequence sequence, IDatabaseIdLookup lookup, ILogger logger) :
            base(connection, sequence, logger)
        {
            _lookup = lookup;
        }

        internal long Add(long scheduleId, IntermediateLocation location)
        {
            var databaseId = GetNewId();
            var row = Table.NewRow();
            row["Id"] = databaseId;
            row["ScheduleId"] = scheduleId;
            row["LocationId"] = _lookup.Find(location.Location);
            row["Sequence"] = location.Sequence;
            row["WorkingArrival"] = (object) location.WorkingArrival ?? DBNull.Value;
            row["WorkingDeparture"] = (object) location.WorkingDeparture ?? DBNull.Value;
            row["WorkingPass"] = (object) location.WorkingPass ?? DBNull.Value;
            row["PublicArrival"] = (object) location.PublicArrival ?? DBNull.Value;
            row["PublicDeparture"] = (object) location.PublicDeparture ?? DBNull.Value;
            row["Platform"] = location.Platform;
            row["Line"] = location.Line;
            row["Path"] = location.Path;
            row["Activities"] = location.Activities;
            row["EngineeringAllowance"] = location.EngineeringAllowance;
            row["PathingAllowance"] = location.PathingAllowance;
            row["PerformanceAllowance"] = location.PerformanceAllowance;
            Table.Rows.Add(row);
            return databaseId;
        }

        internal long Add(long scheduleId, OriginLocation location)
        {
            var databaseId = GetNewId();
            var row = Table.NewRow();
            row["Id"] = databaseId;
            row["ScheduleId"] = scheduleId;
            row["LocationId"] = _lookup.Find(location.Location);
            row["Sequence"] = location.Sequence;
            row["WorkingDeparture"] = (object) location.WorkingDeparture ?? DBNull.Value;
            row["PublicDeparture"] = (object) location.PublicDeparture ?? DBNull.Value;
            row["Platform"] = location.Platform;
            row["Line"] = location.Line;
            row["Activities"] = location.Activities;
            row["EngineeringAllowance"] = location.EngineeringAllowance;
            row["PathingAllowance"] = location.PathingAllowance;
            row["PerformanceAllowance"] = location.PerformanceAllowance;
            Table.Rows.Add(row);
            return databaseId;
        }

        internal long Add(long scheduleId, TerminalLocation location)
        {
            var databaseId = GetNewId();
            var row = Table.NewRow();
            row["Id"] = databaseId;
            row["ScheduleId"] = scheduleId;
            row["LocationId"] = _lookup.Find(location.Location);
            row["Sequence"] = location.Sequence;
            row["WorkingArrival"] = (object) location.WorkingArrival ?? DBNull.Value;
            row["PublicArrival"] = (object) location.PublicArrival ?? DBNull.Value;
            row["Platform"] = location.Platform;
            row["Path"] = location.Path;
            row["Activities"] = location.Activities;
            Table.Rows.Add(row);
            return databaseId;
        }
    }
}