using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace EchoClient
{
    internal class Program
    {
        static void Main()
        {
            while (true)
            {
                try
                {
                    using (TcpClient tcpClient = new TcpClient())
                    {
                        tcpClient.Connect("127.0.0.1", 7777);
                        Console.WriteLine("Подключено!");
                        Console.WriteLine("Введите ваше сообщение. Для выхода введите 'exit'");
                        Console.WriteLine("Для подтверждения отправки введите Enter на пустой строке.");

                        using (NetworkStream stream = tcpClient.GetStream())
                        using (StreamReader reader = new StreamReader(stream))
                        using (StreamWriter writer = new StreamWriter(stream))
                        {
                            // StreamWriter будет автоматически отправлять содержимое буфера в основной поток после Write().
                            writer.AutoFlush = true;

                            // Отдельный поток для получения эхо-ответов.
                            Thread readingThread = new Thread(() => ReadResponses(reader, tcpClient));

                            // Автоматически завершаем readingThread при завершении основного потока.
                            readingThread.IsBackground = true;
                            readingThread.Start();

                            while (tcpClient.Connected)
                            {
                                StringBuilder input = new StringBuilder();
                                string lastRead;

                                do
                                {
                                    lastRead = Console.ReadLine();

                                    if (lastRead == "exit")
                                    {
                                        Console.WriteLine("Клиент успешно остановлен.");
                                        return;
                                    }

                                    input.Append(lastRead);
                                }
                                while (lastRead != "" && tcpClient.Connected);

                                if (input.Length > 0 && tcpClient.Connected)
                                {
                                    writer.WriteLine(input.ToString());
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка: " + ex.ToString());
                    Console.WriteLine("Подключение разорвано... Введите 'retry' для повтора");

                    if (Console.ReadLine().Equals("retry"))
                    {
                        continue;
                    }
                    else
                    {
                        Console.WriteLine("Клиент успешно остановлен.");
                        return;
                    }
                }
            }
        }

        static void ReadResponses(StreamReader reader, TcpClient tcpClient)
        {
            while (tcpClient.Connected)
            {
                if (reader.BaseStream is NetworkStream stream && stream.DataAvailable)
                {
                    string result = reader.ReadLine();
                    if (result != null)
                    {
                        Console.WriteLine(result);
                    }
                }
                else
                {
                    Thread.Sleep(50);
                }
            }
        }
    }
}