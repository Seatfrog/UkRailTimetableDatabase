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
    /// Bulk load Schedule Locations AA
    /// </summary>
    internal class AssociationLoader : RecordLoaderBase, IRecordLoader
    {
        private readonly IDatabaseIdLookup _lookup;

        protected override string TableName => "Associations";
        internal AssociationLoader(SqlConnection connection, Sequence sequence, IDatabaseIdLookup lookup, ILogger logger) :
            base(connection, sequence, logger)
        {
            _lookup = lookup;
        }

        public bool Add(IRecord record)
        {
            switch (record)
            {
                case Association association:
                    Add(association);
                    return true;
                default:
                    return false;
            }
        }
        
        internal long Add(Association association)
        {
            var databaseId = GetNewId();
            var row = Table.NewRow();
            row["Id"] = databaseId;
            row["Action"] = ScheduleHeaderLoader.MapAction(association.Action, _logger);
            row["StpIndicator"] = association.StpIndicator.ToString();
            row["MainUid"] = association.MainUid;
            row["AssociatedUid"] = association.AssociatedUid;
            row["RunsFrom"] = association.RunsFrom;
            row["RunsTo"] = (object) association.RunsTo ?? DBNull.Value;
            row["DayMask"] = DayMaskConverter.Convert(association.DayMask);
            row["Category"] = SetNullIfEmpty(association.Category);
            row["DateIndicator"] = SetNullIfEmpty(association.DateIndicator);
            row["LocationId"] = _lookup.Find(association.Location);
            row["MainSequence"] = association.MainSequence;
            row["AssociatedSequence"] = association.AssociationSequence;
            row["AssociationType"] = SetNullIfEmpty(association.AssociationType);
            Table.Rows.Add(row);
            return databaseId;
        }
    }
}