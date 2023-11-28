using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;

/// <summary>
/// Represents a TCP server for handling client connections and requests.
/// </summary>
class Tcp_Server
{
    private TcpListener tcpListener;
    private string ipAddress;
    private int port;
    private bool isRunning;

    #region Constructors

    /// <summary>
    /// Initializes and set parameters.
    /// </summary>
    /// <param name="ipAddress">The IP address to bind the server to.</param>
    /// <param name="port">The port number to listen on.</param>
    public Tcp_Server(string ipAddress, int port)
    {
        this.ipAddress = ipAddress;
        this.port = port;
        this.isRunning = false;
        this.tcpListener = new TcpListener(IPAddress.Parse(ipAddress), port);
        Debug.WriteLine("Hello Server Constructor");
    }

    #endregion

    #region Start

    /// <summary>
    /// Starts the server and listens for client connections.
    /// </summary>
    public void Start()
    {
        this.isRunning = true;
        NetworkStream stream;
        tcpListener.Start();
        int i = 0;
        Console.WriteLine("Server is listening on " + ipAddress + ":" + port);

        while (this.isRunning)
        {
            TcpClient client = tcpListener.AcceptTcpClient(); //blocking
            stream = client.GetStream();
            Console.WriteLine("Client connected!" + i);
            i++;
            // Process client request
            ProcessInput(stream);
            
        }
    }

    #endregion

    #region Process

    /// <summary>
    /// Processes the input received from the client.
    /// </summary>
    static private void ProcessInput(NetworkStream stream)
    {
        /*// Read the header to determine the type of data
         byte[] headerBytes = new byte[sizeof(bool)];
         stream.Read(headerBytes, 0, headerBytes.Length);
         bool isFile = BitConverter.ToBoolean(headerBytes, 0);

         if (isFile)
         {
             // If it's a file, read the file content
             byte[] fileContent = new byte[4096];
             int bytesRead = stream.Read(fileContent, 0, fileContent.Length);
             Console.WriteLine("FFFİLE");
             // Process file content...
         }
         else
         {
             // If it's a string, read the string
             byte[] stringData = new byte[4096];
             int bytesRead = stream.Read(stringData, 0, stringData.Length);
             string receivedString = Encoding.UTF8.GetString(stringData, 0, bytesRead);
             Console.WriteLine("sstrrng");

             // Process string...
         }*/
        byte[] stringData = new byte[4096];
        int bytesRead = stream.Read(stringData, 0, stringData.Length);
        string response = Encoding.UTF8.GetString(stringData, 0, bytesRead);
        SendResponse(stream, response);
    }



    /// <summary>
    /// Sends a response to the client through stream.
    /// </summary>
    static public void SendResponse(NetworkStream stream, string response)
    {
        byte[] responseBytes = Encoding.ASCII.GetBytes(response);
        stream.Write(responseBytes, 0, responseBytes.Length);
    }

    #endregion

    #region Stop

    /// <summary>
    /// Stops the server.
    /// </summary>
    public void Stop()
    {
        this.isRunning = false;
        this.tcpListener.Stop(); //client beklemeyi durdurur
        tcpListener = null;
    }

    #endregion
}


/*
 * Capture client request with 
 * handle multiple client connections
 * await ListenForClientsAsync()---->search it
 */