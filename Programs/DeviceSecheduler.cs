using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole.Programs
{
    internal class DeviceSecheduler
    {
        private static Timer _timer;

        public static void Run()
        {
            _timer = new Timer(WriteHelloWorld, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));

            Console.ReadLine();
        }
        private static void WriteHelloWorld(Object o)
        {
            // This method is called by the timer every 10 seconds
            Console.WriteLine("Hello, World!");
        }

    }
}
