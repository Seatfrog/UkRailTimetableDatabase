using CifParser.Records;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using CifParser;
using NSubstitute;
using Serilog;
using TimetableLoader;
using Xunit;

namespace TimetableLoaderTest
{
    public class StationLoaderTest : IClassFixture<IntegrationFixture>
    {
        private const string Records =
@"A    TAMWORTH HL                   9TMWTHHLTAH   TAM14213 63045 5                 ";

        private readonly IntegrationFixture _fixture;

        public StationLoaderTest(IntegrationFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void CreateDataTableWithLocationColumns()
        {
            using (var connection = _fixture.CreateConnection())
            {
                connection.Open();
                var loader = new StationLoader(connection, new Sequence(), Substitute.For<ILogger>());
                loader.Initialise();
                var table = loader.Table;

                Assert.Equal(10, table.Columns.Count);
                Assert.NotNull(table.Columns["SubsidiaryThreeLetterCode"]);
            }
        }

        [Fact]
        public void AddChange()
        {
            var records = ParserHelper.ParseStationRecords(Records);

            using (var connection = _fixture.CreateConnection())
            {
                connection.Open();
                var loader = new StationLoader(connection, new Sequence(), Substitute.For<ILogger>());
                loader.Initialise();

                loader.Add(records[0]);

                // A    TAMWORTH HL                   9TMWTHHLTAH   TAM14213 63045 5                                
                var row = loader.Table.Rows[0];
                Assert.Equal(1L, row["Id"]);
                Assert.Equal("TMWTHHL", row["Tiploc"]);
                Assert.Equal("TAMWORTH HL", row["Description"]);
                Assert.Equal((byte)9, row["InterchangeStatus"]);
                Assert.Equal("TAM", row["ThreeLetterCode"]);
                Assert.Equal("TAH", row["SubsidiaryThreeLetterCode"]);
                Assert.Equal(14213, row["Eastings"]);
                Assert.Equal(63045, row["Northings"]);
                Assert.Equal(false, row["LocationIsEstimated"]);
                Assert.Equal((byte)5, row["MinimumChangeTime"]);            
            }
        }

        [Fact]
        public void LoadIntoDatabase()
        {
            var records = ParserHelper.ParseStationRecords(Records);

            using (var connection = _fixture.CreateConnection())
            {
                connection.Open();
                var loader = new StationLoader(connection, new Sequence(), Substitute.For<ILogger>());
                loader.Initialise();

                loader.Add(records[0]);

                using (var transaction = connection.BeginTransaction())
                {
                    loader.Load(transaction);

                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = "SELECT * FROM Stations";
                        using (var adapter = new SqlDataAdapter(command))
                        {
                            var table = new DataTable();
                            adapter.Fill(table);
                            Assert.Equal(1, table.Rows.Count);
                        }

                        ;
                    }
                }
            }
        }
    }
}