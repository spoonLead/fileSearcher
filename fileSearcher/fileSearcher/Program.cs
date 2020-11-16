using System;

namespace fileSearcher
{
    class Program
    {
        static void Main(string[] args)
        {
            startMenu();
            Console.ReadKey();

        }

        static void startMenu()
        {
            while (true)
            {
                //Отрисовка элементов меню
                printText("FileSearcher", "center");
                printText(" ");
                printText(" Стартовая директория поиска: ");
                printText(" Имя файла: ");
                printText(" ");
                printText(" 1 - начать поиск");
                printText(" 2 - изменить параметры поиска");
                printText(" 3 - выйти из программы");
                Console.SetCursorPosition(2, Console.CursorTop + 1);
                
                int enter = 0;

                try
                {
                    enter = Convert.ToInt32(Console.ReadLine());

                    switch (enter)
                    {
                        case 1:
                            Console.Clear();
                            search();
                            break;
                        case 2:
                            Console.Clear();
                            correctSearchParam();
                            break;
                        case 3:
                            Console.Clear();
                            System.Environment.Exit(0);
                            break;
                        default:
                            break;
                    }
                }
                catch (System.FormatException)
                {
                    Console.Clear();
                }
            }
            
        }

        static void search()
        {

        }

        static void correctSearchParam()
        {

        }

        static void printText(string str, string side = "left")
        {
            int ost = Console.BufferWidth - str.Length;  //остаток символов строки с сообщением
            if (side == "center")
            {
                //задание пробелов перед сообщением, для его отображения по центру
                Console.Write(" ");
                for (int i = 0; i < ost / 2 - 1; i++)   // -1 для звёздочки в начале
                {
                    Console.Write(" ");
                }


                Console.Write($"{str}");


                //задание пробелов после сообщениея, для его отображения по центру

                if (ost % 2 == 0)
                {
                    for (int i = 0; i < ost / 2 - 1; i++)
                    {
                        Console.Write(" ");
                    }
                    Console.Write(" ");
                }
                else
                {
                    for (int i = 0; i < ost / 2; i++)
                    {
                        Console.Write(" ");
                    }
                    Console.Write(" ");
                }
            }
            else if (side == "left")
            {
                Console.Write(" ");
                Console.Write($"{str}");


                //задание пробелов после сообщениея, для отображения правой границы

                for (int i = 0; i < ost - 2; i++)
                {
                    Console.Write(" ");
                }
                Console.Write(" ");

            }
            else if (side == "right")
            {
                //задание пробелов перед сообщением, для его отображения по центру
                Console.Write(" ");
                Console.SetCursorPosition(Console.BufferWidth - 1 - str.Length, Console.CursorTop);
                Console.Write($"{str} ");
            }
        }
    }
}
