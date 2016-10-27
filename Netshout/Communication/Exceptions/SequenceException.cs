using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netshout.Communication
{

    [Serializable]
    public class SequenceException : Exception
    {
        public SequenceException() { }
        public SequenceException(int Flags) : base("Server was not expecting packet with flag: " + Flags.ToString() + "; ensure your steps are not out-of-order.") { }
        public SequenceException(String Message, Exception Inner) : base(Message, Inner) { }
        protected SequenceException(
          System.Runtime.Serialization.SerializationInfo Info,
          System.Runtime.Serialization.StreamingContext Context) : base(Info, Context) { }
    }
}
