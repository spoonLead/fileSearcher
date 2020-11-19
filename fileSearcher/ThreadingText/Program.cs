using System;
using System.Threading;
using System.Threading.Tasks;


namespace ThreadingText
{
    class Program
    {
        public static int enter = 0;

        static void Main(string[] args)
        {
            //Thread printingNums = new Thread(printNums);
            // ManualResetEvent me = new ManualResetEvent(true);
            //printingNums.Start(me);

            printNums();

            while (true)
            {
                enter = Convert.ToInt32(Console.ReadLine());
                
            }
        }

        async static void printNums()
        {
            int i = 0;
            while (i <= 100)
            {
                if (enter == 1)
                    await Task.Delay(1);
                if(enter != 1)
                {
                    int cl = Console.CursorLeft;
                    int ct = Console.CursorTop;

                    Console.SetCursorPosition(0, 10);
                    Console.WriteLine(i);
                    Console.SetCursorPosition(cl, ct);

                    await Task.Delay(1000);
                    i++;
                }
            }
            
        }
    }
}
