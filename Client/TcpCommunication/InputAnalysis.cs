using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tcp_Client
{
    class InputAnalysis
    {
        #region Public

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public InputAnalysis()
        {
        }
        #endregion

        #region Public Function

        /// <summary>
        /// Analyzes the incoming input to determine whether it is a string or a file path.
        /// Calls appropriate methods to create the header.
        /// Returns the final byte array prepared for sending over the network.
        /// </summary>
        /// <param name="input">The input string or file path.</param>
        /// <returns>The final byte array for network transmission.</returns>
        public byte[] Analysis(string input)
        {
            byte[] finalBytes = null;
            byte[] headerBytes = null;
            byte[] dataBytes = null;
            try
            {
                if (IsValidPath(input))
                {
                    dataBytes = File.ReadAllBytes(input);
                    headerBytes = CreateFileHeader(input, dataBytes.Length);
                }
                else
                {
                    headerBytes = CreateStringHeader(input);
                    dataBytes = Encoding.UTF8.GetBytes(input);
                }

                finalBytes = PrepareFinalBytes(headerBytes, dataBytes);
                return finalBytes;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }
        #endregion

        #region Private Functions

        /// <summary>
        /// Checks whether the provided input is a valid file path or directory.
        /// </summary>
        /// <param name="input">file path or directory.</param>
        /// <returns>Returns true if the input is a valid path; otherwise, returns false.</returns>
        private bool IsValidPath(string input)
        {
            return Path.IsPathRooted(input) && (File.Exists(input) || Directory.Exists(input));
        }

        /// <summary>
        /// Creates the header for string data.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The header for string data.</returns>
        private byte[]    CreateStringHeader(string input)
        {
            byte[] headerBytes = new byte[5];

            /// [0] = dataType
            headerBytes[0] = (byte)DataTypes.String;

            byte[] inputByte = BitConverter.GetBytes(input.Length);
            /// [1,2,3,4] = inputLength
            inputByte.CopyTo(headerBytes, 1);

            return headerBytes;
        }

        /// <summary>
        /// Creates the header for file data.
        /// </summary>
        /// <param name="input">The file path.</param>
        /// <param name="dataLen">The length of the file content.</param>
        /// <returns>The header for file data.</returns>
        private byte[]    CreateFileHeader(string input, int dataLen)
        {
            string fileName = Path.GetFileName(input);
            Debug.WriteLine("filecontentbytelen: " + dataLen);

            byte[] headerBytes = new byte[fileName.Length + 9];

            /// [0] = dataType
            headerBytes[0] = (byte)DataTypes.File;

            /// [1,2,3,4] = headerLen
            byte[] headerLen = BitConverter.GetBytes(headerBytes.Length);
            headerLen.CopyTo(headerBytes, 1);

            /// [5,6,7,8] = fileContent
            byte[] fileContentBytes = BitConverter.GetBytes(dataLen);
            fileContentBytes.CopyTo(headerBytes, 5);

            /// [9,.......] = fileName
            byte[] nameBytes = Encoding.UTF8.GetBytes(fileName);
            nameBytes.CopyTo(headerBytes, 9);

            return headerBytes;
        }

        /// <summary>
        /// Prepares the final byte array by combining header and data.
        /// </summary>
        /// <param name="headerBytes">The header bytes.</param>
        /// <param name="dataBytes">The incoming data bytes.</param>
        /// <returns>The final byte array.</returns>
        private byte[] PrepareFinalBytes(byte[] headerBytes, byte[] dataBytes)
        {
            byte[] finalBytes = new byte[headerBytes.Length + dataBytes.Length];

            headerBytes.CopyTo(finalBytes, 0);
            dataBytes.CopyTo(finalBytes, headerBytes.Length);

            Debug.WriteLine("HeaderBytes length: " + headerBytes.Length);

            return finalBytes;
        }
        #endregion
    }
}


