using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Net;
using System.Net.Mime;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
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
        Buffer = 1024 * 8;
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
                if (Client != null)
                {
                    ClientConnected = true;
                    Console.WriteLine(i++ + ". client is connected! ");
                    ReceiveBytes();
                }
                else
                {
                    Console.WriteLine("Failed to acceptTTT client connection.");
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

    public Message ReadMessage(NetworkStream stream)
    {
        byte[] headerType = new byte[5];
        stream.Read(headerType, 0, headerType.Length);

        Header header = ParseHeader(headerType, stream);
        if (header != null)
        {
            if (header.DataInfo == DataInfo.File)
            {
                Buffer = 1024 * 64;
            }
            Debug.WriteLine("Buffer: " + Buffer);

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
    #endregion






    #region Private Functions

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
                    if (receivedMessage.Header.DataInfo == DataInfo.File)
                    {
                        SavePath(Message receivedMessage);
                    }
                    HandleReceivedData(receivedMessage);
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

    private void HandleReceivedData(Message receivedMessage)
    {
        bool validPath = false;


        //string content = Encoding.UTF8.GetString(receivedMessage.ContentByte);

        //!!!waiting
        while (!validPath)
        {
            Console.WriteLine("Enter the path where you want to save your file:");
            string savePath = Console.ReadLine();

            if (Path.IsPathRooted(savePath) && Directory.Exists(savePath))
            {
                string save = savePath + receivedMessage.Header.FileName;

                Console.WriteLine("Save Path: " + save);
                File.WriteAllBytes(save, contentByte);
                SendResponse();
                validPath = true;
            }
            else
            {
                Console.WriteLine("Please enter a valid path.");
            }
        }
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

    private void SavePath(Message message)
    {
        bool validPath = false;

        while (!validPath)
        {
            Console.WriteLine("Enter the path where you want to save your file:");
            string save = Console.ReadLine();

            if (Path.IsPathRooted(save) && Directory.Exists(save))
            {
                string savePath = save + message.Header.FileName;
                Debug.WriteLine("Save Path: " + savePath);
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
