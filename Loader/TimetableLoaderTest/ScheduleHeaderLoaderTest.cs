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
    public class ScheduleHeaderLoaderTest : IClassFixture<IntegrationFixture>
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
BSRC564831910261912140000010 POO2P94    125473001 DMUS   075      S R          O
BX         GWN                                                                  
LOGUNISLK 1913 1913          TB                                                 
LICALSTCK 1924H1925      19241924         T                                     
LIBEREALS 1931 1934      19311933         T RM                                  
LIBEREFRS 1938H1939      19381938         T                                     
LISTBDXVR 1946 1947H     19461947         T                                     
LISTBDXJN           1949 00000000                                               
LIKEYHAM  1950 1950H     194919492        T                                     
LIDOCKYDP 1952 1952H     195119512        R                                     
LIDEVNPRT 1954 1954H     195319532        T                                     
LTPLYMTH  1958 19586     TF                                                     
BSDY31280191027                                                                P";

        private readonly IntegrationFixture _fixture;

        public ScheduleHeaderLoaderTest(IntegrationFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void CreateDataTableWithLocationColumns()
        {
            using (var connection = _fixture.CreateConnection())
            {
                connection.Open();
                var loader = new ScheduleHeaderLoader(connection, new Sequence(), Substitute.For<ILogger>());
                loader.CreateDataTable();
                var table = loader.Table;

                Assert.Equal(27, table.Columns.Count);
                Assert.NotNull(table.Columns["TimetableUid"]);
            }
        }

        [Fact]
        public void AddInsert()
        {
            var records = ParserHelper.ParseRecords(Records);

            using (var connection = _fixture.CreateConnection())
            {
                connection.Open();
                var loader = new ScheduleHeaderLoader(connection, new Sequence(), Substitute.For<ILogger>());
                loader.CreateDataTable();

                var schedule = records[0] as Schedule;
                loader.Add(schedule.GetId(), schedule.GetScheduleDetails(), schedule.GetScheduleExtraDetails());
                
                var row = loader.Table.Rows[0];
                Assert.Equal(1, row["Id"]);
                Assert.Equal("C", row["Action"]);
                Assert.Equal("P", row["StpIndicator"]);
                Assert.Equal("C55855", row["TimetableUid"]);
                Assert.Equal(new DateTime(2019, 10, 26), row["RunsFrom"]);
                Assert.Equal(new DateTime(2019, 12, 14), row["RunsTo"]);
                Assert.Equal((byte) 32, row["DayMask"]);
                Assert.Equal("", row["BankHolidayRunning"]);
                Assert.Equal("P", row["Status"]);
                Assert.Equal("EE", row["Category"]);
                Assert.Equal("5P92", row["TrainIdentity"]);
                Assert.Equal(DBNull.Value, row["NrsHeadCode"]);
                Assert.Equal("25476001", row["ServiceCode"]);
                Assert.Equal(DBNull.Value, row["PortionId"]);                
                Assert.Equal("DMU", row["PowerType"]);                
                Assert.Equal("S", row["TimingLoadType"]);                
                Assert.Equal(75, row["Speed"]);
                Assert.Equal(DBNull.Value, row["OperatingCharacteristics"]);                
                Assert.Equal("B", row["SeatClass"]);                
                Assert.Equal(DBNull.Value, row["SleeperClass"]);                
                Assert.Equal("", row["ReservationIndicator"]);                
                Assert.Equal(DBNull.Value, row["Catering"]);                
                Assert.Equal(DBNull.Value, row["Branding"]);                
                Assert.Equal(DBNull.Value, row["EuropeanUic"]);                
                Assert.Equal("GW", row["Toc"]);                
                Assert.Equal(true, row["ApplicableTimetable"]);                
                Assert.Equal(DBNull.Value, row["RetailServiceId"]);                
            }
        }

        [Fact]
        public void AddAmend()
        {
            var records = ParserHelper.ParseRecords(Records);

            using (var connection = _fixture.CreateConnection())
            {
                connection.Open();
                var loader = new ScheduleHeaderLoader(connection, new Sequence(), Substitute.For<ILogger>());
                loader.CreateDataTable();
                
                var schedule = records[1] as Schedule;
                loader.Add(schedule.GetId(), schedule.GetScheduleDetails(), schedule.GetScheduleExtraDetails());

                var row = loader.Table.Rows[0];
                Assert.Equal(1, row["Id"]);
                Assert.Equal("U", row["Action"]);
                Assert.Equal("O", row["StpIndicator"]);
                Assert.Equal("C56483", row["TimetableUid"]);
                Assert.Equal(new DateTime(2019, 10, 26), row["RunsFrom"]);
                Assert.Equal(new DateTime(2019, 12, 14), row["RunsTo"]);
                Assert.Equal((byte) 32, row["DayMask"]);
                Assert.Equal("", row["BankHolidayRunning"]);
                Assert.Equal("P", row["Status"]);
                Assert.Equal("OO", row["Category"]);
                Assert.Equal("2P94", row["TrainIdentity"]);
                Assert.Equal(DBNull.Value, row["NrsHeadCode"]);
                Assert.Equal("25473001", row["ServiceCode"]);
                Assert.Equal(DBNull.Value, row["PortionId"]);                
                Assert.Equal("DMU", row["PowerType"]);                
                Assert.Equal("S", row["TimingLoadType"]);                
                Assert.Equal(75, row["Speed"]);
                Assert.Equal(DBNull.Value, row["OperatingCharacteristics"]);                
                Assert.Equal("S", row["SeatClass"]);                
                Assert.Equal(DBNull.Value, row["SleeperClass"]);                
                Assert.Equal("R", row["ReservationIndicator"]);                
                Assert.Equal(DBNull.Value, row["Catering"]);                
                Assert.Equal(DBNull.Value, row["Branding"]);                
                Assert.Equal(DBNull.Value, row["EuropeanUic"]);                
                Assert.Equal("GW", row["Toc"]);                
                Assert.Equal(false, row["ApplicableTimetable"]);                
                Assert.Equal(DBNull.Value, row["RetailServiceId"]);                                                       
            }
        }

        [Fact]
        public void AddDelete()
        {
            var records = ParserHelper.ParseRecords(Records);

            using (var connection = _fixture.CreateConnection())
            {
                connection.Open();
                var loader = new ScheduleHeaderLoader(connection, new Sequence(), Substitute.For<ILogger>());
                loader.CreateDataTable();
                
                var schedule = records[2] as Schedule;
                loader.Add(schedule.GetId(), schedule.GetScheduleDetails(), schedule.GetScheduleExtraDetails());

                var row = loader.Table.Rows[0];
                Assert.Equal(1, row["Id"]);
                Assert.Equal("D", row["Action"]);
                Assert.Equal("P", row["StpIndicator"]);
                Assert.Equal("Y31280", row["TimetableUid"]);
                Assert.Equal(new DateTime(2019, 10, 27), row["RunsFrom"]);

                //Everything else is null
                Assert.Equal(22, row.ItemArray.Count(o => DBNull.Value.Equals(o)));               
            }
        }
        
        [Fact]
        public void LoadIntoDatabase()
        {
            var records = ParserHelper.ParseRecords(Records);

            using (var connection = _fixture.CreateConnection())
            {
                connection.Open();
                var loader = new ScheduleHeaderLoader(connection, new Sequence(), Substitute.For<ILogger>());
                loader.CreateDataTable();

                foreach (var record in records)
                {
                    var schedule = record as Schedule;
                    loader.Add(schedule.GetId(), schedule.GetScheduleDetails(), schedule.GetScheduleExtraDetails());
                }

                using (var transaction = connection.BeginTransaction())
                {
                    loader.Load(transaction);

                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = "SELECT * FROM Schedules";
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
