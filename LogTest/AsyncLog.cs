namespace LogTest
{
    using LogTest.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class AsyncLog : ILog, IDisposable
    {
        private List<LogLine> _lines = new List<LogLine>();
        private ILogSink _sink;

        private Task _mainLoop;
        private CancellationTokenSource _tokenSource;

        private bool _isFlushingUp = false;
        private bool _skipUnderLoad;

        public AsyncLog() : this(new FileSink()) { }

        public AsyncLog(ILogSink sink, bool SkipUnderLoad = false)
        {
            this._sink = sink;
            this._skipUnderLoad = SkipUnderLoad;

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
                    var hasFlushedAll = this._isFlushingUp && this._lines.Count == 0;

                    if (token.IsCancellationRequested || hasFlushedAll) return;
                    if (this._lines.Count == 0) continue;

                    var shallowSnapshot = new List<LogLine>(this._lines);
                    var batchSize = 5;

                    List<LogLine> batch = GetLinesBatch(shallowSnapshot, batchSize);

                    batch.ForEach(line => HandleLine(line, token));

                    if (this._skipUnderLoad) SkipUnderLoad(shallowSnapshot, batch);

                    this._lines = this._lines.Where(line => line.Status == LineStatus.Pristine).ToList(); // purge lines

                    Thread.Sleep(50);
                }

                catch (OperationCanceledException opEx) { return; }

                catch (Exception ignore) { }
            }
        }

        private List<LogLine> GetLinesBatch(List<LogLine> shallowSnapshot, int batchSize) =>
            shallowSnapshot
                .Where(l => l.Status == LineStatus.Pristine)
                .Take(batchSize)
                .ToList();

        private void HandleLine(LogLine line, CancellationToken token)
        {
            if (token.IsCancellationRequested) token.ThrowIfCancellationRequested();

            string logLine = CreateLogLine(line);
            this._sink.Write(logLine);

            line.Status = LineStatus.Handled;
        }

        private string CreateLogLine(LogLine line) =>
            $"{line.Timestamp.ToString("yyyy - MM - dd HH: mm: ss: fff")}\t{line.LineText()}\t{Environment.NewLine}";

        private void SkipUnderLoad(List<LogLine> shallowSnapshot, List<LogLine> batch)
        {
            var skipped = shallowSnapshot.Except(batch).ToList();

            skipped.ForEach(line => line.Status = LineStatus.Skipped);
        }

        public void StopWithoutFlush()
        {
            this._tokenSource.Cancel();
        }

        public void StopWithFlush()
        {
            this._isFlushingUp = true;
            this._mainLoop.Wait();
        }

        public void Write(string text)
        {
            this._lines.Add(new LogLine() { Text = text, Timestamp = DateTime.Now, Status = LineStatus.Pristine });
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