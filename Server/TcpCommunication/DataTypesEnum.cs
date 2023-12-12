using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpServer
{
    #region DataTypes enum
    /// <summary>
    /// Specifies the types of data for communication.
    /// </summary>
    public enum DataTypes
    {
        /// <summary>
        /// Represents string data.
        /// </summary>
        String,

        /// <summary>
        /// Represents file data.
        /// </summary>
        File,

        /// <summary>
        /// Represents content length data.
        /// </summary>
        ContentLength
    }

    #endregion
}
