﻿using System;
using System.Text;
using System.IO;


namespace fileSearcher
{
    class Program
    {
        public const string searchConfigPath = "./searchConfig.txt";

        static void Main(string[] args)
        {
            checkConfigFileExists();
            startMenu();

            Console.ReadKey();

        }


        static void checkConfigFileExists()
        {
            if (!File.Exists(searchConfigPath))
            {
                StreamWriter writer = new StreamWriter(searchConfigPath, true, Encoding.UTF8);
                writer.Close();
            }
        }



        static void startMenu()
        {
            while (true)
            {
                //Отрисовка элементов меню
                printText("FileSearcher", "center");
                printText(" ");
                printSearchParam();
                printText(" ");
                printText(" 1 - начать поиск");
                printText(" 2 - изменить параметры поиска");
                printText(" 3 - выйти из программы");
                Console.SetCursorPosition(2, Console.CursorTop + 1);
                

                //Вызов функций, соответствующих выбору пользователя
                try
                {
                    int enter = Convert.ToInt32(Console.ReadLine());

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
                            printIncorrectInputMessage();
                            break;
                    }
                }
                catch (System.FormatException)
                {
                    printIncorrectInputMessage();
                    Console.Clear();
                }
            }
            
        }
        static void printSearchParam()
        {
            //Открытие файла конфигурации поиска
            FileStream input = new FileStream(searchConfigPath, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(input);


            printText(" Стартовая директория поиска: ");
            //Отображение информации из файла конфигурации о стартовой директории поиска
            Console.SetCursorPosition(31, Console.CursorTop - 1);
            Console.Write(reader.ReadLine());

            printText("\n  Имя искомого файла: ");
            //Отображение информации из файла конфигурации об имени файла
            Console.SetCursorPosition(22, Console.CursorTop - 0); Console.Write(reader.ReadLine());
            Console.SetCursorPosition(1, Console.CursorTop + 1);

            reader.Close();
        }

        //Вывод на экран надписи о неверном вводе данных и таймер обатного отсчета на 5 секунд
        static void printIncorrectInputMessage()
        {
            Console.Clear();
            printText("Неправильно введено значение!", "center");
            printText("Вы будете направлены назад", "center");
            timerForSec(5);
            Console.Clear();
        }

        //Таймер с обратным отсчетом, замораживающий процесс на заданное кол-во секунд
        static void timerForSec(int sec)
        {
            for (int i = sec; i >= 1; i--)
            {
                printText(Convert.ToString(i), "center");
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                System.Threading.Thread.Sleep(1000);
            }
        }


        static void search()
        {

        }


        static void correctSearchParam()
        {
            bool exit = false;
            while (exit == false)
            {
                printText("FileSearcher", "center");
                printText(" ");
                printSearchParam();
                printText(" ");
                printText("Новая стартовая директория поиска: ");
                Console.SetCursorPosition(2, Console.CursorTop);
                string newStartDirectory = Console.ReadLine();
                printText(" Новое имя искомого файла: ");
                Console.SetCursorPosition(2, Console.CursorTop);
                string newSearchingFileName = Console.ReadLine();

                printText(" ");
                printText(" 1 - Сохранить изменения     2 - Выйти без изменений");
                Console.SetCursorPosition(2, Console.CursorTop);


                try
                {
                    int enter = Convert.ToInt32(Console.ReadLine());

                    switch (enter)
                    {
                        case 1:
                            Console.Clear();
                            saveToConfigNewParam(newStartDirectory, newSearchingFileName);
                            break;
                        case 2:
                            Console.Clear();
                            exit = true;
                            break;
                        default:
                            printIncorrectInputMessage();
                            break;
                    }

                }
                catch (System.FormatException)
                {
                    printIncorrectInputMessage();
                }
            }
            
        }

       static void saveToConfigNewParam(string newStartDir, string newSearchFileName)
       {
            string tempfile = Path.GetTempFileName();
            StreamWriter writer = new StreamWriter(tempfile, false);

            writer.WriteLine(newStartDir);
            writer.WriteLine(newSearchFileName);

            writer.Close();
            File.Delete(searchConfigPath);
            File.Move(tempfile, searchConfigPath);
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
