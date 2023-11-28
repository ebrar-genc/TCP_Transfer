using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Net;
using static System.Formats.Asn1.AsnWriter;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Security;

namespace TcpCommunication
{
    /// <summary>
    /// Entry point for the TCP client application.
    /// </summary>
    internal class Program
    {
        static void Main()
        {
            char flag;
            string num;
            string data;

            Console.WriteLine("Please select what you want to send to the server.\r\n1: string\r\n2: file");
            num = Console.ReadLine();
            InputIsNullOrEmpty(num);

            // create loop ***

            /// set parameters
            if (num == "1")
            {
                data = GetData("string");
                InputIsNullOrEmpty(data);
                flag = 's';

            }
            else if (num == "2")
            {
                data = GetData("file path");
                InputIsNullOrEmpty(data);
                flag = 'd';

            }
            else
            {
                Console.WriteLine("You made an invalid keystroke. Press Enter to exit...");
                Console.ReadLine();
                return;
            }
            // create a loop *****
           
            Tcp_Client client = new Tcp_Client("127.0.0.1", 3001);
            try
            {
                // dont have a return value. Is there a need for a try-catch structure? *****
                client.SendData(flag, data);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
          
            static void InputIsNullOrEmpty(string data)
            {
                if (string.IsNullOrEmpty(data))
                {
                    // ask for try again or return *****
                    Console.WriteLine("You made an invalid data. Press Enter to exit...");
                    Console.ReadLine();
                    return;
                }
            }

            static string GetData(string content)
            {
                Console.WriteLine("Enter the {0} you want to send to the server.", content);
                string data = Console.ReadLine();
                InputIsNullOrEmpty(data);
                return data;
            }
        }

        
    }
}

/*
 * Determine which client you are************> create class 
 * 
*/