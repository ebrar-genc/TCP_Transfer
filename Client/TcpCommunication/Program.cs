using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Net;
using static System.Formats.Asn1.AsnWriter;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Security;
using Tcp_Client;
using System.Diagnostics;
using System.Net.Http;


class Program
{
    #region Main

    /// <summary>
    /// Entry point for the TCP client application.
    static void Main()
    {
        string serverIp = null;
        int port = 3001;

        Console.WriteLine("Enter the IP of the server to connect to: ");
        serverIp = Console.ReadLine();
        TcpDataTransfer client = new TcpDataTransfer(serverIp, port);
        InputAnalysis inputAnalysis = new InputAnalysis();

        Console.WriteLine("Welcome to TcpClient.");
        Console.WriteLine("Enter the information you want to transmit to the server (or 'disconnect!' to exit..): ");

        try
        {
            while (true)
            {
                string userInput = Console.ReadLine();

                if (userInput == "disconnect!")
                {
                    client.Disconnect();
                    Console.WriteLine("Do you want to connect again? (yes/no)");
                    string reconnectInput = Console.ReadLine();

                    if (reconnectInput == "no")
                    {
                        Console.WriteLine("Client was closed..");
                        break;
                    }
                    else if (reconnectInput == "yes")
                        serverIp = null;
                }
                else
                {
                    byte[] data = inputAnalysis.Analysis(userInput);
                    client.SendBytes(data);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Main Error: " + ex.Message);
        }
        finally
        {
            client.Disconnect();
        }
    }



    #endregion
}