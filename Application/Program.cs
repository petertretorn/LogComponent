using System;

namespace LogUsers
{
    using System.Threading;
    using System.Threading.Tasks;
    using LogTest;
    using LogTest.Interfaces;

    class Program
    {
        static void Main(string[] args)
        {
            ILog logger = new SimplerAsyncLog();

            for (int i = 0; i < 15; i++)
            {
                logger.Write("Number with Flush: " + i.ToString());
                Thread.Sleep(50);
            }

            logger.StopWithFlush();

            ILog logger2 = new SimplerAsyncLog();

            Task.Run( () => {
                for (int i = 50; i > 0; i--)
                {
                    logger2.Write("Number with No flush: " + i.ToString());
                    Thread.Sleep(5);
                }
            });

            Thread.Sleep(20);

            logger2.StopWithoutFlush();

            Console.ReadLine();
        }
    }
}
