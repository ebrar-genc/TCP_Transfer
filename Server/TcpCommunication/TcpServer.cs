using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
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
    private int DataLength;
    private string LeftData;
    public bool ServerActive = false;
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
        DataLength = 0;
        LeftData = "";
        ServerActive = true;
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

            int i = 1;
            while (true)
            {
                Client = await TcpListener.AcceptTcpClientAsync();
                ClientConnected = true;
                Console.WriteLine(i++ + ". client is connected! ");
                ReceiveBytes();
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

            ClientConnected = false;
            Client.Close();
            Client = null;

            ListenerActive = false;
            TcpListener.Stop();
            TcpListener = null;

        }
        catch (Exception e)
        {
            Debug.WriteLine("Failed to Stop server! : " + e.ToString());
        }
    }
    #endregion


    public Message ReadMessage(NetworkStream stream)
    {
        byte[] headerName = new byte[5];
        stream.Read(headerName, 0, headerName.Length);

        Header header = ParseHeader(headerName, stream);
        if (header != null)
        {
            byte[] contentBytes = new byte[header.ContentLength];
            stream.Read(contentBytes, 0, contentBytes.Length);

            string content = Encoding.UTF8.GetString(contentBytes);

            return new Message { Header = header, Content = content };
        }
        return null;
    }

    private Header ParseHeader(byte[] headerBytes, NetworkStream stream)
    {
        DataTypes dataType = (DataTypes)headerBytes[0];

        if (dataType == DataTypes.String)
        {
            int contentLen = BitConverter.ToInt32(headerBytes, 1);
            return new Header { DataType = dataType, ContentLength = contentLen };//sıradaki contentlen kadar gelen string
        }
        else if (dataType == DataTypes.File)
        {
            //başlık uzunluğu
            int headerLen = BitConverter.ToInt32(headerBytes, 1);

            // Başlığı oku
            byte[] contentByte = new byte[4];
            stream.Read(contentByte, 0, contentByte.Length);
            int contentLen = BitConverter.ToInt32(contentByte, 0);

            // Dosya ismini oku(headerLen - contentLen(4byte) - 5byte)
            byte[] fileNameBytes = new byte[headerLen - 9];
            stream.Read(fileNameBytes, 0, fileNameBytes.Length);
            string fileName = Encoding.UTF8.GetString(fileNameBytes);

            return new Header { DataType = dataType, ContentLength = contentLen, FileName = fileName };
        }
        return null;
    }

    #region Private Functions



    /// <summary>
    /// Processes the input received from the client.
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
                    Console.WriteLine("Received data type: " + receivedMessage.Header.DataType);
                    Console.WriteLine("Received content length: " + receivedMessage.Header.ContentLength);
                    Console.WriteLine("Received file name: " + receivedMessage.Header.FileName);
                    Console.WriteLine("Received data: " + receivedMessage.Content);

                    byte[] contentByte = ReadContent(stream, receivedMessage.Header.ContentLength);

                    HandleReceivedData(receivedMessage, contentByte);
                }
                else
                {
                    Console.WriteLine("Failed to parse the message header.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred while trying to read the incoming value. Message: {ex.Message}, Stack Trace: {ex.StackTrace}");
            }
        }
    }


    private byte[] ReadContent(NetworkStream stream, int contentLength)
    {
        Console.WriteLine("111");
        byte[] contentByte = new byte[contentLength];

        if (contentLength <= Buffer)
        {
            Console.WriteLine("222");
            Console.WriteLine(contentByte.Length);

            stream.Read(contentByte, 0, contentByte.Length);
            Console.WriteLine("777");

        }
        else
        {
            Console.WriteLine("333");

            // Send in chunks
            int unreadBytes = contentLength;
            int readBytes = 0;
            Console.WriteLine("444");

            while (unreadBytes > 0)
            {
                Console.WriteLine("555");

                int len = Math.Min(unreadBytes, Buffer);
                stream.Read(contentByte, readBytes, len);
                unreadBytes -= len;
                readBytes += len;
            }
        }
        Console.WriteLine("666");
        Console.WriteLine(contentByte.ToString);


        return contentByte;
    }

    private void HandleReceivedData(Message receivedMessage, byte[] contentByte)
    {
        if (receivedMessage.Header.DataType == DataTypes.String)
        {
            string message = Encoding.UTF8.GetString(contentByte, 0, contentByte.Length);
            Console.WriteLine("Message Received! ---> " + message);
        }
        else if (receivedMessage.Header.DataType == DataTypes.File)
        {
            string savePath = "\"C:\\Users\\ebrar\\Desktop\\aa\\selam.txt.txt\"";
            Console.WriteLine("here");
            File.WriteAllBytes(savePath, contentByte);
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

    #endregion
}
