using System;
using System.Threading;
using System.Threading.Tasks;


namespace ThreadingText
{
    class Program
    {
        public static int enter = 0;
        static AutoResetEvent are = new AutoResetEvent(false);

        static void Main(string[] args)
        {
            Thread printingNums = new Thread(printNums);
            printingNums.Start();


            while (true)
            {
                enter = Convert.ToInt32(Console.ReadLine());
                if (enter == 2)
                    are.Set();
            }
        }

        static void printNums()
        {
            int i = 0;
            while (i <= 100)
            {
                if (enter == 1)
                    are.WaitOne();
                if (enter != 1)
                {
                    int cl = Console.CursorLeft;
                    int ct = Console.CursorTop;

                    Console.SetCursorPosition(0, 10);
                    Console.WriteLine(i);
                    Console.SetCursorPosition(cl, ct);

                    System.Threading.Thread.Sleep(1000);
                    i++;
                }
            }

            /*for(int i = 0; i < 100; i++)
            {
                if (enter == 2)
                    are.WaitOne();
                if (enter == 1)
                {
                    int cl = Console.CursorLeft;
                    int ct = Console.CursorTop;

                    Console.SetCursorPosition(0, 10);
                    Console.WriteLine(i);
                    Console.SetCursorPosition(cl, ct);

                    System.Threading.Thread.Sleep(1000);
                    Console.WriteLine(i);
                }
            }*/

        }
    }
}
