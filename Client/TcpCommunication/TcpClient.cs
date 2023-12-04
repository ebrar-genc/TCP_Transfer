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
    #region Public

    public InputCheckAndSend()
    {
    }
    #endregion

    #region Public Function

    /// <summary>
    /// Check whether it is string or file
    /// </summary>
    public void IsPath(string input, TcpDataTransfer client)
    {
        try
        {
            if (Path.IsPathRooted(input))
            {
                if (File.Exists(input))
                {
                    if (IsValidFileExtension(input))
                    {
                        Console.WriteLine("Starting file transfer...");
                        client.SendFile(input);
                    }
                }
            }
            else
            {
                Console.WriteLine("Sending string...");
                client.SendString(input);
            }
            Console.WriteLine("Sent!!!");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
    #endregion

 
    #region Private Functions

    /// <summary>
    /// Valid File path and File found -> IsValidFileExtension(data);
    /// </summary>
    private bool IsValidFileExtension(string input)
    {
        string[] allowedExtensions = { ".txt", ".jpg", ".png", ".pdf", ".zip" };

        string fileExtension = Path.GetExtension(input);

        foreach (string allowedExtension in allowedExtensions)
        {
            if (string.Equals(fileExtension, allowedExtension, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
    #endregion

}

/// <summary>
/// Represents a TCP client for communication with a server.
/// </summary>
class TcpDataTransfer
{
    private TcpClient client;
    private string ipAddress;
    private int port;

    #region Public

    /// <summary>
    /// Initializes and set parameters.
    /// </summary>
    /// <param name="ipAddress">The IP address of the server.</param>
    /// <param name="port">The port number to connect.</param> 
    public TcpDataTransfer(string ipAddress, int port)
    {
        this.ipAddress = ipAddress;
        this.port = port;
        Connect();
    }
    #endregion

    #region Connection
    /// <summary>
    /// Connects to the server.
    /// </summary>
    private void Connect()
    {
        try
        {
            client = new TcpClient();
            client.Connect(ipAddress, port);
            Console.WriteLine("Connected to server");
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error connecting to the server: " + ex.Message);
        }
    }

    #endregion

    #region Data Transfer


    /// <summary>
    /// The requested string is converted into byte array and sent to the server via TCP connection.
    /// </summary>
    /// <param name="flag">Determine whether it is string or file path</param>
    /// <param name="data">The data to be sent.</param>
    public void SendString(string str)
    {
        Debug.WriteLine("Being Sent this string...." + str);

        try
        {
            using (NetworkStream stream = client.GetStream())
            {
                // Translate the passed message into ASCII and store it as a !!!! Byte array!!!
                byte[] strData = Encoding.ASCII.GetBytes(str);

                // send string
                stream.Write(strData, 0, strData.Length);
                ReadResponse(stream, -1);
            }
        }
        catch (Exception ex) 
        {
            Console.WriteLine("An error occurred while sending text: " + ex.Message);
        }

    }

    /// send fileee---byte
    public void SendFile(string filePath)
    {
        Debug.WriteLine("Being Sent this file...." + filePath);
        try
        {
            NetworkStream stream = client.GetStream();
            string header = "dataType: file\n";
            byte[] fileData = File.ReadAllBytes(filePath);
            byte[] headerBytes = Encoding.ASCII.GetBytes(header);
            byte[] dataToSend = new byte[headerBytes.Length + fileData.Length];

            //dataToSend content is created appropriately
            headerBytes.CopyTo(dataToSend, 0);
            fileData.CopyTo(dataToSend, headerBytes.Length);
            // SEND MORE OPTIMIZED WITH BUFFER
            stream.Write(dataToSend, 0, dataToSend.Length);
            ReadResponse(stream, fileData.Length);
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while sending file: " + ex.Message);
        }
    }


    #endregion

    #region Server's Response
    /// <summary>
    /// Reads the server's response.
    /// </summary>
    /// <param name="stream">The NetworkStream used for reading.</param>
    private void ReadResponse(NetworkStream stream, int dataLength)
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
    }

    #endregion

    #region Disconnect
    /// <summary>
    /// Disconnects from the server and releases resources.
    /// </summary>
    public void Disconnect()
    {
        try
        {
            client.Close();
            client.Dispose();
            Console.WriteLine("Disconnected from the server.");
            client = null;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while disconnecting" + ex.Message);
        }
    }

    #endregion
 
}

