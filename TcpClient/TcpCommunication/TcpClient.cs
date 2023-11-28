using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpCommunication
{
    /// <summary>
    /// Represents a TCP client for communication with a server.
    /// </summary>
    internal class Tcp_Client
    {
        private TcpClient client;
        private string ipAddress;
        private int port;

        #region Constructor

        /// <summary>
        /// Initializes and set parameers.
        /// </summary>
        /// <param name="ipAddress">The IP address of the server.</param>
        /// <param name="port">The port number to connect.</param> 
        public Tcp_Client(string ipAddress, int port)
        {
            this.ipAddress = ipAddress;
            this.port = port;
            Debug.WriteLine("Hello Client Constructor");
            this.client = new TcpClient();
            Connect();
        }
        #endregion

        #region Connection - Disconnection Methods
        /// <summary>
        /// Connects to the server.
        /// </summary>
        public void Connect()
        {
            try
            {
                client.Connect(ipAddress, port);
                Debug.WriteLine("Connected to the server.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error connecting to the server: " + ex.Message);
            }
        }

        /// <summary>
        /// Disconnects from the server and releases resources.
        /// </summary>
        public void Disconnect()
        {
            if (client != null)
            {
                client.Close();
                client.Dispose();
                Console.WriteLine("Disconnected from the server.");
            }
            else
            {
                Console.WriteLine("TcpClient is already null.");
            }
        }

        #endregion

        #region Data Transfer Methods


        /// <summary>
        /// The requested data is converted into byte array and sent to the server via TCP connection.
        /// </summary>
        /// <param name="flag">Determine whether it is string or file path</param>
        /// <param name="data">The data to be sent.</param>
        public void SendData(char flag, string data)
        {
            Debug.WriteLine("Hello, SendData()!");

            // Get a client stream for reading and writing.
            NetworkStream stream = client.GetStream();

            if (flag == 's')
            {
                Debug.WriteLine("Hello string!");
                // Translate the passed message into ASCII and store it as a Byte array!!!
                byte[] strData = Encoding.ASCII.GetBytes(data);

                stream.Write(strData, 0, strData.Length);
                Console.WriteLine("Sent message: " + data);
                ReadResponse(stream);
            }
            else
            {
                //allow file ******
                // read the file and send
                byte[] fileData = File.ReadAllBytes(data);
                stream.Write(fileData, 0, fileData.Length);
                Console.WriteLine("Sent file path: " + data);
                ReadResponse(stream);
            }

        }


        /// <summary>
        /// Reads the server's response.
        /// </summary>
        /// <param name="stream">The NetworkStream used for reading.</param>
        public void ReadResponse(NetworkStream stream)
        {

            byte[] responseData = new byte[4096];
            int bytesRead = stream.Read(responseData, 0, responseData.Length);
            Console.WriteLine("geldi miii:");


            string response = Encoding.ASCII.GetString(responseData, 0, bytesRead);

            Console.WriteLine("Server Response: " + response);
            Console.WriteLine("geldiiiiiiiiii: " + response);

            Console.ReadLine();
        }

        #endregion

    }
}


/*
 * public event EventHandler<string> DataReceived ----> search it
 * task, async --> search it
*/


/*
 * /*if (Path.IsPathRooted(request))
            {
             if (File.Exists(request))
             {

             }
             else
             {
                 Console.WriteLine("Not a valid file path. Press Enter to exit...");
             }
            }*/
*/