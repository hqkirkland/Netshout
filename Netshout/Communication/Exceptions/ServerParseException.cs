using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Netshout.Communication
{

    [Serializable]
    public class ServerParseException : Exception
    {
        public ServerParseException() : base("The server could not parse the last packet sent to it.") { }
        public ServerParseException(String Message, Exception Inner) : base("The server could not parse the last packet sent to it.", Inner) { }
        protected ServerParseException(
          SerializationInfo Info,
          StreamingContext Context
        ) : base(Info, Context) { }
    }
}
