using CifParser.Records;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using NSubstitute;
using Serilog;
using TimetableLoader;
using Xunit;

namespace TimetableLoaderTest
{
    public class ScheduleLoaderTest : IClassFixture<IntegrationFixture>
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
BSDY31280191027                                                                P
ZZ                                                                              ";

        private readonly IntegrationFixture _fixture;

        public ScheduleLoaderTest(IntegrationFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void CreateDataTableWithLocationColumns()
        {
            using (var connection = _fixture.CreateConnection())
            {
                connection.Open();
                var schedules = new ScheduleHeaderLoader(connection, new Sequence(), Substitute.For<ILogger>());
                var loader = new ScheduleLoader(schedules, Substitute.For<ILogger>());
                loader.CreateDataTable();
                var table = schedules.Table;

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
                var loader = InitialiseLoader(connection);

                Assert.True(loader.Add(records[0]));             
            }
        }

        private ScheduleLoader InitialiseLoader(SqlConnection connection)
        {
            var schedules = new ScheduleHeaderLoader(connection, new Sequence(), Substitute.For<ILogger>());
            var loader = new ScheduleLoader(schedules, Substitute.For<ILogger>());
            loader.CreateDataTable();
            return loader;
        }

        [Fact]
        public void AddAmend()
        {
            var records = ParserHelper.ParseRecords(Records);

            using (var connection = _fixture.CreateConnection())
            {
                connection.Open();
                var loader = InitialiseLoader(connection);
                
                Assert.True(loader.Add(records[1]));                                                     
            }
        }

        [Fact]
        public void AddDelete()
        {
            var records = ParserHelper.ParseRecords(Records);

            using (var connection = _fixture.CreateConnection())
            {
                connection.Open();
                var loader = InitialiseLoader(connection);
                
                Assert.True(loader.Add(records[2]));            
            }
        }

        [Fact]
        public void OtherRecordsNotAdded()
        {
            var records = ParserHelper.ParseRecords(Records);

            using (var connection = _fixture.CreateConnection())
            {
                connection.Open();
                var loader = InitialiseLoader(connection);
                
                Assert.False(loader.Add(records[3]));
            }
        }
        
        [Fact]
        public void LoadIntoDatabase()
        {
            var records = ParserHelper.ParseRecords(Records);

            using (var connection = _fixture.CreateConnection())
            {
                connection.Open();
                var loader = InitialiseLoader(connection);

                foreach (var record in records)
                    loader.Add(record);

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
