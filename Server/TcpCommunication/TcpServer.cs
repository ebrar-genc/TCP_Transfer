using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TcpServer;


/// <summary>
/// Represents a class that manages TCP connections.
/// /// </summary>
class Tcp_Server
{
    #region Parameters

    private TcpListener TcpListener;
    private TcpClient Client;
    private string IpAddress;
    private int Port;
    private int Buffer;
    private bool ClientConnected = false;
    private bool ListenerActive = false;
    #endregion

    #region Public

    /// <summary>
    /// Initializes and set parameters.
    /// </summary>
    /// <param name="ipAddress">The IP address to bind the server to.</param>
    /// <param name="port">The port number to listen on.</param>
    public Tcp_Server(string ipAddress, int port)
    {
        IpAddress = ipAddress;
        Port = port;
        Buffer = 1024 * 64;
    }

    /// <summary>
    /// Starts the server and listens for client connections.
    /// </summary>
    public async void Start()
    {
        TcpListener = new TcpListener(IPAddress.Parse(IpAddress), Port);
        try
        {
            TcpListener.Start();
            ListenerActive = true;
            Console.WriteLine("Server is listening on " + IpAddress + ":" + Port);

            while (true)
            {
                Client = await TcpListener.AcceptTcpClientAsync();
                if (Client != null)
                {
                    ClientConnected = true;
                    ReceiveBytes();
                }
                else
                {
                    Console.WriteLine("Failed to accept client connection.");
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("An error occurred while starting the server: " + ex.Message);
        }
    }

    /// <summary>
    /// Stops the server.
    /// </summary>
    public void Stop()
    {
        try
        {
            if (Client != null && Client.Connected)
            {
                ClientConnected = false;
                Client.Close();
                Client = null;
            }

            if (TcpListener != null && ListenerActive)
            {
                ListenerActive = false;
                TcpListener.Stop();
                TcpListener = null;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to stop the server: " + e.ToString());
        }
    }
    #endregion


    #region Private Functions

    /// <summary>
    /// The incoming information is first divided into pieces. ProcessMessage() is then sent for processing
    /// </summary>
    private void ReceiveBytes()
    {
        using NetworkStream stream = Client.GetStream();
        {
            try
            {
                Message receivedMessage = ReadMessage(stream);
                if (receivedMessage != null)
                {
                    ProcessMessage(receivedMessage);
                }
                else
                {
                    Console.WriteLine("Failed to parse the message header.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred while trying to read the incoming value. Message: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Reading and analyzing messages
    /// </summary>
    /// <param name="stream">The network stream to read from.</param>
    /// <returns>Returns the information of incoming data or null in case of an error.</returns>
    private Message ReadMessage(NetworkStream stream)
    {
        byte[] headerType = new byte[5];
        stream.Read(headerType, 0, headerType.Length);

        Header header = ParseHeader(headerType, stream);
        if (header != null)
        {
            int unreadBytes = header.ContentLength;
            byte[] contentByte = new byte[unreadBytes];
            int readBytes = 0;

            while (unreadBytes > 0)
            {
                int len = Math.Min(unreadBytes, Buffer);
                stream.Read(contentByte, readBytes, len);
                unreadBytes -= len;
                readBytes += len;
                Debug.WriteLine("readBytes: " + readBytes);
            }
            return new Message { Header = header, ContentByte = contentByte };

        }
        return null;
    }

    /// <summary>
    /// Parses the header information from the network stream.
    /// </summary>
    /// <param name="headerType">It contains information about whether it is a string header or a file header.</param>
    /// <param name="stream">The network stream to read additional data if needed.</param>
    /// <returns>The parsed header or null in case of an error.</returns>
    private Header ParseHeader(byte[] headerType, NetworkStream stream)
    {
        DataInfo dataInfo = (DataInfo)headerType[0];
        if (dataInfo == DataInfo.String)
        {
            //1 2 3 4 byte = int ==> headerBytes[5] 
            int contentLength = BitConverter.ToInt32(headerType, 1);
            //sırada okunacak bytelar: contentlen kadar gelen string

            return new Header { DataInfo = dataInfo, ContentLength = contentLength, FileName = "str" };
        }
        else if (dataInfo == DataInfo.File)
        {
            //TOTAL HEADER LENGTH = 4 BYTE
            int totalHeaderLen = BitConverter.ToInt32(headerType, 1);

            // Length of incoming data --> NEXT 4 BYTE
            byte[] contentByte = new byte[4];
            stream.Read(contentByte, 0, contentByte.Length);
            int contentLen = BitConverter.ToInt32(contentByte, 0);
            Debug.WriteLine("Received content length: " + contentLen);

            // FileName = (totalHeaderLen - headerType[5] - contentByte(4))
            byte[] fileNameBytes = new byte[totalHeaderLen - 9];
            stream.Read(fileNameBytes, 0, fileNameBytes.Length);
            string fileName = Encoding.UTF8.GetString(fileNameBytes);

            return new Header { DataInfo = dataInfo, ContentLength = contentLen, FileName = "\\" + fileName};
        }
        return null;
    }

    /// <summary>
    /// Processes the received message and performs appropriate actions.
    /// </summary>
    /// <param name="receivedMessage">The file message containing information about the file to be saved.</param>
    private void ProcessMessage(Message receivedMessage)
    {
        if (receivedMessage.Header.DataInfo == DataInfo.File)
        {
            GetSavePath(receivedMessage);
            File.WriteAllBytes(receivedMessage.SavePath, receivedMessage.ContentByte);
            Console.WriteLine("The file was saved successfully");
        }
        else
        {
            string content = Encoding.UTF8.GetString(receivedMessage.ContentByte);
            Console.WriteLine("Incoming data: " + content);
        }

        SendResponse();
    }

    /// <summary>
    /// Sends a response to the client through stream.
    /// </summary>
    private void SendResponse()
    {
        string response = "Server's response: transfer is successful!";
        try
        {
            NetworkStream stream = Client.GetStream();
            byte[] responseBytes = Encoding.UTF8.GetBytes(response);
            stream.Write(responseBytes, 0, responseBytes.Length);
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while SENDRESPONSE() " + ex.Message);
        }
    }

    /// <summary>
    /// Obtains and validates the save path for file messages.
    /// </summary>
    /// <param name="message">The file message containing information about the file to be saved.</param>
    private void GetSavePath(Message message)
    {
        bool validPath = false;

        while (!validPath)
        {
            Console.WriteLine("Enter the path where you want to save your file:");
            string save = Console.ReadLine();

            if (Path.IsPathRooted(save) && Directory.Exists(save))
            {
                string savePath = save + message.Header.FileName;
                message.SavePath = savePath;
                validPath = true;
            }
            else
            {
                Console.WriteLine("Please enter a valid path.");
            }
        }
    }
    #endregion
}
