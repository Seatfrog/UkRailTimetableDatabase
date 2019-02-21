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
    /// Bulk load Schedule CR records
    /// </summary>
    internal class ScheduleChangeLoader
    {
        private readonly SqlConnection _connection;
        private readonly Sequence _sequence;
        private readonly ILogger _logger;

        internal DataTable Table { get; private set; }

        internal ScheduleChangeLoader(SqlConnection connection, Sequence sequence, ILogger logger)
        {
            _connection = connection;
            _sequence = sequence;
            _logger = logger;
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
                command.CommandText = "SELECT TOP 0 * FROM ScheduleChanges";
                using (var adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(table);
                }

                ;
            }

            Table = table;
        }

        internal long Add(long scheduleId, long scheduleLocationId, ScheduleChange change)
        {
            var databaseId = SetNewId();

            var row = Table.NewRow();
            row["Id"] = databaseId;
            row["ScheduleId"] = scheduleId;
            row["ScheduleLocationId"] = scheduleLocationId;
            row["Category"] = SetNullIfEmpty(change.Category);
            row["TrainIdentity"] = SetNullIfEmpty(change.TrainIdentity);
            row["NrsHeadCode"] = SetNullIfEmpty(change.HeadCode);
            row["ServiceCode"] = SetNullIfEmpty(change.ServiceCode);
            row["PortionId"] = SetNullIfEmpty(change.PortionId);
            row["PowerType"] = SetNullIfEmpty(change.PowerType);
            row["TimingLoadType"] = SetNullIfEmpty(change.TimingLoadType);
            row["Speed"] = SetNullIfEmpty(change.Speed);
            row["OperatingCharacteristics"] = SetNullIfEmpty(change.OperatingCharacteristics);
            row["SeatClass"] = ConvertAccommodationClass(change.SeatClass);
            row["SleeperClass"] = ConvertAccommodationClass(change.SleeperClass);
            row["ReservationIndicator"] = ConvertReservationIndicator(change.ReservationIndicator);
            row["Catering"] = SetNullIfEmpty(change.Catering);
            row["Branding"] = SetNullIfEmpty(change.Branding);
            row["EuropeanUic"] = SetNullIfEmpty(change.Uic);
            row["RetailServiceId"] = SetNullIfEmpty(change.RetailServiceId);

            Table.Rows.Add(row);
            return databaseId;
        }

        private object MapAction(RecordAction action)
        {
            switch (action)
            {
                case RecordAction.Create:
                    return "I";
                case RecordAction.Delete:
                    return "D";
                case RecordAction.Update:
                    return "U";
                default:
                    _logger.Error("Unknown record action {action}", action);
                    return DBNull.Value;
            }
        }

        private object ConvertAccommodationClass(ServiceClass accommodation, bool isCancelOrDelete = false)
        {
            if (accommodation == ServiceClass.None)
                return (object) DBNull.Value;

            // If cancel or delete then need to override default value of B
            return isCancelOrDelete && accommodation == ServiceClass.B
                ? (object) DBNull.Value
                : accommodation.ToString();
        }

        private object ConvertReservationIndicator(ReservationIndicator indicator, bool isCancelOrDelete = false)
        {
            if (indicator == ReservationIndicator.None)
                return isCancelOrDelete ? (object) DBNull.Value : "";

            return indicator.ToString();
        }

        private long SetNewId()
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
                    bulk.DestinationTableName = "ScheduleChanges";
                    bulk.WriteToServer(Table);
                }
                catch (Exception ex)
                {
                    Serilog.Log.Error(ex, "Error loading schedule changes");
                    throw;
                }
            }
        }
    }
}