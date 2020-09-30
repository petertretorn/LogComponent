using System;

namespace LogTest.Interfaces
{
    public interface ILogSink : IDisposable
    {
        void Write(string content);
    }
}