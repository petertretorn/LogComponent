using LogTest.Interfaces;
using System;
using System.IO;
using LogTest.Extensions;

namespace LogTest
{
    public class FileSink : ILogSink
    {
        private StreamWriter _writer;
        DateTime _curDate;
        protected IDateTimeServer _timeServer;

        public FileSink(IDateTimeServer timeServer)
        {
            this._timeServer = timeServer;
            this._curDate = _timeServer.Now;

            if (!Directory.Exists(@"C:\LogTest"))
                Directory.CreateDirectory(@"C:\LogTest");

            CreateNewWriter();
        }

        public FileSink() : this(new DateTimeServer()) { }

        public void Write(string content)
        {
            if (this._timeServer.Now.Date != _curDate.Date)
            {
                _curDate = _timeServer.Now;

                CreateNewWriter();
            }
            
            this._writer.Write(content);
        }

        private void CreateNewWriter()
        {
            this._writer?.Dispose();

            var path = _timeServer.Now.LogPath();
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