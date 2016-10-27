using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netshout.Communication
{

    [Serializable]
    public class BitrateDeniedException : Exception
    {
        public BitrateDeniedException() : base("The specified bitrate is invalid or not supported by the distribution point. Try a lower bitrate value.") { }
        public BitrateDeniedException(Exception Inner) : base("The specified bitrate is invalid or not supported by the distribution point. Try a lower bitrate value.", Inner) { }
        protected BitrateDeniedException(
          System.Runtime.Serialization.SerializationInfo Info,
          System.Runtime.Serialization.StreamingContext Context) : base(Info, Context) { }
    }
}
