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
    internal class ScheduleChangeLoader : RecordLoaderBase
    {
        protected override string TableName => "ScheduleChanges";

        internal ScheduleChangeLoader(SqlConnection connection, Sequence sequence, ILogger logger) : 
            base(connection, sequence, logger)
        {
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
    }
}