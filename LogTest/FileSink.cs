using LogTest.Interfaces;
using System;
using System.IO;
using LogTest.Extensions;

namespace LogTest
{
    public class FileSink : ILogSink
    {
        private StreamWriter _writer;
        DateTime _registeredDate;
        public IDateTimeServer TimeServer { get; private set; }

        private readonly ILogRotationRule _rotationRule;

        public FileSink(IDateTimeServer timeServer, ILogRotationRule rotationRule)
        {
            this.TimeServer = timeServer;
            this._rotationRule = rotationRule;

            this._registeredDate = TimeServer.Now;

            if (!Directory.Exists(@"C:\LogTest"))
                Directory.CreateDirectory(@"C:\LogTest");

            CreateNewWriter();
        }

        public FileSink() : this(new DateTimeServer(), new DailyRotationRule() ) { }
        public FileSink(IDateTimeServer timeServer) : this(timeServer, new DailyRotationRule()) { }

        public void Write(string content)
        {
            if (_rotationRule.Evaluate(TimeServer.Now, _registeredDate))
            {
                _registeredDate = TimeServer.Now;

                CreateNewWriter();
            }

            this._writer.Write(content);
        }

        private void CreateNewWriter()
        {
            this._writer?.Dispose();

            var path = TimeServer.Now.LogPath();
            this._writer = File.AppendText(path);
            
            string header = CreateLogHeader();
            
            this._writer.Write(header);

            this._writer.AutoFlush = true;
        }

        private static string CreateLogHeader()
        {
            return $"{"Timestamp".PadRight(25, ' ')}\t{"Data".PadRight(15, ' ')}\t{Environment.NewLine}";
        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this._writer.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        
    }


}