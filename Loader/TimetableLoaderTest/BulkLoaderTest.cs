using System;
using CifParser.Records;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using Serilog;
using TimetableLoader;
using Xunit;

namespace TimetableLoaderTest
{
    public class BulkLoaderTest
    {
        
        
        [Fact]
        public void AddRecordInFirstLoader()
        {
            var loader1 = Substitute.For<ILoader>();
            loader1.Add(Arg.Any<ICifRecord>()).Returns(true);
            
            var loader2 = Substitute.For<ILoader>();
            
            var loader = new BulkLoader(new [] { loader1, loader2}, Substitute.For<ILogger>());
            
            loader.Load(new ICifRecord[] { new Header() });

            loader1.ReceivedWithAnyArgs().Add(null);
            loader2.DidNotReceiveWithAnyArgs().Add(null);
        }
        
        [Fact]
        public void AddRecordInSecondLoader()
        {
            var loader1 = Substitute.For<ILoader>();
            loader1.Add(Arg.Any<ICifRecord>()).Returns(false);
            
            var loader2 = Substitute.For<ILoader>();
            loader2.Add(Arg.Any<ICifRecord>()).Returns(true);
           
            var loader = new BulkLoader(new [] { loader1, loader2}, Substitute.For<ILogger>());
            
            loader.Load(new ICifRecord[] { new Header() });

            loader2.ReceivedWithAnyArgs().Add(null);
         }
        
        [Fact]
        public void DropThroughLogsWarning()
        {
            var logger = Substitute.For<ILogger>();
            var loader1 = Substitute.For<ILoader>();
            loader1.Add(Arg.Any<ICifRecord>()).Returns(false);
            
            var loader2 = Substitute.For<ILoader>();
            loader2.Add(Arg.Any<ICifRecord>()).Returns(false);
           
            var loader = new BulkLoader(new [] { loader1, loader2}, logger);
            
            loader.Load(new ICifRecord[] { new Header() });

            logger.ReceivedWithAnyArgs().Warning<Type, ICifRecord>(messageTemplate: null, propertyValue0: null, propertyValue1: null);
        }
    }
}