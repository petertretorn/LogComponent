using LogTest;
using LogTest.Extensions;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Tests.Helpers;
using Xunit;

namespace Tests
{
    public class LoggerTests
    {
        
        [Fact]
        public void Can_Write_To_Log()
        {
            var expected = "test write";

            var testTime = DateTime.Now - TimeSpan.FromDays(1);

            var fileSink = new FileSink(new TestDateTimeServer(testTime));

            using (var sut = new AsyncLog(fileSink))
            {
                sut.Write(expected);
                sut.StopWithFlush();
            }

            var result = File.ReadAllText(testTime.LogPath());

            Assert.Contains(expected, result);

            File.Delete(testTime.LogPath());
        }

        [Fact]
        public void Can_Create_New_File_When_Midnight_Is_Crossed()
        {
            var beforeMidnight = DateTime.ParseExact("9/29/2020 11:51:58 PM", "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
            var afterMidnight = DateTime.ParseExact("9/30/2020 00:00:58 AM", "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);

            var timeServer = new TestDateTimeServer(beforeMidnight);
            var fileSink = new FileSink(timeServer);

            using (var sut = new AsyncLog(fileSink))
            {
                sut.Write("before midnight");

                Thread.Sleep(200);

                timeServer.ChangeTestTime( () => afterMidnight );

                sut.Write("after midnight");

                sut.StopWithFlush();
            }

            var resultBeforeMidnight = File.ReadAllText(beforeMidnight.LogPath());
            var resultAfterMidnight = File.ReadAllText(afterMidnight.LogPath());

            Assert.Contains("before midnight", resultBeforeMidnight);
            Assert.Contains("after midnight", resultAfterMidnight);

            File.Delete(beforeMidnight.LogPath());
            File.Delete(afterMidnight.LogPath());
        }

        [Fact]
        public void Can_Stop_Without_Flushing()
        {
            var testTime = DateTime.Now - TimeSpan.FromDays(2);

            var fileSink = new FileSink(new TestDateTimeServer(testTime));

            using (var sut = new AsyncLog(fileSink))
            {
                for (int i = 50; i > 0; i--)
                {
                    sut.Write("Number with No flush: " + i.ToString());
                }

                sut.StopWithoutFlush();
            }

            var lines = File.ReadAllLines(testTime.LogPath()).Count();

            Assert.True(lines < 50);

            File.Delete(testTime.LogPath());
        }

        [Fact]
        public void Can_Stop_With_Flushing()
        {
            var testTime = DateTime.Now - TimeSpan.FromDays(3);
            var fileSink = new FileSink(new TestDateTimeServer(testTime));

            using (var sut = new AsyncLog(fileSink))
            {
                for (int i = 50; i > 0; i--)
                {
                    sut.Write("Number with flushing: " + i.ToString());
                }

                sut.StopWithFlush();
            }

            var expected = 51;

            var actual = File.ReadAllLines(testTime.LogPath()).Count();

            Assert.Equal(expected, actual);

            File.Delete(testTime.LogPath());
        }

    }
}
