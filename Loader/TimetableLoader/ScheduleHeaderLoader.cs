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
    /// Bulk load Schedule BS and BX records
    /// </summary>
    internal class ScheduleHeaderLoader : RecordLoaderBase
    {
        protected override string TableName => "Schedules";

        internal ScheduleHeaderLoader(SqlConnection connection, Sequence sequence, ILogger logger) : base(connection, sequence, logger)
        {
        }

        internal long Add(ScheduleId id, ScheduleDetails details, ScheduleExtraData extra)
        {
            bool isCancelOrDelete = details.StpIndicator == StpIndicator.C ||
                                    details.Action == RecordAction.Delete;
            var databaseId = GetNewId();
            
            var row = Table.NewRow();
            row["Id"] = databaseId;
            row["Action"] = MapAction(details.Action, _logger);
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
                GetNewId();    //Skip an Id to keep it synced with record in file 
                row["EuropeanUic"] = SetNullIfEmpty(extra.UIC);
                row["Toc"] = SetNullIfEmpty(extra.Toc);
                row["ApplicableTimetable"] = extra.ApplicableTimetableCode == "Y" ? 1 : 0;
                row["RetailServiceId"] = SetNullIfEmpty(extra.RetailServiceId);
            }

            Table.Rows.Add(row);
            return databaseId;
        }

        internal static object MapAction(RecordAction action, ILogger logger)
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
                    logger.Error("Unknown record action {action}", action);
                    return DBNull.Value;
            }
        }
        internal static object ConvertAccommodationClass(ServiceClass accommodation, bool isCancelOrDelete = false)
        {
            if (accommodation == ServiceClass.None)
                return (object) DBNull.Value;

            // If cancel or delete then need to override default value of B
            return isCancelOrDelete && accommodation == ServiceClass.B
                ? (object) DBNull.Value
                : accommodation.ToString();
        }

        internal static object ConvertReservationIndicator(ReservationIndicator indicator, bool isCancelOrDelete = false)
        {
            if (indicator == ReservationIndicator.None)
                return isCancelOrDelete ? (object) DBNull.Value : "";

            return indicator.ToString();
        }
     }
}