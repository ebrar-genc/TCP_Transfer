using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.IO;
using Microsoft.VisualBasic;
using static System.Runtime.InteropServices.JavaScript.JSType;

public enum DataType
{
    String,
    File
}
class InputCheckAndSend
{
    #region Parameters

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
                CreateHeader(input, DataType.File);       
            else
                CreateHeader(input, DataType.String);        }
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
    private void CreateHeader(string input, DataType dataType)
    {
        byte[] inputBytes;
        string header;
        byte[] headerBytes;
        byte[] finalBytes;

        if (dataType == DataType.File)
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
            inputBytes = Encoding.UTF8.GetBytes(input);
            header = "StrLength: " + inputBytes.Length + "\n";
        }
        Debug.WriteLine("Client: Transferred Header Information:");
        Debug.WriteLine(header);
        headerBytes = Encoding.UTF8.GetBytes(header);
        finalBytes = new byte[headerBytes.Length + inputBytes.Length];
        headerBytes.CopyTo(finalBytes, 0);
        inputBytes.CopyTo(finalBytes, headerBytes.Length);

        Client.SendBytes(finalBytes);
        Client.ReadResponse();
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
    public bool ClientActive = false;
    
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
            if (Client != null && Client.Connected)
            {
                Client.Close();
                Client.Dispose();
                Console.WriteLine("Disconnected from the server.");
            }
            Client = null;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while disconnecting: " + ex.Message);
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

    /// <summary>
    /// Reads the server's response.
    /// </summary>
    /// <param name="stream">The NetworkStream used for reading.</param>
    public void ReadResponse()
    {
        try
        {
            NetworkStream stream = Client.GetStream();
            byte[] responseByte = new byte[1024];
            int bytesRead = stream.Read(responseByte, 0, responseByte.Length);
            string message = Encoding.UTF8.GetString(responseByte, 0, bytesRead);
            Console.WriteLine(message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("aaAn error occurred while receiving response: " + ex.Message);
            Console.WriteLine("aaaAn error occurred while receiving response: " + ex.ToString());

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
            ClientActive = true;
            Console.WriteLine("Connected to server");
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error connecting to the server: " + ex.Message);
        }
    }
    #endregion
}


    