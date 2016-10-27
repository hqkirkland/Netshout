using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netshout.Communication
{

    [Serializable]
    public class CompatibilityModeDisabledException : Exception
    {
        public CompatibilityModeDisabledException() : base("The distribution point does not have legacy SHOUTcast support enabled. The value was not set.") { }
        public CompatibilityModeDisabledException(Exception Inner) : base("The distribution point does not have legacy SHOUTcast support enabled. The value was not set.", Inner) { }
        protected CompatibilityModeDisabledException(
          System.Runtime.Serialization.SerializationInfo Info,
          System.Runtime.Serialization.StreamingContext Context) : base(Info, Context) { }
    }
}
