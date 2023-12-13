using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.IO;
using Microsoft.VisualBasic;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net.Mime;


/// <summary>
/// Represents a TCP client for communication with a server.
/// </summary>
class TcpDataTransfer
{
    #region Parameters;

    private TcpClient Client;
    private string IpAddress;
    private int Port;
    public bool ClientActive = false;

    /// <summary>
    /// The buffer size for data transmission.
    /// </summary>
    private int Buffer;
    #endregion

    #region Public

    /// <summary>
    /// Initializes and set parameters.
    /// </summary>
    /// <param name="ipAddress">The IP address of the server.</param>
    /// <param name="port">The port number to connect.</param> 
    public TcpDataTransfer(string ipAddress, int port)
    {
        IpAddress = ipAddress;
        Port = port;
        Buffer = 1024 * 64;
        Connect();
    }

    /// <summary>
    /// Disconnects from the server and releases resources.
    /// </summary>
    public void Disconnect()
    {
        try
        {
            if (Client != null && Client.Connected)
            {
                Client.GetStream().Close(); // Close the stream first
                Client.Close();
                Console.WriteLine("Disconnected from the server.");
            }
            Client = null;
            ClientActive = false;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while disconnecting: " + ex.Message);
        }
    }
    #endregion

    #region Public Functions

    /// <summary>
    /// Sends the specified byte array to the server.
    /// </summary>
    /// <param name="finalBytes">The byte array to be sent.</param>
    public void SendBytes(byte[] finalBytes)
    {
 
        if (Client != null && Client.Connected)
        {
            try
            {
                NetworkStream stream = Client.GetStream();

                int finalLength = finalBytes.Length;
                if (finalLength <= Buffer)
                {
                    
                    stream.Write(finalBytes, 0, finalLength);
                }
                else
                {
                    // Send in chunks
                    int unsentBytes = finalLength;
                    int sentBytes = 0;

                    while (unsentBytes > 0)
                    {
                        int len = Math.Min(unsentBytes, Buffer);
                        stream.Write(finalBytes, sentBytes, len);
                        unsentBytes -= len;
                        sentBytes += len;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while sending text: " + ex.Message);
            }
        }
        else
        {
            Console.WriteLine("Client is not connected.");
        }
        string content = Encoding.UTF8.GetString(finalBytes);
        
        Debug.WriteLine("Content as UTF-8: " + content);

    }

    /// <summary>
    /// Reads the server's response.
    /// </summary>
    /// <param name="stream">The NetworkStream used for reading.</param>
    public void ReadResponse()
    {
        try
        {
            NetworkStream stream = Client.GetStream();
            byte[] responseByte = new byte[1024];
            int bytesRead = stream.Read(responseByte, 0, responseByte.Length);
            string message = Encoding.UTF8.GetString(responseByte, 0, bytesRead);
            Console.WriteLine(message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("aaAn error occurred while receiving response: " + ex.Message);
            Console.WriteLine("aaaAn error occurred while receiving response: " + ex.ToString());

        }
    }

    #endregion

    #region Private Functions

    /// <summary>
    /// Connects to the server.
    /// </summary>
    private void Connect()
    {
        try
        {
            Client = new TcpClient();
            Client.Connect(IpAddress, Port);
            ClientActive = true;
            Console.WriteLine("Connected to server");
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error connecting to the server: " + ex.Message);
        }
    }
    #endregion
}


