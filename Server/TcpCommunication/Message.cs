using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpServer
{
    /// <summary>
    /// Represents a message containing header and content.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Gets or sets the header of the message.
        /// </summary>
        public Header Header { get; set; }

        /// <summary>
        /// Gets or sets the content of the message.
        /// </summary>
        public byte[] ContentByte { get; set; }

        public string SavePath { get; set; }

    }


}
