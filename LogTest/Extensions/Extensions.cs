using LogTest.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogTest.Extensions
{
    public static class Extensions
    {
        public static string LogPath(this DateTime dt)
        {
            return $@"C:\LogTest\Log{dt.ToString("yyyyMMdd HHmmss fff")}.log";
        }
    }
}
