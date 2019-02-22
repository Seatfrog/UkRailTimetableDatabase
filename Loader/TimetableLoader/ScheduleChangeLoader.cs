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
            var databaseId = GetNewId();

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
            row["SeatClass"] = ScheduleHeaderLoader.ConvertAccommodationClass(change.SeatClass);
            row["SleeperClass"] = ScheduleHeaderLoader.ConvertAccommodationClass(change.SleeperClass);
            row["ReservationIndicator"] = ScheduleHeaderLoader.ConvertReservationIndicator(change.ReservationIndicator);
            row["Catering"] = SetNullIfEmpty(change.Catering);
            row["Branding"] = SetNullIfEmpty(change.Branding);
            row["EuropeanUic"] = SetNullIfEmpty(change.Uic);
            row["RetailServiceId"] = SetNullIfEmpty(change.RetailServiceId);

            Table.Rows.Add(row);
            return databaseId;
        }
    }
}