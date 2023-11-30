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
    private TcpListener TcpListener;

    private string IpAddress;
    private int Port;
    int Fd;

    #region Constructors

    /// <summary>
    /// Initializes and set parameters.
    /// </summary>
    /// <param name="ipAddress">The IP address to bind the server to.</param>
    /// <param name="port">The port number to listen on.</param>
    public Tcp_Server(string ipAddress, int port)
    {
        IpAddress = ipAddress;
        Port = port;
        Fd = 0;
        IsRunning = false;
    }

    #endregion

    #region Start

    /// <summary>
    /// Starts the server and listens for client connections.
    /// </summary>
    public async void Start()
    {
        this.IsRunning = true;

        // TcpListener initialization
        using (TcpListener = new TcpListener(IPAddress.Parse(IpAddress), Port))
        {
            try
            {
                TcpListener.Start();
                Console.WriteLine("Server is listening on " + IpAddress + ":" + Port);

                while (true)
                {
                    using (TcpClient client = await TcpListener.AcceptTcpClientAsync())
                    {
                        Console.WriteLine(++Fd + ". client is connected! ");
                        ProcessInput(client);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("An error occurred while starting the server: " + ex.Message);
            }
        }
    }
    #endregion

    #region Process

    /// <summary>
    /// Processes the input received from the client.
    /// </summary>
    private void ProcessInput(TcpClient client)
    {
        using NetworkStream stream = client.GetStream();
        {
            try
            {
                byte[] data = new byte[1024]; //make specific****
                int bytesRead = stream.Read(data, 0, data.Length);
                string message = Encoding.ASCII.GetString(data, 0, bytesRead);
                if (IsFileHeader(message))
                {
                    message = FileProcess(message, stream); 
                }
                Console.WriteLine(message);
                SendResponse(stream, message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred while trying to read the incoming value. Message: {ex.Message}, Stack Trace: {ex.StackTrace}");
            }
        }
    }

    private string FileProcess(string message, NetworkStream stream)
    {
        // Extract file name from file header
        string[] messageParts = message.Split(" file\n");
        int dataLength = messageParts[1].Length;
        byte[] messageData = Encoding.ASCII.GetBytes(messageParts[1]);

        string savePath = Path.Combine("C:\\Users\\ebrar\\Desktop\\S\\ReceivedFiles", Fd.ToString() + ".txt");

        // Write the file content to the specified file path
        File.WriteAllBytes(savePath, messageData);
        Debug.WriteLine("Dosya başarıyla alındı ve kopyalandı.");
       
        return message;
    }


    private bool IsFileHeader(string data)
    {
        string headerPattern = "dataType: file\n";
        return data.Contains(headerPattern);
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
            TcpListener?.Stop();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"aaaaaaAn error occurred while stopping the server: {ex.Message}");
        }
    }

    #endregion
}
