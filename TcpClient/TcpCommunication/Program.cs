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

            Console.WriteLine("Enter string to send message to server or paste the path for file transfer..");
            string data = Console.ReadLine();

            TcpDataTransfer client = new TcpDataTransfer("127.0.0.1", 3001);

            if (IsPath(data))
            {
                Console.WriteLine("Starting file transfer...");
                try
                {
                    client.SendFile(data);
                    data = Console.ReadLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
            else
            {
                try
                {
                    client.SendString(data);
                    data = Console.ReadLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }

            Console.WriteLine("The program is over. Press enter to exit");
            Console.ReadLine();
        }
        #endregion

        #region File Path Control
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
        #endregion

    }
}








/*
 * Determine which client you are************> create class 
 * 
*/


//classları ayırdım
//string de olsssnn dıye