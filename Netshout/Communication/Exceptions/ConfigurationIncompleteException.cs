using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netshout.Communication
{

    [Serializable]
    public class ConfigurationIncompleteException : Exception
    {
        public ConfigurationIncompleteException() : base("The distribution point is not ready to receive data; stream configuration has not been completed.") { }
        public ConfigurationIncompleteException(Exception Inner) : base("The distribution point is not ready to receive data; stream configuration has not been completed.", Inner) { }
        protected ConfigurationIncompleteException(
          System.Runtime.Serialization.SerializationInfo Info,
          System.Runtime.Serialization.StreamingContext Context) : base(Info, Context) { }
    }
}
