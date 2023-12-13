using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpServer
{
    /// <summary>
    /// Represents the header information for the message.
    /// </summary>
    public class Header
    {
        /// <summary>
        /// Gets or sets the type of data in the message.
        /// </summary>
        public DataInfo DataInfo { get; set; }

        /// <summary>
        /// Gets or sets the length of the content in the message.
        /// </summary>
        public int ContentLength { get; set; }

        /// <summary>
        /// Gets or sets the file name. (only for file data type).
        /// </summary>
        public string ContentName { get; set; }

        public string SavePath { get; set; }
    }
}
