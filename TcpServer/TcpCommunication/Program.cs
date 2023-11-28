using System.Diagnostics;
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
            Debug.WriteLine("Hello Server Main!");
            Tcp_Server server = new Tcp_Server("127.0.0.1", 3001);
            try
            {
                server.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                server.Stop();
            }
            Console.WriteLine("The program is over. Press enter to exit");
            Console.ReadLine();
        }
    }
}
