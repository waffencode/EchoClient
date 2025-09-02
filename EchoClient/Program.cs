using System;
using System.Net.Sockets;
using System.Text;

namespace EchoClient
{
    internal class Program
    {
        static void Main()
        {
            using (TcpClient tcpClient = new TcpClient())
            {
                try
                {
                    tcpClient.Connect("127.0.0.1", 7777);
                    Console.WriteLine("Подключено!");
                    Console.WriteLine("Введите ваше сообщение. Для выхода введите 'exit'");
                    Console.WriteLine("Для подтверждения отправки введите Enter на пустой строке.");

                    while (true)
                    {
                        if (!tcpClient.Connected)
                        {
                            Console.WriteLine("Ошибка подключения... Введите 'retry' для повтора");
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


                        StringBuilder input = new StringBuilder();
                        string lastRead;
                        
                        do
                        {
                            lastRead = Console.ReadLine();
                            if (lastRead.Equals("exit"))
                            {
                                Console.WriteLine("Клиент успешно остановлен.");
                                return;
                            }
                            input.Append(lastRead);
                        }
                        while (lastRead != "");
                        
                        int bytesSent = tcpClient.Client.Send(Encoding.UTF8.GetBytes(input.ToString() + '\n'));
                        Console.WriteLine($"Отправлено байт: {bytesSent}");
                    }
                }
                catch
                {
                    Console.WriteLine("Подключение разорвано... Введите 'retry' для повтора");
                    if (Console.ReadLine().Equals("retry"))
                    {
                        Main();
                    }
                    else
                    {
                        Console.WriteLine("Клиент успешно остановлен.");
                        return;
                    }
                }
                finally
                {
                    tcpClient.Close();
                }
            }
        }
    }
}
