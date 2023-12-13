using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Net;
using System.Net.Mime;
using System.Net.Sockets;
using System.Reflection.Metadata;
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

            int unreadBytes = header.ContentLength;
            byte[] contentByte = new byte[unreadBytes];

            int readBytes = 0;
            // byte[] contentByte = ReadContent(stream, header.ContentLength);
            while (unreadBytes > 0)
            {
                Debug.WriteLine("unreadBytes: " + unreadBytes);
                int len = Math.Min(unreadBytes, Buffer);
                stream.Read(contentByte, readBytes, len);
                unreadBytes -= len;
                readBytes += len;
                Debug.WriteLine("readBytes: " + readBytes);


            }
            string content = Encoding.UTF8.GetString(contentByte);
            Debug.WriteLine("content's information: " + content);
            if (header.DataInfo == DataInfo.File)
            {
                HandleReceivedData(contentByte);
            }
            return new Message { Header = header, Content = content };

        }
        return null;
    }

    private Header ParseHeader(byte[] headerBytes, NetworkStream stream)
    {
        DataInfo dataInfo = (DataInfo)headerBytes[0];
        if (dataInfo == DataInfo.String)
        {
            int contentLength = BitConverter.ToInt32(headerBytes, 1);
            Debug.WriteLine("contentlen: " + contentLength);

            return new Header { DataInfo = dataInfo, ContentLength = contentLength, ContentName = "str" };//sırada okunacak bytelar: contentlen kadar gelen string
        }
        else if (dataInfo == DataInfo.File)
        {
            //TOTAL HEADER LENGTH
            int headerLen = BitConverter.ToInt32(headerBytes, 1);
            Debug.WriteLine("total headerLen: " + headerLen);


            // Length of incoming data
            byte[] contentByte = new byte[4];
            stream.Read(contentByte, 0, contentByte.Length);
            int contentLen = BitConverter.ToInt32(contentByte, 0);
            Debug.WriteLine("incoming data length: " + contentLen);


            // FileName = (headerLen - contentLen(4byte) - 5byte)
            byte[] fileNameBytes = new byte[headerLen - 9];
            stream.Read(fileNameBytes, 0, fileNameBytes.Length);
            string fileName = Encoding.UTF8.GetString(fileNameBytes);
            Debug.WriteLine("Filename: " + fileName);

            return new Header { DataInfo = dataInfo, ContentLength = contentLen, ContentName = fileName, SavePath = "C:\\Users\\ebrar\\Desktop\\aa" + "selammm" };
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
                    Debug.WriteLine("Received data type: " + receivedMessage.Header.DataInfo);
                    Debug.WriteLine("Received content length: " + receivedMessage.Header.ContentLength);
                    Debug.WriteLine("Received data name: " + receivedMessage.Header.ContentName);
                    //Debug.WriteLine("Received message: " + receivedMessage.ContentByte);
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
        byte[] contentByte = new byte[contentLength];

         if (contentLength <= Buffer)
         {
             Debug.WriteLine("contentbyte Length: " + contentLength);
             stream.Read(contentByte, 0, contentByte.Length);
             string readContent = Encoding.UTF8.GetString(contentByte);
             Debug.WriteLine("read data content: " + readContent);
         }
         else
         {
             // Send in chunks
             int unreadBytes = contentLength;
             int readBytes = 0;

            while (unreadBytes > 0)
            {
                Debug.WriteLine("unreadBytes: " + unreadBytes);
                int len = Math.Min(unreadBytes, Buffer);
                stream.Read(contentByte, readBytes, len);
                unreadBytes -= len;
                readBytes += len;
                Debug.WriteLine("readBytes: " + readBytes);

            }
        }
        string content = Encoding.UTF8.GetString(contentByte);
        Debug.WriteLine("content's information: " + contentByte);
        return contentByte;
    }

    private void HandleReceivedData( byte[] contentByte)
    {

        string savePath = "C:\\Users\\ebrar\\Desktop\\aa\\" + "ayn";
        Debug.WriteLine("Save Path: " + savePath);
        File.WriteAllBytes(savePath, contentByte);
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
