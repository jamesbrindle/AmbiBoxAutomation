using System;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;

namespace PrismatikApiCaller
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                var Rw = new TelnetReaderWriter();

                string command = args[0];
                var client = new TcpClient();
                try
                {
                    int retryCount = 120; // 2 minutes (accounts for computer booting up)
                    int i = 0;
                    while (true)
                    {
                        try
                        {
                            client.Connect("localhost", 3636);
                            break;
                        }
                        catch (Exception)
                        {
                            if (i < retryCount)
                                Thread.Sleep(1000);
                            else
                                throw;

                            i++;
                        }
                    }

                    NetworkStream ns = client.GetStream();

                    Rw.Write(ns, "lock");
                    string reply = Rw.Read(ns);
                    if (!reply.ToLower().Contains("lock:success"))
                    {
                        ConsoleWrite(ConsoleColor.Red, "Warning: " + reply);
                        return;
                    }

                    if (command.ToLower().StartsWith("script:"))
                    {
                        try
                        {
                            typeof(Scripts).GetMethod(Regex.Replace(command, "script:", "", RegexOptions.IgnoreCase))
                                           .Invoke(new Scripts(), new object[] { ns, Rw });
                        }
                        catch (Exception e)
                        {
                            ConsoleWrite(ConsoleColor.Red, "Error: " + e.Message);
                        }
                    }
                    else
                        Rw.Write(ns, command);

                    reply = Rw.Read(ns);
                    Console.WriteLine(reply);

                    Rw.Write(ns, "unlock");
                    Rw.Write(ns, "exit");

                    ns.Close();
                    client.Close();

                }
                catch (Exception e)
                {
                    ConsoleWrite(ConsoleColor.Red, "Error: " + e.Message);
                }
            }
            else
                ConsoleWrite(ConsoleColor.Red, "No arguments supplied. I.e. 'setcolor:Colour - Red'");
        }

        private static void ConsoleWrite(ConsoleColor colour, string text)
        {
            Console.ForegroundColor = colour;
            Console.WriteLine(text);
            Console.ResetColor();
        }
    }
}
