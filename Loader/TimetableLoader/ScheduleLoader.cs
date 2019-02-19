using CifParser.Records;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CifParser;

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
        private readonly SqlConnection _connection;
        private readonly Sequence _sequence;

        internal DataTable Table { get; private set; }

        public ScheduleLoader(SqlConnection connection, Sequence sequence)
        {
            _connection = connection;
            _sequence = sequence;
        }

        /// <summary>
        /// Create the DataTable to load the records into
        /// </summary>
        public void CreateDataTable()
        {
            var table = new DataTable();

            // read the table structure from the database
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = "SELECT TOP 0 * FROM Schedules";
                using (var adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(table);
                };
            }

            Table =  table;
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
            if (id.Action == RecordAction.Delete)
                AddDelete(id, schedule.GetScheduleDetails());
            else
            {
                Add(id, schedule.GetScheduleDetails(), schedule.GetScheduleExtraDetails()); 
            }
        }

        private void AddDelete(ScheduleId id, ScheduleDetails details)
        {
            var row = Table.NewRow();
            row["Id"] = SetNewId(id);
            row["Action"] = "D";
            row["StpIndicator"] = details.StpIndicator.ToString();
            row["TimetableUid"] = details.TimetableUid;
            row["RunsFrom"] = details.RunsFrom;
            Table.Rows.Add(row);
        }

        private void Add(ScheduleId id, ScheduleDetails details, ScheduleExtraData extra)
        {
            var row = Table.NewRow();
            row["Id"] = SetNewId(id);
            row["Action"] = details.Action == RecordAction.Create ? "C" : "U";
            row["StpIndicator"] = details.StpIndicator.ToString();
            row["TimetableUid"] = details.TimetableUid;
            row["RunsFrom"] = details.RunsFrom;
            row["RunsTo"] = details.RunsTo;
            row["DayMask"] = DayMaskConverter.Convert(details.DayMask);           
            row["BankHolidayRunning"] = details.BankHolidayRunning;
            row["Status"] = details.Status;
            row["Category"] = details.Category;
            row["TrainIdentity"] = details.TrainIdentity;
            row["NrsHeadCode"] = SetNullIfEmpty(details.HeadCode);
            row["ServiceCode"] = details.ServiceCode;
            row["PortionId"] = SetNullIfEmpty(details.PortionId);                
            row["PowerType"] = SetNullIfEmpty(details.PowerType);                
            row["TimingLoadType"] = SetNullIfEmpty(details.TimingLoadType);                
            row["Speed"] = SetNullIfEmpty(details.Speed);
            row["OperatingCharacteristics"] = SetNullIfEmpty(details.OperatingCharacteristics);                
            row["SeatClass"] = ConvertAccommodationClass(details.SeatClass);                
            row["SleeperClass"] = ConvertAccommodationClass(details.SleeperClass);                
            row["ReservationIndicator"] = details.ReservationIndicator == ReservationIndicator.None ? "" : details.ReservationIndicator.ToString();                
            row["Catering"] = SetNullIfEmpty(details.Catering);                
            row["Branding"] = SetNullIfEmpty(details.Branding);

            // Extra data from BX record
            if (extra != null)
            {
                row["EuropeanUic"] = SetNullIfEmpty(extra.UIC);                
                row["Toc"] = SetNullIfEmpty(extra.Toc);                
                row["ApplicableTimetable"] = extra.ApplicableTimetableCode == "Y" ? 1 : 0;                
                row["RetailServiceId"] = SetNullIfEmpty(extra.RetailServiceId);                 
            }

            Table.Rows.Add(row);

        }

        private static object ConvertAccommodationClass(ServiceClass accommodation)
        {
            return accommodation == ServiceClass.None ? (object) DBNull.Value : accommodation.ToString();
        }

        private int SetNewId(ScheduleId id)
        {
            var newId = _sequence.GetNext();
            return newId;
        }  
            
        private object SetNullIfEmpty(string value)
        {
            return string.IsNullOrEmpty(value) ? (object) DBNull.Value : value;
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
                    bulk.DestinationTableName = "Schedules";
                    bulk.WriteToServer(Table);
                }
                catch (Exception ex)
                {
                    Serilog.Log.Error(ex, "Error loading schedules");
                    throw;
                }
            }
        }
    }
}
