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
    internal class ScheduleLocationLoader
    {
        private readonly SqlConnection _connection;
        private readonly Sequence _sequence;
        private readonly IDatabaseIdLookup _lookup;
        private readonly ILogger _logger;
        
        internal DataTable Table { get; private set; }
       
        internal ScheduleLocationLoader(SqlConnection connection, Sequence sequence, IDatabaseIdLookup lookup, ILogger logger)
        {
            _connection = connection;
            _sequence = sequence;
            _logger = logger;
            _lookup = lookup;
        }

        /// <summary>
        /// Create the DataTable to load the records into
        /// </summary>
        internal void CreateDataTable()
        {
            var table = new DataTable();

            // read the table structure from the database
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = "SELECT TOP 0 * FROM ScheduleLocations";
                using (var adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(table);
                }

                ;
            }

            Table = table;
        }
                
        internal long Add(long scheduleId, IntermediateLocation location)
        {
            
            var databaseId = SetNewId();
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
       
        private long SetNewId()
        {
            var newId = _sequence.GetNext();
            return newId;
        }

        internal long Add(long scheduleId, OriginLocation location)
        {           
            var databaseId = SetNewId();
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
            var databaseId = SetNewId();
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
        
        /// <summary>
        /// Load the DataTable into the database
        /// </summary>
        /// <param name="transaction"></param>
        public void Load(SqlTransaction transaction)
        {
            using (var bulk = new SqlBulkCopy(_connection, SqlBulkCopyOptions.KeepIdentity, transaction))
            {
                try
                {
                    bulk.DestinationTableName = "ScheduleLocations";
                    bulk.WriteToServer(Table);
                }
                catch (Exception ex)
                {
                    Serilog.Log.Error(ex, "Error loading schedule locations");
                    throw;
                }
            }
        }
    }
}