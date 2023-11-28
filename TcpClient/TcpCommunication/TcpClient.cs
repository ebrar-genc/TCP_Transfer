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
        this.client = new TcpClient();
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
        Debug.WriteLine(str);
        NetworkStream stream = client.GetStream();

        // Translate the passed message into ASCII and store it as a !!!! Byte array!!!
        byte[] strData = Encoding.ASCII.GetBytes(str);

        // send string
        stream.Write(strData, 0, strData.Length);
        ReadResponse(stream);
    }

    /// send file with filezstream
    public void SendFile(string filePath)
    {
        Debug.WriteLine(filePath);
        NetworkStream stream = client.GetStream();

        using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
        {
            byte[] buffer = new byte[4096];
            int bytesRead;

            // read file and send file
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                stream.Write(buffer, 0, bytesRead);
            }

            Console.WriteLine($"Dosya gönderildi: {filePath}");
        }
        ReadResponse(stream);
    }


    #endregion

    #region 
    /// <summary>
    /// Reads the server's response.
    /// </summary>
    /// <param name="stream">The NetworkStream used for reading.</param>
    private void ReadResponse(NetworkStream stream)
    {
            Debug.WriteLine("444");
            byte[] responseData = new byte[4096];
            Debug.WriteLine("555");

            int bytesRead = stream.Read(responseData, 0, responseData.Length);
            Debug.WriteLine("664");


            string response = Encoding.ASCII.GetString(responseData, 0, bytesRead);
            Debug.WriteLine("7777");

            Debug.WriteLine("Server Response: " + response);
            //Disconnect();
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
                client.Close();
                client.Dispose();
                Console.WriteLine("Disconnected from the server.");
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











