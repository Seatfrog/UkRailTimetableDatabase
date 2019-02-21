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
    public class ScheduleChangeLoaderTest : IClassFixture<IntegrationFixture>
    {
        private const string Records =
@"BSRG760721902021902020000010 POO2Y72    121386001 EMU360 100D     S            O
BX         XRY                                                                  
LOHTRWTM4 2316 23162         TB                                                 
LIHTRWAPT 2320 2322      232023222        T                                     
LIHTRWTJN           2325 00000000                                               
LISTKYJN            2325H00000000   ML                                          
LIHTRWAJN           2326 00000000   ML                   H                      
CRHAYESAH OO2Y72    121384001 EMU360 100D     S                                 
LIHAYESAH 2327H2328      232823282     ML T                                     
LISTHALL  2331 2331H     233123312  ML    T                                     
LISTHALEJ           2332H00000000   RL                                          
LIHANWELL 2333H2334      233423343        T                                     
LIWEALING 2335H2336      233623364  RL    T                                     
LIEALINGB 2338 2339      233823394  RL    T                                     
LIACTONW            2340 00000000   RL RL             1                         
LILDBRKJ            2346 00000000   3  RL                                       
LIPRTOBJP           2347 00000000   2                                           
LIROYAOJN           2348 00000000                                               
LTPADTON  2349 23503     TF                                                     
";

        private readonly IntegrationFixture _fixture;

        public ScheduleChangeLoaderTest(IntegrationFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void CreateDataTableWithLocationColumns()
        {
            using (var connection = _fixture.CreateConnection())
            {
                connection.Open();
                var loader = new ScheduleChangeLoader(connection, new Sequence(), Substitute.For<ILogger>());
                loader.CreateDataTable();
                var table = loader.Table;

                Assert.Equal(19, table.Columns.Count);
                Assert.NotNull(table.Columns["ScheduleLocationId"]);
            }
        }

        [Fact]
        public void AddChange()
        {
            var records = ParserHelper.ParseRecords(Records);

            using (var connection = _fixture.CreateConnection())
            {
                connection.Open();
                var loader = new ScheduleChangeLoader(connection, new Sequence(), Substitute.For<ILogger>());
                loader.CreateDataTable();

                var schedule = records[0] as Schedule;
                var change = schedule.Records[7] as ScheduleChange;
                loader.Add(12, 34, change);

                // CRHAYESAH OO2Y72    121384001 EMU360 100D     S                                 
                var row = loader.Table.Rows[0];
                Assert.Equal(1L, row["Id"]);
                Assert.Equal(12L, row["ScheduleId"]);
                Assert.Equal(34L, row["ScheduleLocationId"]);
                Assert.Equal("OO", row["Category"]);
                Assert.Equal("2Y72", row["TrainIdentity"]);
                Assert.Equal(DBNull.Value, row["NrsHeadCode"]);
                Assert.Equal("21384001", row["ServiceCode"]);
                Assert.Equal(DBNull.Value, row["PortionId"]);
                Assert.Equal("EMU", row["PowerType"]);
                Assert.Equal("360", row["TimingLoadType"]);
                Assert.Equal(100, row["Speed"]);
                Assert.Equal("D", row["OperatingCharacteristics"]);
                Assert.Equal("S", row["SeatClass"]);
                Assert.Equal(DBNull.Value, row["SleeperClass"]);
                Assert.Equal("", row["ReservationIndicator"]);
                Assert.Equal(DBNull.Value, row["Catering"]);
                Assert.Equal(DBNull.Value, row["Branding"]);
                Assert.Equal(DBNull.Value, row["EuropeanUic"]);
                Assert.Equal(DBNull.Value, row["RetailServiceId"]);
            }
        }

        [Fact]
        public void LoadIntoDatabase()
        {
            var records = ParserHelper.ParseRecords(Records);

            using (var connection = _fixture.CreateConnection())
            {
                connection.Open();
                var loader = new ScheduleChangeLoader(connection, new Sequence(), Substitute.For<ILogger>());
                loader.CreateDataTable();

                var schedule = records[0] as Schedule;
                var change = schedule.Records[7] as ScheduleChange;
                loader.Add(12, 34, change);

                using (var transaction = connection.BeginTransaction())
                {
                    loader.Load(transaction);

                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = "SELECT * FROM ScheduleChanges";
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