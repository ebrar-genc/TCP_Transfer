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
        #region Parameters



        #endregion

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
        /// Check whether it is string or file and call CreateHeader() function to prepare header.
        /// </summary>
        /// <param name="input">The input string or file path.</param>
        public byte[] Analysis(string input)
        {
            byte[] finalBytes = null;
            byte[] headerBytes = null;

            try
            {
                if (IsValidPath(input))
                    headerBytes = CreateHeader(input, DataTypes.File);
                else
                    headerBytes = CreateHeader(input, DataTypes.String);
                finalBytes = PrepareSendData(input, headerBytes);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return finalBytes;
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
        /// [packet type]+[filenameLength]+{File Name}+[fileSize (4 bytes)]
        /// </summary>
        /// <param name="input">The input string or file path.</param>
        public byte[] CreateHeader(string input, DataTypes dataType)
        {
            byte[] headerBytes = null; 

            if (dataType == DataTypes.File)
            {
                string inputName = Path.GetFileNameWithoutExtension(input);
                headerBytes = new byte[inputName.Length + 2];
                headerBytes[0] = (byte)dataType;
                headerBytes[1] = (byte)input.Length;
                byte[] nameBytes = Encoding.UTF8.GetBytes(inputName);
                nameBytes.CopyTo(headerBytes, 2);
            }
            else if (dataType == DataTypes.String)
            {
                headerBytes = new byte[5];
                uint inputLen = (uint)input.Length;
                headerBytes[0] = (byte)dataType;
                BitConverter.GetBytes(inputLen).CopyTo(headerBytes, 1);
            }
            Debug.WriteLine("Client: Header Information:");
            Debug.WriteLine(headerBytes);
            return headerBytes;
        }

        /// <summary>
        /// Combine header and input
        /// </summary>
        /// <param name="input">The input string or file path.</param>
        private byte[] PrepareSendData(string input, byte[] headerBytes)
        {
            byte[] inputBytes = null;
            byte[] finalBytes = null;

            inputBytes = Encoding.UTF8.GetBytes(input);
            finalBytes = new byte[inputBytes.Length + headerBytes.Length];
            headerBytes.CopyTo(finalBytes, 0);
            inputBytes.CopyTo(finalBytes, headerBytes.Length);
            Console.WriteLine("png length: " + headerBytes.Length);
            return finalBytes;
        }


        #endregion

    }
}


