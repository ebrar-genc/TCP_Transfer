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
    private TcpListener tcpListener;///uPPERCASE
    private TcpClient Client;
    private Thread listenerThread;

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
        Debug.WriteLine("Hello Server Constructor");
    }

    #endregion

    #region Start

    /// <summary>
    /// Starts the server and listens for client connections.
    /// </summary>
    public void Start()
    {
        try
        {
            this.isRunning = true;//***
            tcpListener = new TcpListener(IPAddress.Parse(ipAddress), port);
            listenerThread = new Thread(new ThreadStart(ListenForClients));
            listenerThread.Start();
            Console.WriteLine("Server is listening on " + ipAddress + ":" + port);
          
        }
        catch (Exception ex)
        {
            Debug.WriteLine("An error occurred while starting the server: " + ex.Message);
        }

    }

    private void ListenForClients()
    {
        tcpListener.Start();
        int i = 1;
        while (true)
        {
            // Listen to client connections
            Client = tcpListener.AcceptTcpClient(); //blocking mode
            Console.WriteLine("Client connected! " + i);
            i++;

            // Create a new thread and process the connection
            Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
            clientThread.Start(Client);
        }
    }

    private void HandleClientComm(object clientObj)
    {
        TcpClient client = (TcpClient)clientObj;
        NetworkStream stream = client.GetStream();

        ProcessInput(client, stream);/////optimize
        stream.Close();
        client.Close();
    }

        #endregion

        #region Process

        /// <summary>
        /// Processes the input received from the client.
        /// </summary>
        private void ProcessInput(TcpClient client, NetworkStream stream)
    {
        try
        {
            byte[] data = new byte[1024]; //make specific****
            int bytesRead = stream.Read(data, 0, data.Length);
            string message = Encoding.ASCII.GetString(data, 0, bytesRead);
            if (IsFileHeader(message))
            {
                // Extract file name from file header
                string[] headerParts = message.Split(';');
                string fileName = headerParts[0];
                int dataLength = int.Parse(headerParts[1]);

                Debug.WriteLine($"Gelen dosya: {fileName}, Boyut: {dataLength} byte");

                string savePath = Path.Combine("C:\\Users\\ebrar\\Desktop\\S", fileName); // Dizini değiştirin

                using (FileStream fileStream = File.Create(savePath))
                {
                    byte[] fileBuffer = new byte[1024];
                    int totalBytesRead = 0;

                    while (totalBytesRead < dataLength)
                    {
                        int bytesToRead = Math.Min(1024, dataLength - totalBytesRead);
                        int bytesReadFromFile = stream.Read(fileBuffer, 0, bytesToRead);
                        fileStream.Write(fileBuffer, 0, bytesReadFromFile);
                        totalBytesRead += bytesReadFromFile;
                    }

                    Debug.WriteLine("Dosya başarıyla alındı ve kopyalandı.");
                }
            }
            else
            {

            }

            SendResponse(stream, message);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"An error occurred while trying to read the incoming value. Message: {ex.Message}, Stack Trace: {ex.StackTrace}");
        }
    }

    private bool IsFileHeader(string data)
    {
        string headerPattern = "dataType: file";

        return data.Contains(headerPattern, StringComparison.OrdinalIgnoreCase);
    }


    /// <summary>
    /// Sends a response to the client through stream.
    /// </summary>
    public void SendResponse(NetworkStream stream, string response)
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
        try
        {
            tcpListener?.Stop();

            // Client nesnesini doğru bir şekilde kapat
            if (Client != null)
            {
                Client.GetStream()?.Close();
                Client.Close();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"aaaaaaAn error occurred while stopping the server: {ex.Message}");
        }
    }

    #endregion
}


/*
 * Capture client request with 
 * handle multiple client connections
 * await ListenForClientsAsync()---->search it
 */
