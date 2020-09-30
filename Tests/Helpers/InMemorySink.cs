using LogTest.Interfaces;
using System;

namespace Tests.Helpers
{
    public class InMemorySink : ILogSink  // TODO - for faster tests
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Write(string content)
        {
            throw new NotImplementedException();
        }
    }
}
