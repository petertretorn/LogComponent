using LogTest.Interfaces;
using System;

namespace LogTest
{

    public class DateTimeServer : IDateTimeServer
    {
        public DateTime Now => DateTime.Now;
    }
}
