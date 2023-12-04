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
            Tcp_Server server = new Tcp_Server("127.0.0.1", 3001);
            try
            {
                server.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ErrorRRRRRR: " + ex.Message);
            }
            finally 
            {
                Console.WriteLine("Press 'q' to stop the server...");
                while (Console.ReadKey().KeyChar != 'q') { }
                server.Stop();
            }   
        }
    }
}
