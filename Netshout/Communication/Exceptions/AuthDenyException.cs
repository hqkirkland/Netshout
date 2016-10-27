using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netshout.Communication
{

    [Serializable]
    public class AuthDenyException : Exception
    {
        public readonly String Uid;
        public readonly String Password;
        public readonly int StreamId;

        public AuthDenyException() { }

        // Version incompatible.
        public AuthDenyException(String Version) : base("Server version is lower than Connection.Version " + Version + "; try setting Connection.Version to a lower value before authenticating.") { }
        
        // Stream moved
        public AuthDenyException(int _StreamId) : base("Stream ID " + _StreamId.ToString() + " has moved from the server.")
        {
            StreamId = _StreamId;
        }

        // Invalid credentials
        public AuthDenyException(String AttemptedUid, String AttemptedPassword) : base("Invalid credentials for UID '" + AttemptedUid + "' using password: " + AttemptedPassword)
        {
            Uid = AttemptedUid;
            Password = AttemptedPassword;

        }
            // public AuthDenyException(String AttemptedUid, String AttemptedPassword, Exception inner) : base("Invalid credentials for UID '" + AttemptedUid + "' using password: " + AttemptedPassword, inner) { }

        protected AuthDenyException(
          System.Runtime.Serialization.SerializationInfo Info,
          System.Runtime.Serialization.StreamingContext Context) : base(Info, Context) { }
    }
}
