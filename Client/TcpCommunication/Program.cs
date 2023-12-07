using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Net;
using static System.Formats.Asn1.AsnWriter;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Security;


class Program
{
    #region Main

    /// <summary>
    /// Entry point for the TCP client application.
    /// </summary>
    static void Main()
    {
        //string ip;
        int port = 3001;

        Console.WriteLine("Welcome to TcpClient.");
        Console.WriteLine("Enter the IP of the server to connect to");
        //ip = Console.ReadLine();

        try
        {
            while (true)
            {
                TcpDataTransfer client = new TcpDataTransfer("192.168.1.112", port);
                InputCheckAndSend data = new InputCheckAndSend(client);

                Console.WriteLine("Enter the information you want to transmit to the server or enter 'disconnect!' to exit.");
                string input = Console.ReadLine();
                if (input == "disconnect!")
                {
                    client.Disconnect();
                    Console.WriteLine("Disconnected. Do you want to connect again? (y/n)");
                    string reconnectInput = Console.ReadLine();
                    if (reconnectInput != "y")
                    {
                        break;
                    }
                }
                else
                {
                    data.IsPath(input);
                    client.Disconnect();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Main Error: " + ex.Message);
        }

    }
    #endregion
}





