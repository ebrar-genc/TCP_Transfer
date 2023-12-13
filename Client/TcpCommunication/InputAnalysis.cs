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
        /// Analyzes whether the input is a string or a file path, calls CreateHeader() to prepare the header,
        /// combines the header and input value, and returns the final data in the PrepareSendData() function.
        /// </summary>
        /// <param name="input">The input string or file path.</param>
        /// <returns>The final version of the data prepared for sending.</returns>
        public byte[] Analysis(string input)
        {
            byte[] finalBytes = null;
            //byte[] headerBytes = null;

            try
            {
                if (IsValidPath(input))
                    finalBytes = CreateHeader(input, DataTypes.File);
                else
                    finalBytes = CreateHeader(input, DataTypes.String);
                //finalBytes = PrepareSendData(input, headerBytes);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return finalBytes;
        }

        /// <summary>
        /// Checks whether the provided input is a valid file path or directory.
        /// </summary>
        /// <param name="input">file path or directory.</param>
        /// <returns>Returns true if the input is a valid path; otherwise, returns false.</returns>
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
        /// <param name="dataType">The type of data (String or File).</param>
        /// <returns>The byte array representing the header.</returns>
        public byte[] CreateHeader(string input, DataTypes dataType)
        {
            byte[] headerBytes = null;
            byte[] finalBytes = null;
            byte[] contentBytes = null;


            if (dataType == DataTypes.File)
            {
                Debug.WriteLine("hello file");
                string inputName = Path.GetFileNameWithoutExtension(input);

                string fileContent = File.ReadAllText(input);
                int fileContentLen = fileContent.Length;
                Debug.WriteLine("filecontentbytelen: " + fileContentLen);

                contentBytes = Encoding.UTF8.GetBytes(fileContent);

                headerBytes = new byte[inputName.Length + 9];
                headerBytes[0] = (byte)dataType;

                byte[] headerLen = BitConverter.GetBytes(headerBytes.Length);
                headerLen.CopyTo(headerBytes, 1); //1 2 3 4. byte'lara header uzunluğu yazacak

                byte[] fileContentBytes = BitConverter.GetBytes(fileContentLen);
                fileContentBytes.CopyTo(headerBytes, 5); //5 6 7 8'econtent uzunluğu

                byte[] nameBytes = Encoding.UTF8.GetBytes(inputName);
                nameBytes.CopyTo(headerBytes, 9); //9 ve gerisine dosya ismi
            }
            else if (dataType == DataTypes.String)
            {
                Debug.WriteLine("hello string");
                headerBytes = new byte[5];
                int inputLen = input.Length;
                headerBytes[0] = (byte)dataType;

                byte[] contentByte = BitConverter.GetBytes(inputLen);
                contentByte.CopyTo(headerBytes, 1);//1 2 3 4. bytelara stringin buyukluğu

                contentBytes = Encoding.UTF8.GetBytes(input);

            }
            finalBytes = new byte[contentBytes.Length + headerBytes.Length];
            headerBytes.CopyTo(finalBytes, 0);
            contentBytes.CopyTo(finalBytes, headerBytes.Length);
            Debug.WriteLine("content stringg: " + Encoding.UTF8.GetString(contentBytes));

            Debug.WriteLine("headerBytes length: " + headerBytes.Length);
            Debug.WriteLine("finalBytes content: " + BitConverter.ToString(finalBytes));


            return finalBytes;
        }

       
        #endregion

    }
}



/*
 *  /// <summary>
        /// Combine header and input
        /// </summary>
        /// <param name="input">The input string or file path.</param>
        /// <param name="headerBytes">The byte array representing the header.</param>
        /// <returns>The final byte array prepared for transmission.</returns>
        private byte[] PrepareSendData(string input, byte[] headerBytes)
        {
            byte[] inputBytes = null;
            byte[] finalBytes = null;

            inputBytes = Encoding.UTF8.GetBytes(input);
            finalBytes = new byte[inputBytes.Length + headerBytes.Length];
            headerBytes.CopyTo(finalBytes, 0);
            inputBytes.CopyTo(finalBytes, headerBytes.Length);
            Debug.WriteLine("headerBytes length: " + headerBytes.Length);
            return finalBytes;
        }
*/