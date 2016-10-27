using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netshout.Communication
{

    [Serializable]
    public class BufferNegotiationException : Exception
    {
        public BufferNegotiationException() : base("The minimum buffer size was denied by the server; try a lower value.") { }
        public BufferNegotiationException(String Message, Exception Inner) : base(Message, Inner) { }
        protected BufferNegotiationException(
          System.Runtime.Serialization.SerializationInfo Info,
          System.Runtime.Serialization.StreamingContext Context) : base(Info, Context) { }
    }
}
