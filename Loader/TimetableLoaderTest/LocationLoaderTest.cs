using CifParser.Records;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using TimetableLoader;
using Xunit;

namespace TimetableLoaderTest
{
    public class LocationLoaderTest : IClassFixture<IntegrationFixture>
    {
        private const string Records =
@"TIPRNC84800932885DEDINBURGH SIGNAL 848      04305   0                           
TIPRNC88400932884CEDINBURGH SIGNAL 844      04310   0                           
TAWROX14 00732701NWROXHOVETON & WROXHAM SIG 48032   0                           
TDLNDRBES                                                                       
AANP19165P183661812151905180000010NPSCRDFCEN2 TO                               P
";

        private readonly IntegrationFixture _fixture;

        public LocationLoaderTest(IntegrationFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void CreateDataTableWithLocationColumns()
        {
            using (var connection = _fixture.CreateConnection())
            {
                connection.Open();
                var loader = new LocationLoader(connection, null);
                loader.CreateDataTable();
                var table = loader.Table;

                Assert.Equal(9, table.Columns.Count);
                Assert.NotNull(table.Columns["Nlc"]);
            }
        }

        [Fact]
        public void AddInsert()
        {
            var records = ParserHelper.ParseRecords(Records);

            using (var connection = _fixture.CreateConnection())
            {
                connection.Open();
                var loader = new LocationLoader(connection, new Sequence());
                loader.CreateDataTable();

                Assert.True(loader.Add(records[0]));
                
                var row = loader.Table.Rows[0];
                Assert.Equal(1, row["Id"]);
                Assert.Equal("I", row["Action"]);
                Assert.Equal("PRNC848", row["Tiploc"]);
                Assert.Equal("EDINBURGH SIGNAL 848", row["Description"]);
                Assert.Equal("932885", row["Nlc"]);
                Assert.Equal("D", row["NlcCheckCharacter"]);
                Assert.Equal(DBNull.Value, row["NlcDescription"]);
                Assert.Equal("04305", row["Stanox"]);
                Assert.Equal(DBNull.Value, row["ThreeLetterCode"]);
            }
        }

        [Fact]
        public void AddAmend()
        {
            var records = ParserHelper.ParseRecords(Records);

            using (var connection = _fixture.CreateConnection())
            {
                connection.Open();
                var loader = new LocationLoader(connection, new Sequence());
                loader.CreateDataTable();
                
                Assert.True(loader.Add(records[2]));

                var row = loader.Table.Rows[0];
                Assert.Equal(1, row["Id"]);
                Assert.Equal("U", row["Action"]);
                Assert.Equal("WROX14", row["Tiploc"]);
                Assert.Equal("WROXHOVETON & WROXHAM SIG", row["Description"]);
                Assert.Equal("732701", row["Nlc"]);
                Assert.Equal("N", row["NlcCheckCharacter"]);
                Assert.Equal(DBNull.Value, row["NlcDescription"]);
                Assert.Equal("48032", row["Stanox"]);
                Assert.Equal(DBNull.Value, row["ThreeLetterCode"]);
            }
        }

        [Fact]
        public void AddDelete()
        {
            var records = ParserHelper.ParseRecords(Records);

            using (var connection = _fixture.CreateConnection())
            {
                connection.Open();
                var loader = new LocationLoader(connection, new Sequence());
                loader.CreateDataTable();
                
                Assert.True(loader.Add(records[3]));

                var row = loader.Table.Rows[0];
                Assert.Equal(1, row["Id"]);
                Assert.Equal("D", row["Action"]);
                Assert.Equal("LNDRBES", row["Tiploc"]);
                Assert.Equal(DBNull.Value, row["Description"]);
                Assert.Equal(DBNull.Value, row["Nlc"]);
                Assert.Equal(DBNull.Value, row["NlcCheckCharacter"]);
                Assert.Equal(DBNull.Value, row["NlcDescription"]);
                Assert.Equal(DBNull.Value, row["Stanox"]);
                Assert.Equal(DBNull.Value, row["ThreeLetterCode"]);
            }
        }

        [Fact]
        public void OtherRecordsNotAdded()
        {
            var records = ParserHelper.ParseRecords(Records);

            using (var connection = _fixture.CreateConnection())
            {
                connection.Open();
                var loader = new LocationLoader(connection, new Sequence());
                loader.CreateDataTable();
                
                Assert.False(loader.Add(records[4]));

                Assert.Equal(0, loader.Table.Rows.Count);
            }
        }
        
        [Fact]
        public void CreatesLookup()
        {
            var records = ParserHelper.ParseRecords(Records);
            var expected = records.OfType<Tiploc>().Select(r => r.Code);

            using (var connection = _fixture.CreateConnection())
            {
                connection.Open();
                var loader = new LocationLoader(connection, new Sequence());
                loader.CreateDataTable();

                foreach (var record in records)
                    loader.Add(record);
                
                Assert.All(expected, t => Assert.True(loader.Lookup.ContainsKey(t)));
            }
        }

        [Fact]
        public void LoadIntoDatabase()
        {
            var records = ParserHelper.ParseRecords(Records);

            using (var connection = _fixture.CreateConnection())
            {
                connection.Open();
                var loader = new LocationLoader(connection, new Sequence());
                loader.CreateDataTable();

                foreach (var record in records)
                    loader.Add(record);

                using (var transaction = connection.BeginTransaction())
                {
                    loader.Load(transaction);

                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = "SELECT * FROM Locations";
                        using (var adapter = new SqlDataAdapter(command))
                        {
                            var table = new DataTable();
                            adapter.Fill(table);
                            Assert.Equal(4, table.Rows.Count);
                        };
                    }
                }
            }
        }
        
        [Fact]
        public void FindExistingTiploc()
        {
            var records = ParserHelper.ParseRecords(Records);

            using (var connection = _fixture.CreateConnection())
            {
                connection.Open();
                var loader = new LocationLoader(connection, new Sequence());
                loader.CreateDataTable();

                foreach (var record in records)
                    loader.Add(record);

                var id = loader.Find("PRNC884");
                Assert.Equal(2, id);
            }
        }
        
        [Fact]
        public void FindMissingTiploc()
        {
            var records = ParserHelper.ParseRecords(Records);

            using (var connection = _fixture.CreateConnection())
            {
                connection.Open();
                var loader = new LocationLoader(connection, new Sequence());
                loader.CreateDataTable();

                foreach (var record in records)
                    loader.Add(record);

                var id = loader.Find("WATRLOO");
                Assert.Equal(5, id);
                
                var row = loader.Table.Rows[4];
                Assert.Equal(5, row["Id"]);
                Assert.Equal("I", row["Action"]);
                Assert.Equal("WATRLOO", row["Tiploc"]);
                Assert.Equal("WATRLOO - MISSING", row["Description"]);
                Assert.Equal(DBNull.Value, row["Nlc"]);
                Assert.Equal(DBNull.Value, row["NlcCheckCharacter"]);
                Assert.Equal(DBNull.Value, row["NlcDescription"]);
                Assert.Equal(DBNull.Value, row["Stanox"]);
                Assert.Equal(DBNull.Value, row["ThreeLetterCode"]);
            }
        }
    }
}
