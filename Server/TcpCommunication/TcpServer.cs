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
                    Console.WriteLine("Received file name: " + receivedMessage.Header.FileName);
                    Console.WriteLine("Received data: " + receivedMessage.Content);
                }
                else
                {
                    Console.WriteLine("Failed to parse the message header.");
                }
                /* byte[] input = new byte[Buffer];
                 int bytesRead = stream.Read(input, 0, input.Length);
                 string message = Encoding.UTF8.GetString(input, 0, bytesRead);
                 if (message.StartsWith("File"))
                 {
                     FileProcess(message, input);
                 }
                 else
                     StrProcess(message);
                 Console.WriteLine("Message Received!");
                 SendResponse();*/
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred while trying to read the incoming value. Message: {ex.Message}, Stack Trace: {ex.StackTrace}");
            }
        }
    }


    /// <summary>
    /// Separates header and file information and saves the transferred file
    /// </summary>
    /// <param name="header">The input header string contains header and file information.</param>
    private void FileProcess(string header, byte[] input)
    {
        string[] headerLines = header.Split('\n');
        string fileName = headerLines[0].Replace("FileName: ", "");
        string fileExtension = headerLines[1].Replace("FileExtension: ", "");
        string pathToSave = headerLines[2].Replace("pathToSave: ", "");
        DataLength = int.Parse(headerLines[3].Replace("FileLength: ", ""));
        int index = 93;
        /*for (int i = 0; i < 4; i++)
        {
            index += headerLines.Length + 1;
        }*/

        try
        {

            byte[] leftDataBytes = new byte[input.Length - index - 1];
            Array.Copy(input, index + 1, leftDataBytes, 0, leftDataBytes.Length);
            string savePath = Path.Combine(pathToSave + "/" + fileName + fileExtension);
            Debug.WriteLine("Server: Incoming Header Information:");
            Debug.WriteLine(string.Join("\n", fileName, fileExtension, pathToSave, DataLength.ToString(), savePath));
            File.WriteAllBytes(savePath, leftDataBytes);
        }
        catch (Exception ex)
        {
            Console.WriteLine("FileProcess Error! " + ex.Message);
        }
    }

    /// <summary>
    /// Separates header and data information
    /// </summary>
    /// <param name="header">The input header string contains header and data information</param>
    private void StrProcess(string header)
    {
        string[] headerLines = header.Split('\n');
        DataLength = int.Parse(headerLines[0].Replace("StrLength: ", ""));
        LeftData = string.Join("\n", headerLines.Skip(1));
        Debug.WriteLine("Incoming Header Information:");
        Debug.WriteLine(DataLength.ToString());
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
