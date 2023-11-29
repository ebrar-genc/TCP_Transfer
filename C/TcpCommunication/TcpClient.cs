using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;


/// <summary>
/// Represents a TCP client for communication with a server.
/// </summary>
class TcpDataTransfer
{
    private TcpClient client;
    private string ipAddress;
    private int port;

    #region Constructor

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
    public void Connect()
    {
        try
        {
            client = new TcpClient();
            client.Connect(ipAddress, port);
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
            NetworkStream stream = client.GetStream();//usinGGGG

            // Translate the passed message into ASCII and store it as a !!!! Byte array!!!
            byte[] strData = Encoding.ASCII.GetBytes(str);

            // send string
            stream.Write(strData, 0, strData.Length);
            Debug.WriteLine("Sent!!");
            ReadResponse(stream, -1);
        }
        catch (Exception ex) 
        {
            Console.WriteLine("An error occurred while sending text: " + ex.Message);
        }

    }

    /// send fileee
    public void SendFile(string filePath)
    {
        Debug.WriteLine("Being Sent this file...." + filePath);
        try
        {
            NetworkStream stream = client.GetStream();
            byte[] fileData = File.ReadAllBytes(filePath);
            string header = "dataType: file";
            byte[] headerBytes = Encoding.ASCII.GetBytes(header);
            byte[] dataToSend = new byte[headerBytes.Length + filePath.Length];
            headerBytes.CopyTo(dataToSend, 0);
            fileData.CopyTo(dataToSend, headerBytes.Length);
            stream.Write(dataToSend, 0, dataToSend.Length);
            Debug.WriteLine("Sent!!!");
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
                    Debug.WriteLine("ssssssssssssss");
                    int bytesToRead = Math.Min(1024, dataLength - totalBytesRead);
                    int bytesReadFromFile = stream.Read(fileBuffer, totalBytesRead, bytesToRead);
                    totalBytesRead += bytesReadFromFile;
                    if (bytesReadFromFile == 0)
                    {
                        // Eğer bytesReadFromFile 0 ise, veri yok demektir ve döngüden çıkabiliriz.
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
            Debug.WriteLine("5555555555");

            Debug.WriteLine("Server Response: " + response);
            //Disconnect();
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
    private void Disconnect()
    {
            if (client != null)
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
            else
            {
                Console.WriteLine("TcpClient is already null.");
            }
    }

    #endregion
 
}











/*
 * public event EventHandler<string> DataReceived ----> search it
 * task, async --> search it
*/


/*
 * /*if (Path.IsPathRooted(request))
            {
             if (File.Exists(request))
             {

             }
             else
             {
                 Console.WriteLine("Not a valid file path. Press Enter to exit...");
             }
            }*/







//classlar başka classtan inerit olabilir start connect vs aynı











