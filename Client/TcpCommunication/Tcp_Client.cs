﻿using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;



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
    }

    /// <summary>
    /// Connects to the server.
    /// </summary>
    public void Connect(byte[] data)
    {
        try
        {
            Client = new TcpClient();
            Client.Connect(IpAddress, Port);
            ClientActive = true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error connecting to the server: " + ex.Message);
        }
        SendBytes(data);
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
                Client.GetStream().Close();
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
    /// <param name="data">The byte array to be sent.</param>
    public void SendBytes(byte[] data)
    {
        try
        {
            using NetworkStream stream = Client.GetStream();
            {
                int buffer = 1024 * 64;
                int unsentBytes = data.Length;
                int sentBytes = 0;

                while (unsentBytes > 0)
                {
                    int len = Math.Min(unsentBytes, buffer);
                    stream.Write(data, sentBytes, len);
                    unsentBytes -= len;
                    sentBytes += len;
                }

                ReadServerResponse();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while sending data: " + ex.Message);
        }
    }
    #endregion

    #region Private Functions

    /// <summary>
    /// Reads the server's response.
    /// </summary>
    private void ReadServerResponse()
    {
        try
        {
            using NetworkStream stream = Client.GetStream();
            {
                byte[] responseByte = new byte[45];
                int bytesRead = stream.Read(responseByte, 0, responseByte.Length);
                string message = Encoding.UTF8.GetString(responseByte, 0, bytesRead);
                Console.WriteLine(message);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while receiving response: " + ex.Message);
        }
    }
    #endregion
}

