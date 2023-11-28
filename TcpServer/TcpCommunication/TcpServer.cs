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

    #region Methods

    /// <summary>
    /// Starts the server and listens for client connections.
    /// </summary>
    public void Start()
    {
        this.isRunning = true;
        tcpListener.Start();//********

        Console.WriteLine("Server is listening on " + ipAddress + ":" + port);

        while (this.isRunning)
        {
            TcpClient client = tcpListener.AcceptTcpClient();
            Console.WriteLine("Client connected!");
            Console.ReadLine();

            // Process client request
            ProcessInput(client);
            client.Close();
        }
    }

    /// <summary>
    /// Processes the input received from the client.
    /// </summary>
    private void ProcessInput(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        byte[] requestBuffer = new byte[4096];
        int bytesRead = stream.Read(requestBuffer, 0, requestBuffer.Length);
        string request = Encoding.ASCII.GetString(requestBuffer, 0, bytesRead);
        Console.WriteLine("Received request: " + request);
        SendResponse(stream, "File path received successfully!");

        // PERFORM STRING OR FILE ACCORDING TO READBYTEA *****
        Console.ReadLine();
        //checks if file path valid && exist->even though there is a file path, if that file does not physically exist or does not have read permissions
        if (!Path.IsPathRooted(request) && !File.Exists(request))
        {
            SendResponse(stream, "Valid file path received successfully!");
            try
            {
                Console.WriteLine("Press Enter to read the file content.");
                Console.ReadLine();

                // read a file content
                string fileContent = File.ReadAllText(request);
                Console.WriteLine("File Content: " + fileContent);

                
            }
            Console.WriteLine("Press Enter to continue.");
            Console.ReadLine();


        }
        else
        {
            Console.WriteLine("Invalid path. Press Enter to exit...");
            Console.ReadLine();
            Stop();
        }

        /// <summary>
        /// Sends a response to the client through stream.
        /// </summary>
        private void SendResponse(NetworkStream stream, string response)
        {
            byte[] responseBytes = Encoding.ASCII.GetBytes(response);
            stream.Write(responseBytes, 0, responseBytes.Length);
        }

        /// <summary>
        /// Stops the server.
        /// </summary>
        public void Stop()
        {
            this.isRunning = false;
            this.tcpListener.Stop();
        }

        #endregion

    /*
     * Capture client request with 
     * handle multiple client connections
     * await ListenForClientsAsync()---->search it
     */