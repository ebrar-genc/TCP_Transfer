using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;


class InputCheckAndSend
{
    #region Parameters;

    TcpDataTransfer Client;
    #endregion

    #region Public

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public InputCheckAndSend(TcpDataTransfer client)
    {
        Client = client;
    }
    #endregion

    #region Public Function

    /// <summary>
    /// Check whether it is string or file and call CreateHeader() function to prepare header.
    /// </summary>
    /// <param name="input">The input string or file path.</param>
    public void IsPath(string input)
    {
        try
        {
            if(IsValidPath(input))
                CreateHeader(input, "file");       
            else
                CreateHeader(input, "str");
            Console.WriteLine("Sent!!!");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }

    private bool IsValidPath(string input)
    {
        if (Path.IsPathRooted(input) && (File.Exists(input) || Directory.Exists(input)))
        {
            return true;
        }
        return false;
    }
    #endregion

    #region Private Functions

    /// <summary>
    /// Prepare special headers for string and file. 
    /// Send the information combined with the header as bytes to the SendByte() function.
    /// </summary>
    /// <param name="input">The input string or file path.</param>
    /// <param name="flag">The flag indicating whether the input is a file or a string.</param>
    private void CreateHeader(string input, string flag)
    {
        byte[] inputBytes;
        string header;
        byte[] headerBytes;
        byte[] finalBytes;

        if (flag == "file")
        {
            inputBytes = File.ReadAllBytes(input);
            string fileName = Path.GetFileNameWithoutExtension(input);
            string fileExtension = Path.GetExtension(input);
            Console.WriteLine("Enter the path where you want to save your file");
            string pathToSave = Console.ReadLine();
            if (!IsValidPath(pathToSave))
                Console.WriteLine("Error");//memorycheck!!
            header = "FileName: " + fileName + "\nFileExtension: " + fileExtension + "\npathToSave: " + pathToSave + "\nFileLength: " + inputBytes.Length + "\n"; 
        }
        else
        {
            inputBytes = Encoding.ASCII.GetBytes(input);
            header = "StrLength: " + inputBytes.Length + "\n";
        }
        headerBytes = Encoding.ASCII.GetBytes(header);
        finalBytes = new byte[headerBytes.Length + inputBytes.Length];
        headerBytes.CopyTo(finalBytes, 0);
        inputBytes.CopyTo(finalBytes, headerBytes.Length);

        Console.WriteLine("Sending .. " + flag);
        Client.SendBytes(finalBytes);
    }

    #endregion
}

/// <summary>
/// Represents a TCP client for communication with a server.
/// </summary>
class TcpDataTransfer
{
    #region Parameters;

    private TcpClient Client;
    private string IpAddress;
    private int Port;
    
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
        Connect();
    }

    /// <summary>
    /// Disconnects from the server and releases resources.
    /// </summary>
    public void Disconnect()
    {
        try
        {
            Client.Close();
            Client.Dispose();
            Console.WriteLine("Disconnected from the server.");
            Client = null;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while disconnecting" + ex.Message);
        }
    }
    #endregion

    #region Public Functions

    public void SendBytes(byte[] finalBytes)
    {
        int finalLength = finalBytes.Length;
        try
        {
            NetworkStream stream = Client.GetStream();

            if (finalLength <= Buffer)
            {
                stream.Write(finalBytes, 0, finalLength);
            }
            else
            {
                //send in chunkd
                int unsentBytes = finalLength;
                int sentBytes = 0;

                while (unsentBytes > 0)
                {
                    int len = Math.Min(unsentBytes, Buffer);
                    stream.Write(finalBytes, sentBytes, len);
                    unsentBytes -= len;
                    sentBytes += len;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while sending text: " + ex.Message);
        }
    }
    #endregion

    #region Private Functions

    /// <summary>
    /// Connects to the server.
    /// </summary>
    private void Connect()
    {
        try
        {
            Client = new TcpClient();
            Client.Connect(IpAddress,Port);
            Console.WriteLine("Connected to server");
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error connecting to the server: " + ex.Message);
        }
    }
    #endregion
}



/*
 * 
    #region Server's Response
    /// <summary>
    /// Reads the server's response.
    /// </summary>
    /// <param name="stream">The NetworkStream used for reading.</param>
    /*private void ReadResponse(NetworkStream stream, int dataLength)
    {
        string response;

        try
        {
            if (dataLength > 0)
            {
                byte[] headerBuffer = new byte[1024];
                int headerBytesRead = stream.Read(headerBuffer, 0, headerBuffer.Length); //If there is no data, the program is waiting
                
                string header = Encoding.ASCII.GetString(headerBuffer, 0, headerBytesRead);

                // Read file data, header not included!!
                byte[] fileBuffer = new byte[dataLength];

                int totalBytesRead = 0;

                while (totalBytesRead < dataLength)
                {
                    int bytesToRead = Math.Min(1024, dataLength - totalBytesRead);
                    int bytesReadFromFile = stream.Read(fileBuffer, totalBytesRead, bytesToRead);
                    totalBytesRead += bytesReadFromFile;
                    if (bytesReadFromFile == 0)
                    {
                        // there is no data and we can exit the loop.
                        break;
                    }
                }
                response = Encoding.ASCII.GetString(fileBuffer);
            }
            else
            {
                byte[] responseData = new byte[1024];
                int bytesRead = stream.Read(responseData, 0, responseData.Length);
                response = Encoding.ASCII.GetString(responseData, 0, bytesRead);
            }
            Console.WriteLine("Sent!!!...Server Response: " + response);
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error reading server response: " + ex.Message);
        }
    }*/
