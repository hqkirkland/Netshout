using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netshout.Communication
{

    [Serializable]
    public class StreamInUseException : Exception
    {
        public StreamInUseException(int StreamId) : base("Stream ID " + StreamId.ToString() + " is unable to standby because it is currently in use.") { }
        public StreamInUseException(int StreamId, Exception Inner) : base("Stream ID " + StreamId.ToString() + " is unable to standby because it is currently in use.", Inner) { }
        protected StreamInUseException(
          System.Runtime.Serialization.SerializationInfo Info,
          System.Runtime.Serialization.StreamingContext Context) : base(Info, Context) { }
    }
}
