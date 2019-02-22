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
    public class AssociationLoaderTest : IClassFixture<IntegrationFixture>
    {
        private const string Records =
@"AANW00908W009091901061902100000001   FAVRSHM  T                                C
AADY55115Y56393190107                HDRSFLD  T                                N
AARL27747L288891902041902041000000NPSMANNGTR  TO                               O
ZZ";

        private readonly IntegrationFixture _fixture;

        public AssociationLoaderTest(IntegrationFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void CreateDataTableWithLocationColumns()
        {
            using (var connection = _fixture.CreateConnection())
            {
                connection.Open();
                var loader = new AssociationLoader(connection, new Sequence(), Substitute.For<IDatabaseIdLookup>(), Substitute.For<ILogger>());
                loader.Initialise();
                var table = loader.Table;

                Assert.Equal(14, table.Columns.Count);
                Assert.NotNull(table.Columns["AssociationType"]);
            }
        }

        [Fact]
        public void AddAmend()
        {
            var records = ParserHelper.ParseRecords(Records);

            using (var connection = _fixture.CreateConnection())
            {
                connection.Open();
                var lookup = Substitute.For<IDatabaseIdLookup>();
                lookup.Find("MANNGTR").Returns(34);
                
                var loader = new AssociationLoader(connection, new Sequence(), lookup, Substitute.For<ILogger>());
                loader.Initialise();

                Assert.True( loader.Add(records[2]));
                
                // AARL27747L288891902041902041000000NPSMANNGTR  TO                               O
                var row = loader.Table.Rows[0];
                Assert.Equal(1L, row["Id"]);
                Assert.Equal("U", row["Action"]);
                Assert.Equal("O", row["StpIndicator"]);
                Assert.Equal("L27747", row["MainUid"]);
                Assert.Equal("L28889", row["AssociatedUid"]);
                Assert.Equal(new DateTime(2019, 02, 04), row["RunsFrom"]);
                Assert.Equal(new DateTime(2019, 02, 04), row["RunsTo"]);
                Assert.Equal((byte) 1, row["DayMask"]);
                Assert.Equal("NP", row["Category"]);
                Assert.Equal("S", row["DateIndicator"]);             
                Assert.Equal(34L, row["LocationId"]);
                Assert.Equal(1, row["MainSequence"]);
                Assert.Equal(1, row["AssociatedSequence"]);           
                Assert.Equal("O", row["AssociationType"]);           
            }
        }

        [Fact]
        public void AddInsert()
        {
            var records = ParserHelper.ParseRecords(Records);

            using (var connection = _fixture.CreateConnection())
            {
                var lookup = Substitute.For<IDatabaseIdLookup>();
                lookup.Find("FAVRSHM").Returns(34);

                connection.Open();
                var loader = new AssociationLoader(connection, new Sequence(), lookup, Substitute.For<ILogger>());
                loader.Initialise();
                
                Assert.True( loader.Add(records[0]));
                
                // AANW00908W009091901061902100000001   FAVRSHM  T                                C
                var row = loader.Table.Rows[0];
                Assert.Equal(1L, row["Id"]);
                Assert.Equal("I", row["Action"]);
                Assert.Equal("C", row["StpIndicator"]);
                Assert.Equal("W00908", row["MainUid"]);
                Assert.Equal("W00909", row["AssociatedUid"]);
                Assert.Equal(new DateTime(2019, 01, 06), row["RunsFrom"]);
                Assert.Equal(new DateTime(2019, 02, 10), row["RunsTo"]);
                Assert.Equal((byte) 64, row["DayMask"]);
                Assert.Equal(DBNull.Value, row["Category"]);
                Assert.Equal(DBNull.Value, row["DateIndicator"]);             
                Assert.Equal(34L, row["LocationId"]);
                Assert.Equal(1, row["MainSequence"]);
                Assert.Equal(1, row["AssociatedSequence"]);           
                Assert.Equal(DBNull.Value, row["AssociationType"]);                                                         
            }
        }

        [Fact]
        public void AddDelete()
        {
            var records = ParserHelper.ParseRecords(Records);

            using (var connection = _fixture.CreateConnection())
            {
                connection.Open();
                var lookup = Substitute.For<IDatabaseIdLookup>();
                lookup.Find("HDRSFLD").Returns(34);
                
                var loader = new AssociationLoader(connection, new Sequence(), lookup, Substitute.For<ILogger>());
                loader.Initialise();

                Assert.True( loader.Add(records[1]));
                
                // AADY55115Y56393190107                HDRSFLD  T                                N
                var row = loader.Table.Rows[0];
                Assert.Equal(1L, row["Id"]);
                Assert.Equal("D", row["Action"]);
                Assert.Equal("N", row["StpIndicator"]);
                Assert.Equal("Y55115", row["MainUid"]);
                Assert.Equal("Y56393", row["AssociatedUid"]);
                Assert.Equal(new DateTime(2019, 01, 07), row["RunsFrom"]);
                Assert.Equal(DBNull.Value, row["RunsTo"]);
                Assert.Equal(DBNull.Value, row["DayMask"]);
                Assert.Equal(DBNull.Value, row["Category"]);
                Assert.Equal(DBNull.Value, row["DateIndicator"]);             
                Assert.Equal(34L, row["LocationId"]);
                Assert.Equal(1, row["MainSequence"]);
                Assert.Equal(1, row["AssociatedSequence"]);           
                Assert.Equal(DBNull.Value, row["AssociationType"]);                       
            }
        }

        
        [Fact]
        public void LoadIntoDatabase()
        {
            var records = ParserHelper.ParseRecords(Records);

            using (var connection = _fixture.CreateConnection())
            {
                connection.Open();
                var sequence = new Sequence();
                var lookup = Substitute.For<IDatabaseIdLookup>();
                lookup.Find(Arg.Any<string>()).Returns(c => sequence.GetNext());
                
                var loader = new AssociationLoader(connection, new Sequence(), lookup, Substitute.For<ILogger>());
                loader.Initialise();

                foreach (var record in records)
                    loader.Add(record);

                using (var transaction = connection.BeginTransaction())
                {
                    loader.Load(transaction);

                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = "SELECT * FROM Associations";
                        using (var adapter = new SqlDataAdapter(command))
                        {
                            var table = new DataTable();
                            adapter.Fill(table);
                            Assert.Equal(3, table.Rows.Count);
                        };
                    }
                }
            }
        }
    }
}
