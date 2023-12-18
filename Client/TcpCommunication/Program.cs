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


class Program
{
    #region Main

    /// <summary>
    /// Entry point for the TCP client application.
    /// </summary>
    static void Main()
    {
        string ip;
        int port = 3001;
        int flag = 0; // Flag to indicate whether the client needs to reconnect.

        Console.WriteLine("Welcome to TcpClient.");
        Console.WriteLine("Enter the IP of the server to connect to");
        //ip = Console.ReadLine();

        TcpDataTransfer client = new TcpDataTransfer("127.0.0.1", port);
        InputAnalysis inputAnalysis = new InputAnalysis();
        Console.WriteLine("Enter the information you want to transmit to the server or enter 'disconnect!' to exit.");

        try
        {
            while (true)
            {
                if (flag == 1)
                {
<<<<<<< HEAD
                    Console.WriteLine("Enter the IP of the server to connect to");
                    ip = Console.ReadLine();
=======
                    //Console.WriteLine("Enter the IP of the server to connect to");
                    //ip = Console.ReadLine();
>>>>>>> b598ca77e8898fb6ab6e7071e0ec62bd4d9337cc
                    client = new TcpDataTransfer("127.0.0.1", port);
                }
                string input = Console.ReadLine();
                if (input == "disconnect!")
                {
                    client.Disconnect();
                    Console.WriteLine("Disconnected. Do you want to connect again? (yes/no)");
                    string reconnectInput = Console.ReadLine();
                    if (reconnectInput == "yes")
                    {
                        flag = 1;
                    }
                    else if (reconnectInput == "no") 
                    {
                        Console.WriteLine("Client was closed..");
                        break;
                    }
                        
                }
                else
                {
                    byte[] data = inputAnalysis.Analysis(input);
<<<<<<< HEAD
                    client.Connect();

                    client.SendBytes(data);
=======


                    client.Connect(data);
>>>>>>> b598ca77e8898fb6ab6e7071e0ec62bd4d9337cc
                    if (flag == 1)
                        flag = 0;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Main Error: " + ex.Message);
        }
        Console.ReadLine();
    }
    #endregion
}