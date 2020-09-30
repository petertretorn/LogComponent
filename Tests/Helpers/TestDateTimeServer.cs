using LogTest.Interfaces;
using System;

namespace Tests.Helpers
{
    public class TestDateTimeServer : IDateTimeServer
    {

        private Func<DateTime> _dateFn;
        public TestDateTimeServer(DateTime time)
        {
            _dateFn = () => time;
        }
        public DateTime Now => _dateFn();

        public void ChangeTestTime(Func<DateTime> fn)
        {
            this._dateFn = fn;
        }

    }
}
