using CifParser.Records;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace TimetableLoader
{
    public interface ILoader
    {
        /// <summary>
        /// Add a record to the DataTable
        /// </summary>
        /// <param name="record"></param>
        /// <returns>Whether added record</returns>
        bool Add(ICifRecord record);

        /// <summary>
        /// Load the DataTable into the database
        /// </summary>
        /// <param name="transaction"></param>
        void Load(SqlTransaction transaction);
    }

    /// <summary>
    /// Bulk load Locations
    /// </summary>
    /// <remarks> This has a built in assumption that loading a Full CIF file and so there are only TiplocInsert records (as it expccts Tiploc code to be unique)
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
    internal class LocationLoader : ILoader
    {
        private readonly SqlConnection _connection;
        private readonly Sequence _sequence;

        internal DataTable Table { get; private set; }

        /// <summary>
        /// Provides the lookup from Tiploc to database Id
        /// </summary>
        internal IDictionary<string, int> Lookup { get; } = new Dictionary<string, int>();

        public LocationLoader(SqlConnection connection, Sequence sequence)
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
                command.CommandText = "SELECT TOP 0 * FROM Locations";
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
                case TiplocInsertAmend insertAmend:
                    Add(insertAmend);
                    return true;
                case TiplocDelete delete:
                    Add(delete);
                    return true;
                default:
                    return false;
            }
        }

        private void Add(TiplocInsertAmend record)
        {
            var row = Table.NewRow();
            row["Id"] = SetNewId(record.Code);
            row["Action"] = record.Action == RecordAction.Create ? "C" : "U";
            row["Tiploc"] = record.Code;
            row["Description"] = record.Description;
            row["Nlc"] = record.Nalco;
            row["NlcCheckCharacter"] = record.NalcoCheckCharacter;
            row["NlcDescription"] = SetNullIfEmpty(record.NlcDescription);
            row["Stanox"] = record.Stanox;
            row["ThreeLetterCode"] = SetNullIfEmpty(record.ThreeLetterCode);
            Table.Rows.Add(row);
        }

        private int SetNewId(string tiploc)
        {
            var newId = _sequence.GetNext();
            Lookup.Add(tiploc, newId);
            return newId;
        }  
            
        private object SetNullIfEmpty(string value)
        {
            return string.IsNullOrEmpty(value) ? (object) DBNull.Value : value;
        }

        private void Add(TiplocDelete record)
        {
            var row = Table.NewRow();
            row["Id"] = SetNewId(record.Code);
            row["Action"] = "D";
            row["Tiploc"] = record.Code;
            Table.Rows.Add(row);
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
                    bulk.DestinationTableName = "Locations";
                    bulk.WriteToServer(Table);
                }
                catch (Exception ex)
                {
                    Serilog.Log.Error(ex, "Error loading locations");
                    throw;
                }
            }
        }
    }
}
