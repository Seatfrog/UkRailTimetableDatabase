using System.Data.SqlClient;
using CifParser.Records;

namespace TimetableLoader
{
    public interface IRecordLoader
    {
        /// <summary>
        /// Create the DataTable to load the records into
        /// </summary>
        void Initialise();
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
}