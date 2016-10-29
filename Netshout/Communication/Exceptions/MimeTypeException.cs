using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netshout.Communication
{

    [Serializable]
    public class MimeTypeException : Exception
    {
        public MimeTypeException() : base("The MimeType of the Broadcast object was invalid; try selecting one from Netshout.Communication.MimeType") { }
        public MimeTypeException(string message, Exception inner) : base("The MimeType of the Broadcast object was invalid; try selecting one from Netshout.Communication.MimeType", inner) { }
        protected MimeTypeException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
