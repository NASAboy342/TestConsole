using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole.Programs
{
    public class ConsoleSpinner
    {
        public void Run()
        {
            Console.Write("Working....");
            while (true)
            {
                Turn();
                Thread.Sleep(100);
            }
        }

        int counter;
        public ConsoleSpinner()
        {
            counter = 0;
        }

        public void Turn()
        {
            Console.CursorVisible = false;
            counter++;
            switch (counter % 4)
            {
                case 0: Console.Write(":"); break;
                case 1: Console.Write(" :"); break;
                case 2: Console.Write("  :"); break;
                case 3: Console.Write("   :"); break;
            }
            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
        }
    }
}