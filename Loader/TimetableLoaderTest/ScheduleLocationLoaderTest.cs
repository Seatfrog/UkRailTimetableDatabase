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
    public class ScheduleLocationLoaderTest : IClassFixture<IntegrationFixture>
    {
        private const string Records =
@"BSNC558551910261912140000010 PEE5P92    125476001 DMUS   075                   P
BX         GWY                                                                  
LOLISKARD 2042 00003  BAY    TB                                                 
LILISKDGF 2043 2047      00000000   JNL   OPRM                                  
LILISKARD22052 2055      000000002        OPRM                                  
LIMENH259           2101 00000000                                               
LISTGRMNS           2104H000000002                       H                      
LISASH              2110 000000002                                              
LISTBDXJN           2113H00000000                     1                         
LTPLYMTH  2119 00003     TF                                                     
";

        private readonly IntegrationFixture _fixture;

        public ScheduleLocationLoaderTest(IntegrationFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void CreateDataTableWithLocationColumns()
        {
            using (var connection = _fixture.CreateConnection())
            {
                connection.Open();
                var loader = new ScheduleLocationLoader(connection, new Sequence(), Substitute.For<IDatabaseIdLookup>(), Substitute.For<ILogger>());
                loader.Initialise();
                var table = loader.Table;

                Assert.Equal(16, table.Columns.Count);
                Assert.NotNull(table.Columns["PublicArrival"]);
            }
        }

        [Fact]
        public void AddOrigin()
        {
            var records = ParserHelper.ParseRecords(Records);

            using (var connection = _fixture.CreateConnection())
            {
                connection.Open();
                var lookup = Substitute.For<IDatabaseIdLookup>();
                lookup.Find("LISKARD").Returns(34);
                
                var loader = new ScheduleLocationLoader(connection, new Sequence(), lookup, Substitute.For<ILogger>());
                loader.Initialise();

                var schedule = records[0] as Schedule;
                loader.Add(12, schedule.Records[2] as OriginLocation);
                
                // LOLISKARD 2042 00003  BAY    TB                                                 
                var row = loader.Table.Rows[0];
                Assert.Equal(1L, row["Id"]);
                Assert.Equal(12L, row["ScheduleId"]);
                Assert.Equal(34L, row["LocationId"]);
                Assert.Equal(1, row["Sequence"]);
                Assert.Equal(DBNull.Value, row["WorkingArrival"]);            
                Assert.Equal(new TimeSpan(20, 42, 0), row["WorkingDeparture"]);            
                Assert.Equal(DBNull.Value, row["WorkingPass"]);            
                Assert.Equal(DBNull.Value, row["PublicArrival"]);            
                Assert.Equal(DBNull.Value, row["PublicDeparture"]);            
                Assert.Equal("3", row["Platform"]);            
                Assert.Equal("BAY", row["Line"]);            
                Assert.Equal(DBNull.Value, row["Path"]);            
                Assert.Equal("TB", row["Activities"]);            
                Assert.Equal("", row["EngineeringAllowance"]);            
                Assert.Equal("", row["PathingAllowance"]);            
                Assert.Equal("", row["PerformanceAllowance"]);            
            }
        }

        [Fact]
        public void AddIntermediateLocation()
        {
            var records = ParserHelper.ParseRecords(Records);

            using (var connection = _fixture.CreateConnection())
            {
                connection.Open();
                var lookup = Substitute.For<IDatabaseIdLookup>();
                lookup.Find("LISKDGF").Returns(34);
                
                var loader = new ScheduleLocationLoader(connection, new Sequence(), lookup, Substitute.For<ILogger>());
                loader.Initialise();

                var schedule = records[0] as Schedule;
                loader.Add(12, schedule.Records[3] as IntermediateLocation);
                
                // LILISKDGF 2043 2047      00000000   JNL   OPRM                                  
                var row = loader.Table.Rows[0];
                Assert.Equal(1L, row["Id"]);
                Assert.Equal(12L, row["ScheduleId"]);
                Assert.Equal(34L, row["LocationId"]);
                Assert.Equal(1, row["Sequence"]);
                Assert.Equal(new TimeSpan(20, 43, 0), row["WorkingArrival"]);            
                Assert.Equal(new TimeSpan(20, 47, 0), row["WorkingDeparture"]);            
                Assert.Equal(DBNull.Value, row["WorkingPass"]);            
                Assert.Equal(DBNull.Value, row["PublicArrival"]);            
                Assert.Equal(DBNull.Value, row["PublicDeparture"]);            
                Assert.Equal("", row["Platform"]);            
                Assert.Equal("JNL", row["Line"]);            
                Assert.Equal("", row["Path"]);            
                Assert.Equal("OPRM", row["Activities"]);            
                Assert.Equal("", row["EngineeringAllowance"]);            
                Assert.Equal("", row["PathingAllowance"]);            
                Assert.Equal("", row["PerformanceAllowance"]);            
            }
        }

        [Fact]
        public void AddTerminalLocation()
        {
            var records = ParserHelper.ParseRecords(Records);

            using (var connection = _fixture.CreateConnection())
            {
                connection.Open();
                var lookup = Substitute.For<IDatabaseIdLookup>();
                lookup.Find("PLYMTH").Returns(34);
                
                var loader = new ScheduleLocationLoader(connection, new Sequence(), lookup, Substitute.For<ILogger>());
                loader.Initialise();

                var schedule = records[0] as Schedule;
                loader.Add(12, schedule.Records[9] as TerminalLocation);
                
                // LTPLYMTH  2119 00003     TF                                                     
                var row = loader.Table.Rows[0];
                Assert.Equal(1L, row["Id"]);
                Assert.Equal(12L, row["ScheduleId"]);
                Assert.Equal(34L, row["LocationId"]);
                Assert.Equal(1, row["Sequence"]);
                Assert.Equal(new TimeSpan(21, 19, 0), row["WorkingArrival"]);            
                Assert.Equal(DBNull.Value, row["WorkingDeparture"]);            
                Assert.Equal(DBNull.Value, row["WorkingPass"]);            
                Assert.Equal(DBNull.Value, row["PublicArrival"]);            
                Assert.Equal(DBNull.Value, row["PublicDeparture"]);            
                Assert.Equal("3", row["Platform"]);            
                Assert.Equal(DBNull.Value, row["Line"]);            
                Assert.Equal("", row["Path"]);            
                Assert.Equal("TF", row["Activities"]);            
                Assert.Equal(DBNull.Value, row["EngineeringAllowance"]);            
                Assert.Equal(DBNull.Value, row["PathingAllowance"]);            
                Assert.Equal(DBNull.Value, row["PerformanceAllowance"]);            
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
                
                var loader = new ScheduleLocationLoader(connection, new Sequence(), lookup, Substitute.For<ILogger>());
                loader.Initialise();

                var schedule = records[0] as Schedule;
                loader.Add(12, schedule.Records[2] as OriginLocation);
                loader.Add(12, schedule.Records[3] as IntermediateLocation);
                loader.Add(12, schedule.Records[9] as TerminalLocation);

                using (var transaction = connection.BeginTransaction())
                {
                    loader.Load(transaction);

                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = "SELECT * FROM ScheduleLocations";
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
