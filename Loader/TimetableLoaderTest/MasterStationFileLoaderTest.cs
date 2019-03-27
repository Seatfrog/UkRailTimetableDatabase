using System;
using CifExtractor;
using CifParser;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using TimetableLoader;
using Xunit;

namespace TimetableLoaderTest
{
    public class MasterStationFileLoaderTest
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
        public void LoadsMasterStationFile()
        {
            var extractor = Substitute.For<IArchiveFileExtractor>();
            var loader = new MasterStationFileLoader(
                extractor,
                Substitute.For<IParser>(),
                Substitute.For<IDatabaseLoader>());

            loader.Run(TestConfig);
            
            extractor.Received().ExtractFile(TestArchive, RdgZipExtractor.StationExtension);
        }
    }
}