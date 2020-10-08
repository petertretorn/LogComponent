using LogTest.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogTest
{
    public class DailyRotationRule : ILogRotationRule
    {
        public bool Evaluate(DateTime actual, DateTime registered) => actual.Date != registered.Date;
    }
}
