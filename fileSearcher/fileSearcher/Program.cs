using System;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Diagnostics;

namespace fileSearcher
{
    class Program
    {
        public const string searchConfigPath = "./searchConfig.txt";


        static AutoResetEvent are = new AutoResetEvent(false);
        static int currentFileNumber = 0;
        static int fileCount = 0;
        static bool searchingPause = false;
        static Stopwatch watch = new Stopwatch();
        static long elapsedSearchTime = 0;
        static bool endSearch = false;
        static List<string> requiredFiles = new List<string>();

        static string startDir;
        static string searchFileName;
        static Regex searchFileNamePattern;

        static void Main(string[] args)
        {
            //Проверка существования конфигурационного файла и его создание в случае отсуствия
            if (!File.Exists(searchConfigPath))
                makeConfigFile();

            startMenu();

            Console.ReadKey();
        }

        static void makeConfigFile()
        {
                StreamWriter writer = new StreamWriter(searchConfigPath, true, Encoding.UTF8);
                writer.Close();
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
                printText("1 - начать поиск");
                printText("2 - изменить параметры поиска");
                printText("3 - выйти из программы");
                printText(" ");
                printText("Выбор пункта меню: ");
                Console.SetCursorPosition(21, Console.CursorTop -1);
                


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
                            printIncorrectInputMessageForSec();
                            break;
                    }
                }
                catch (System.FormatException)
                {
                    printIncorrectInputMessageForSec();
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
        static void printIncorrectInputMessageForSec(int sec = 5)
        {
            Console.Clear();
            printText("Неправильно введено значение!", "center");
            printText("Вы будете направлены назад", "center");
            freezeTimerForSec(sec);
            Console.Clear();
        }

        //Таймер с обратным отсчетом, замораживающий процесс на заданное кол-во секунд
        static void freezeTimerForSec(int sec)
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
            //Открытие файла конфигурации поиска
            FileStream input = new FileStream(searchConfigPath, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(input);
            
            //Получение из файла параметров поиска
            startDir = reader.ReadLine();
            searchFileName = reader.ReadLine();
            searchFileNamePattern = new Regex(@"(\w*)" + searchFileName + @"(\w*)");

            //Закрытие файла конфигурации
            reader.Close();

            //Отрисовка элементов меню
            printText("FileSearcher", "center");
            printText(" ");
            printSearchParam();
            printText(" ");
            printText("Введите номер файла после завершения, чтобы открыть его в блокноте или");
            printText("s - остановить   w - продолжить поиск   0 - выйти в меню: ");
            printText(" ");
            printText(" ");

            Console.SetCursorPosition(2, 9);
            Console.Write("Общее кол-во файлов: ");
            Console.SetCursorPosition(2, 10);
            Console.Write("Кол-во совпавших файлов: ");
            Console.SetCursorPosition(2, 11);
            Console.Write("Кол-во секунд с начала поиска: ");
            Console.SetCursorPosition(2, 12);
            Console.Write("Поиск в директории: ");
            Console.SetCursorPosition(60, 6);
            


            Thread searchingThread = new Thread(searchingThr);
            searchingThread.Start();

            //TODO: Refactor
            if(endSearch == true)
            {
                are.Set();
                endSearch = false;
            }
            //TODO_END


            //Обработка пользовательского ввода во врмемя поиска
            while (true)
            {
                char enter = '1';
                Console.SetCursorPosition(60, 6);

                //Пользовательский ввод во время поиска
                if (!endSearch)
                    enter = Console.ReadKey().KeyChar;
                else
                    break;
                

                
                if (enter == 'w')   //Продолжить поиск
                {
                    searchingPause = false;
                    are.Set();      //Посылание сигнала процессу поиска
                }
                if (enter == 's')   //Приостановить поиск
                {
                    searchingPause = true;
                }
                if (enter == '0')   //Завершить поиск
                {
                    endSearch = true;
                    while (searchingThread.IsAlive) { }   //Ожидание завершения работы поискового потока
                    break;
                }
            }



            //Обработка пользовательского ввода по завершению поиска
            while (true)
            {
                try
                {
                    //Ввод номера файла для открытия
                    Console.SetCursorPosition(60, 6);
                    int fileNumber = Convert.ToInt32(Console.ReadLine());
                    

                    //Открытие файла из списка по введенному номеру
                    //и выход из меню поиска при введении 0
                    if (fileNumber != 0)
                    {
                        try
                        {
                            System.Diagnostics.Process.Start("C:\\Windows\\System32\\notepad.exe", requiredFiles[fileNumber - 1]);
                        }
                        catch(System.ArgumentOutOfRangeException)    //Стирание введенного пользователем значения в случае выхода за границы списка
                        {
                            Console.SetCursorPosition(60, 6);
                            Console.Write("                                                            ");      //TODO: Стирание всей строки
                            Console.SetCursorPosition(60, 6);
                        }
                    }
                    else
                        break;
                }
                catch (System.FormatException)              //Стирание введенного пользователем значения в случае введения нечислового значения
                {
                    Console.SetCursorPosition(60, 6);
                    Console.Write("                                                            ");      //TODO: Стирание всей строки
                    Console.SetCursorPosition(60, 6);
                }
            }


            resetSearchParam();
            Console.Clear();
        }

        public static void resetSearchParam()
        {
            currentFileNumber = 0;
            fileCount = 0;
            searchingPause = false;
            elapsedSearchTime = 0;
            requiredFiles.Clear();
            watch.Reset();
        }



        public static void searchingThr()
        {
            requiredFiles = getRecursiveFilesMatchPattern(startDir, searchFileNamePattern);
            Console.WriteLine(" ");
            Console.WriteLine("  Поиск завершен - нажмите любую клавишу перед вводом значений");
            Console.SetCursorPosition(60, 6);
            endSearch = true;
        }

        public static List<string> getRecursiveFilesMatchPattern(string dir, Regex pattern)
        {
            watch.Start(); //Старт таймера
            
            List<string> recursiveFilesMatchPattern = new List<string>();

            //Возврат пустых значений в случае принудительного завершения поиска
            if (endSearch == true)
            {
                return recursiveFilesMatchPattern;
            }


            try
            {
                //Рекурсивный проход по директориям
                string[] dirs = Directory.GetDirectories(dir);
                int d = 0;
                while(d < dirs.Length)
                {
                    //Установка потока в режим ожидания сигнала в случае паузы
                    if (searchingPause == true)
                        are.WaitOne();

                    recursiveFilesMatchPattern.AddRange(getRecursiveFilesMatchPattern(dirs[d], pattern));
                    d++;
                }
                
                //Проход по файлам внутри директории
                string[] files = Directory.GetFiles(dir);
                int f = 0;
                while (f < files.Length)
                {
                    //Установка потока в режим ожидания сигнала в случае паузы
                    if (searchingPause == true)
                        are.WaitOne();

                    //Проверка файла на соответсвие паттерну
                    if (pattern.IsMatch(files[f]))
                    {
                        recursiveFilesMatchPattern.Add(files[f]);
                        currentFileNumber++;
                        Console.SetCursorPosition(2, 14 + currentFileNumber);
                        Console.Write(currentFileNumber + ") - " + files[f]);
                        Console.SetCursorPosition(62, 6);
                    }


                    //Вывод общего количества проверенных файлов и количества соответсвующих паттерну файлов
                    fileCount++;
                    Console.SetCursorPosition(22, 9);
                    Console.Write(fileCount);
                    Console.SetCursorPosition(60, 6);
                    Console.SetCursorPosition(26, 10);
                    Console.Write(currentFileNumber);
                    Console.SetCursorPosition(60, 6);
                    f++;
                }
                


                //TODO: вынести в отдельный метод
                //Очищение строк вывода диеректории, в которой производится поиск
                Console.SetCursorPosition(22, 12);
                for (int i = 22; i < Console.BufferWidth; i++)
                {
                    Console.Write(" ");
                }
                Console.SetCursorPosition(0, 13);
                for (int i = 0; i < Console.BufferWidth; i++)
                {
                    Console.Write(" ");
                }
                //Вывод директории, в которой производится поиск
                Console.SetCursorPosition(22, 12);
                Console.Write(dir);
                Console.SetCursorPosition(60, 6);
                Thread.Sleep(30);



                //Подсчет и вывод времени с начала выполнения поиска 
                watch.Stop();
                elapsedSearchTime = watch.ElapsedMilliseconds;
                Console.SetCursorPosition(34,11);
                Console.Write(elapsedSearchTime/1000);
                Console.SetCursorPosition(60, 6);

            }
            catch (System.Exception e) { }

            return recursiveFilesMatchPattern;
        }



        static void correctSearchParam()
        {
            bool exit = false;
            while (exit == false)
            {
                //Отрисовка интерфейса и считывание пользовательского ввода
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
                        case 1:         //При выборе сохранить внесенные изменения
                            saveToConfigNewParam(newStartDirectory, newSearchingFileName);
                            Console.Clear();
                            printText("Изменения сохранены!", "center");
                            printText("Вы будете возвращены в главное меню через:", "center");
                            freezeTimerForSec(5);
                            Console.Clear();
                            exit = true;
                            break;
                        case 2:         //При выборе выйти без изменений
                            Console.Clear();
                            exit = true;
                            break;
                        default:
                            printIncorrectInputMessageForSec();
                            break;
                    }

                }
                catch (System.FormatException)
                {
                    printIncorrectInputMessageForSec();
                }
            }
            
        }

        //Сохранение в файл конфига заданных параметров
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

        //Метод красивого отображения текста с заданием центровки
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
