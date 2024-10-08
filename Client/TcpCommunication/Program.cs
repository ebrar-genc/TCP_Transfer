﻿using Tcp_Client;



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
        int flag = 0;

        Console.WriteLine("Welcome to TcpClient.");
        Console.WriteLine("Enter the IP of the server to connect to");
        ip = Console.ReadLine();

        TcpDataTransfer client = new TcpDataTransfer(ip, port);
        InputAnalysis inputAnalysis = new InputAnalysis();

        try
        {
            Console.WriteLine("Enter the information you want to transmit to the server or enter 'disconnect!' to exit.");
            while (true)
            {
                if (flag == 1)
                {
                    Console.WriteLine("Enter the IP of the server to connect to");
                    ip = Console.ReadLine();
                    client = new TcpDataTransfer(ip, port);
                }
                string input = Console.ReadLine();
                if (input == "disconnect!")
                {
                    client.Disconnect();
                    Console.WriteLine("Disconnected. Do you want to connect again? (yes/no)");
                    string reconnectInput = Console.ReadLine();
                    if (reconnectInput == "yes")
                    {
                        byte[] data = inputAnalysis.Analysis(reconnectInput);
                        client.Connect(data);
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
                    client.Connect(data);
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