﻿using CifParser.Records;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
        private readonly SqlConnection _connection;
        private readonly Sequence _sequence;
        private ILogger _logger;

        internal DataTable Table { get; private set; }

        public ScheduleLoader(SqlConnection connection, Sequence sequence, ILogger logger)
        {
            _connection = connection;
            _sequence = sequence;
            _logger = logger;
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
                }

                ;
            }

            Table = table;
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
            Add(id, schedule.GetScheduleDetails(), schedule.GetScheduleExtraDetails());
        }

        private void Add(ScheduleId id, ScheduleDetails details, ScheduleExtraData extra)
        {
            bool isCancelOrDelete = details.StpIndicator == StpIndicator.C ||
                                    details.Action == RecordAction.Delete;
            
            var row = Table.NewRow();
            row["Id"] = SetNewId(id);
            row["Action"] = MapAction(details.Action);
            row["StpIndicator"] = details.StpIndicator.ToString();
            row["TimetableUid"] = details.TimetableUid;
            row["RunsFrom"] = details.RunsFrom;
            row["RunsTo"] = (object) details.RunsTo ?? DBNull.Value;
            row["DayMask"] = DayMaskConverter.Convert(details.DayMask);
            row["BankHolidayRunning"] =
                DayMaskConverter.ConvertBankHoliday(details.BankHolidayRunning, isCancelOrDelete);
            row["Status"] = SetNullIfEmpty(details.Status);
            row["Category"] = SetNullIfEmpty(details.Category);
            row["TrainIdentity"] = SetNullIfEmpty(details.TrainIdentity);
            row["NrsHeadCode"] = SetNullIfEmpty(details.HeadCode);
            row["ServiceCode"] = SetNullIfEmpty(details.ServiceCode);
            row["PortionId"] = SetNullIfEmpty(details.PortionId);
            row["PowerType"] = SetNullIfEmpty(details.PowerType);
            row["TimingLoadType"] = SetNullIfEmpty(details.TimingLoadType);
            row["Speed"] = SetNullIfEmpty(details.Speed);
            row["OperatingCharacteristics"] = SetNullIfEmpty(details.OperatingCharacteristics);
            row["SeatClass"] = ConvertAccommodationClass(details.SeatClass, isCancelOrDelete);
            row["SleeperClass"] = ConvertAccommodationClass(details.SleeperClass, isCancelOrDelete);
            row["ReservationIndicator"] = ConvertReservationIndicator(details.ReservationIndicator, isCancelOrDelete);
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

        private object MapAction(RecordAction action)
        {
            switch (action)
            {
                case RecordAction.Create:
                    return "C";
                case RecordAction.Delete:
                    return "D";
                case RecordAction.Update:
                    return "U";
                default:
                    _logger.Error("Unknown record action {action}", action);
                    return DBNull.Value;
            }
        }

        private object ConvertAccommodationClass(ServiceClass accommodation, bool isCancelOrDelete)
        {
            if (accommodation == ServiceClass.None)
                return (object) DBNull.Value;
            
            // If cancel or delete then need to override default value of B
            return isCancelOrDelete && accommodation == ServiceClass.B ? 
                (object) DBNull.Value : 
                accommodation.ToString();
        }

        private object ConvertReservationIndicator(ReservationIndicator indicator, bool isCancelOrDelete)
        {
            if (indicator == ReservationIndicator.None)
                return isCancelOrDelete ? (object) DBNull.Value : "";
            
            return indicator.ToString();
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