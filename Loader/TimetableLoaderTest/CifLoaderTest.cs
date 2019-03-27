using System.Data.SqlClient;
using CifParser;
using NSubstitute;
using TimetableLoader;
using Xunit;

namespace TimetableLoaderTest
{
    public class CifLoaderTest
    {
        private const string TestArchive = "Dummy.zip";

        private ILoaderConfig TestConfig
        {
            get
            {
                var config = Substitute.For<ILoaderConfig>();
                config.TimetableArchiveFile.Returns(TestArchive);
                config.IsRdgZip.Returns(true);
                return config;
            }
        }

        [Fact]
        public void LoadsCifFile()
        {
            var extractor = Substitute.For<IExtractor>();
            var factory = CreateStubFactory();
            factory.CreateExtractor().Returns(extractor);
            
            var loader = new CifLoader(factory);

            loader.Run(TestConfig);
            
            extractor.Received().ExtractCif(TestArchive);
        }
             
        [Fact]
        public void LoadsMasterStationFileWhenRdgZip()
        {
            var extractor = Substitute.For<IArchiveFileExtractor>();
            var factory = CreateStubFactory();
            factory.CreateStationLoader(Arg.Any<IDatabase>()).Returns(new MasterStationFileLoader(
                extractor,
                Substitute.For<IParser>(),
                Substitute.For<IDatabaseLoader>()));
            
            var loader = new CifLoader(factory);

            var config = Substitute.For<ILoaderConfig>();
            config.TimetableArchiveFile.Returns(TestArchive);
            config.IsRdgZip.Returns(false);

            loader.Run(config);
            
            extractor.DidNotReceive().ExtractFile(Arg.Any<string>(), RdgZipExtractor.StationExtension);
        }

        private static IFactory CreateStubFactory()
        {
            var factory = Substitute.For<IFactory>();

            var parser = Substitute.For<IParser>();
            factory.CreateParser().Returns(parser);
            
            var db = Substitute.For<IDatabase>();
            factory.CreateDatabase().Returns(db);
            
            return factory;
        }

        [Fact]
        public void DoesNotLoadMasterStationFileWhenNrodArchive()
        {
            var extractor = Substitute.For<IArchiveFileExtractor>();
            var factory = CreateStubFactory();
            factory.CreateStationLoader(Arg.Any<IDatabase>()).Returns(new MasterStationFileLoader(
                extractor,
                Substitute.For<IParser>(),
                Substitute.For<IDatabaseLoader>()));
            
            var loader = new CifLoader(factory);

            loader.Run(TestConfig);
            
            extractor.Received().ExtractFile(TestArchive, RdgZipExtractor.StationExtension);
        }
    }
}