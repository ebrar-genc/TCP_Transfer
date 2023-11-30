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
    class Program
    {
        #region Main

        /// <summary>
        /// Entry point for the TCP client application.
        /// </summary>
        static void Main()
        {
            Console.WriteLine("Enter string to send message to server or paste the path for file transfer...");
            TcpDataTransfer client = new TcpDataTransfer("127.0.0.1", 3001);

            while (true)
            {
                string data = Console.ReadLine();

                if (data == "q")
                    break;
                try
                {
                    client.Connect();
                    InputCheckAndSend(data, client);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
            client.Disconnect();
        }
        #endregion

        #region Check and send input

        /// <summary>
        /// Valid File path and File found -> IsValidFileExtension(data);
        /// </summary>
        static bool IsPath(string data)
        {
            try
            {
                if (Path.IsPathRooted(data))
                {
                    if (File.Exists(data))
                    {
                        if (IsValidFileExtension(data))
                            return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return false;
        }

        static bool IsValidFileExtension(string data)
        {
            string[] allowedExtensions = { ".txt", ".jpg", ".png", ".pdf", ".zip" };

            string fileExtension = Path.GetExtension(data);

            foreach (string allowedExtension in allowedExtensions)
            {
                if (string.Equals(fileExtension, allowedExtension, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Check whether it is string or file
        /// </summary>
        static void InputCheckAndSend(string data, TcpDataTransfer client)
        {
            try
            {
                if (IsPath(data))
                {
                    Console.WriteLine("Starting file transfer...");
                    client.SendFile(data);
                }
                else
                {
                    Console.WriteLine("Sending string...");
                    client.SendString(data);
                }
                Console.WriteLine("Sent!!!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        #endregion

    }
}








/*
 * Determine which client you are************> create class 
 * 
*/


//classları ayırdım
//string de olsssnn dıye