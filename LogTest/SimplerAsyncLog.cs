using LogTest.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogTest
{
    public class SimplerAsyncLog : ILog, IDisposable
    {
        private BlockingCollection<LogLine> _lines = new BlockingCollection<LogLine>(new ConcurrentQueue<LogLine>());

        private ILogSink _sink;

        private Task _mainLoop;
        private CancellationTokenSource _tokenSource;

        private bool _flush = false;

        public SimplerAsyncLog() : this(new FileSink()) { }

        public SimplerAsyncLog(ILogSink sink)
        {
            this._sink = sink;

            this._tokenSource = new CancellationTokenSource();
            this._mainLoop = new Task(() => this.MainLoop(this._tokenSource.Token), this._tokenSource.Token);

            this._mainLoop.Start();
        }

        private void MainLoop(CancellationToken token)
        {
            while (true)
            {
                try
                {
                    var isDoneFlushing = (_flush && _lines.Count == 0);

                    if (token.IsCancellationRequested || isDoneFlushing) return;

                    if ( _lines.TryTake( out LogLine logLine, TimeSpan.FromMilliseconds(50) ) )
                    {
                        var line = CreateLogLine(logLine);
                        _sink.Write(line);
                    }
                }

                catch (Exception swallow) { }
            }
        }

        private string CreateLogLine(LogLine line) =>
            $"{line.Timestamp.ToString("yyyy - MM - dd HH: mm: ss: fff")}\t{line.LineText()}\t{Environment.NewLine}";

        public void StopWithFlush()
        {
            _lines.CompleteAdding();
            _flush = true;
            _mainLoop.Wait();
        }

        public void StopWithoutFlush()
        {
            _lines.CompleteAdding();
            _tokenSource.Cancel();
        }

        public void Write(string text)
        {
            if (_lines.IsAddingCompleted) throw new InvalidOperationException("Cannot Write to Closed Log");
            
            try
            {
                _lines.TryAdd(new LogLine() { Text = text, Timestamp = DateTime.Now });
            }
            catch (Exception swallow) { }
        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this._sink.Dispose();
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
