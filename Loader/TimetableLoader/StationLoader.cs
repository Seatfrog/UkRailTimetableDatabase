using CifParser.Records;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using CifParser;
using CifParser.RdgRecords;
using Serilog;

namespace TimetableLoader
{   
    /// <summary>
    /// Bulk load Master Station A records
    /// </summary>
    internal class StationLoader : RecordLoaderBase, IRecordLoader
    {
        protected override string TableName => "Stations";
        
        internal StationLoader(SqlConnection connection, Sequence sequence, ILogger logger) :
            base(connection, sequence, logger)
        {
        }

        /// <summary>
        /// Add a record to the DataTable
        /// </summary>
        /// <param name="record"></param>
        /// <returns>Success</returns>
        public bool Add(IRecord record)
        {
            switch (record)
            {
                case Station station:
                    Add(station);
                    return true;
                default:
                    return false;
            }
        }

        private void Add(Station record)
        {
            var databaseId = GetNewId();
            var row = Table.NewRow();
            row["Id"] = databaseId;
            row["Tiploc"] = record.Tiploc;
            row["Description"] = record.Name;
            row["InterchangeStatus"] = record.InterchangeStatus;
            row["ThreeLetterCode"] = record.ThreeLetterCode;
            row["SubsidiaryThreeLetterCode"] = record.SubsidiaryThreeLetterCode;
            row["Eastings"] = record.East;
            row["Northings"] = record.North;
            row["LocationIsEstimated"] = record.PositionIsEstimated;
            row["MinimumChangeTime"] = record.MinimumChangeTime;
            Table.Rows.Add(row);
        }      
    }
}
