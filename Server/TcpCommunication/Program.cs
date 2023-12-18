using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace TcpCommunication
{
    /// <summary>
    /// Entry point for the TCP server application.
    /// </summary>
    internal class Program
    {   
        static void Main()
        {
            string serverIp = GetServerIp();
            Tcp_Server server = new Tcp_Server(serverIp, 3001);

            try
            {
                server.Start();

                Console.WriteLine("Press 'Esc' to stop the server...");

                while (true)
                {
                    if (Console.KeyAvailable)
                    {
                        ConsoleKeyInfo key = Console.ReadKey(intercept: true);

                        if (key.Key == ConsoleKey.Escape)
                        {
                            Console.WriteLine("Stopping the server...");
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while starting the server. " + ex.Message);
            }
            finally
            {
                server.Stop();
            }   
        }

        static string GetServerIp()
        {
            try
            {
                IPAddress localAddr = null;
                var host = Dns.GetHostEntry(Dns.GetHostName());

                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        localAddr = ip;
                        break;
                    }
                }

                return localAddr?.ToString();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Failed to get IPv4 address:" + e.ToString());
                return null;
            }
        }
    }
}
