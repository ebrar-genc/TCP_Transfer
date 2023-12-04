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
        int flag = 0;


        Console.WriteLine("Welcome to TcpClient.");
        Console.WriteLine("Enter the IP of the server to connect to");
        //ip = Console.ReadLine();

        TcpDataTransfer client = new TcpDataTransfer("127.0.0.1", port);
        InputCheckAndSend data = new InputCheckAndSend();
        
        
        while (true)
        {
            Console.WriteLine("Enter the information you want to transmit to the server or enter 'disconnect!' to exit.");
            string input = Console.ReadLine();
            if (input == "disconnect")
                break;
            data.IsPath(input, client);
        }

    }
    #endregion
}





