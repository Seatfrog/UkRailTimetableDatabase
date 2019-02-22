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
BSRG760721902021902020000010 POO2Y72    121386001 EMU360 100D     S            O
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
ZZ                                                                              ";

        private readonly IntegrationFixture _fixture;
        
        private ScheduleHeaderLoader _schedules;
        private ScheduleLocationLoader _locations;
        private ScheduleChangeLoader _changes;

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
                var loader = InitialiseLoader(connection);

                Assert.NotNull(_schedules.Table);
                Assert.NotNull(_locations.Table);
                Assert.NotNull(_changes.Table);                
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
            var sequence = new Sequence();
            var lookup = Substitute.For<IDatabaseIdLookup>();
            lookup.Find(Arg.Any<string>()).Returns(c => sequence.GetNext());
            _schedules = new ScheduleHeaderLoader(connection, new Sequence(), Substitute.For<ILogger>());
            _locations = new ScheduleLocationLoader(connection, new Sequence(), lookup, Substitute.For<ILogger>());
            _changes = new ScheduleChangeLoader(connection, new Sequence(), Substitute.For<ILogger>());
            var loader = new ScheduleLoader(_schedules, _locations, _changes, Substitute.For<ILogger>());
            loader.Initialise();
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
                
                Assert.False(loader.Add(records[4]));
            }
        }
        
        [Fact]
        public void AddScheduleRows()
        {
            var records = ParserHelper.ParseRecords(Records);

            using (var connection = _fixture.CreateConnection())
            {
                connection.Open();
                var loader = InitialiseLoader(connection);

                Assert.True(loader.Add(records[0])); 
                
                Assert.NotEmpty(_schedules.Table.Rows);
            }
        }
        
        [Fact]
        public void AddLocationRows()
        {
            var records = ParserHelper.ParseRecords(Records);

            using (var connection = _fixture.CreateConnection())
            {
                connection.Open();
                var loader = InitialiseLoader(connection);

                Assert.True(loader.Add(records[0])); 
                
                Assert.NotEmpty(_locations.Table.Rows);
            }
        }
        
        [Fact]
        public void AddChangeRows()
        {
            var records = ParserHelper.ParseRecords(Records);

            using (var connection = _fixture.CreateConnection())
            {
                connection.Open();
                var loader = InitialiseLoader(connection);

                Assert.True(loader.Add(records[3]));
                
                Assert.NotEmpty(_changes.Table.Rows);
            }
        }
        
        [Fact]
        public void ChangeRowLinkedToNextLocation()
        {
            var records = ParserHelper.ParseRecords(Records);

            using (var connection = _fixture.CreateConnection())
            {
                connection.Open();
                var loader = InitialiseLoader(connection);

                Assert.True(loader.Add(records[3]));

                var locationId = _changes.Table.Rows[0]["ScheduleLocationId"];

                _locations.Table.PrimaryKey = new [] { _locations.Table.Columns["id"] };
                var locationRow = _locations.Table.Rows.Find(locationId);
                
                Assert.Equal(new TimeSpan(23, 28, 0), locationRow["PublicArrival"]);
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

                    void AssertInsertedRecords(string sql)
                    {
                        using (var command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;
                            command.CommandText = sql;
                            using (var adapter = new SqlDataAdapter(command))
                            {
                                var table = new DataTable();
                                adapter.Fill(table);
                                Assert.NotEmpty(table.Rows);
                            }
                        }
                    }

                    AssertInsertedRecords(@"SELECT * FROM Schedules");
                    AssertInsertedRecords(@"SELECT * FROM ScheduleLocations");
                    AssertInsertedRecords(@"SELECT * FROM ScheduleChanges");
                }
            }
        }
    }
}
