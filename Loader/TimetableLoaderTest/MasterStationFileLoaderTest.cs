using System;
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
        
        private readonly Options TestOptions = new Options()
        {
            TimetableArchiveFile = TestArchive,
            IsRdgZip = true
        };
        
        [Fact]
        public void LoadsMasterStationFile()
        {
            var extractor = Substitute.For<IArchiveFileExtractor>();
            var loader = new MasterStationFileLoader(
                extractor,
                Substitute.For<IParser>(),
                Substitute.For<IDatabaseLoader>());

            loader.Run(TestOptions);
            
            extractor.Received().ExtractFile(TestArchive, RdgZipExtractor.StationExtension);
        }
    }
}