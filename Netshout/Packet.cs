using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netshout
{
    class Packet
    {
        private byte Class;
        private byte Type;
        private String Payload;

        public Packet(byte Class, byte Type, String Payload)
        {
            this.Class = Class;
            this.Type = Type;
            this.Payload = Payload;
        }

        public byte[] Serialize()
        {
            byte[] Header = new byte[4];
            Header[0] = 0x5A;
            Header[1] = 0x0;
            Header[2] = this.Class;
            Header[3] = this.Type;

            byte[] RawPayload = Encoding.Default.GetBytes(Payload + (char)(0));

            int Len = Convert.ToInt16(RawPayload.Length - 1);
            byte[] MsgLength = BitConverter.GetBytes((short)(Len));

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(MsgLength);
            }

            byte[] OutputBuffer = new byte[RawPayload.Length + 6];

            Header.CopyTo(OutputBuffer, 0);
            MsgLength.CopyTo(OutputBuffer, 4);
            RawPayload.CopyTo(OutputBuffer, 6);

            if (this.Type == 0x050)
            {
                OutputBuffer = new byte[] { 0x5A, 0x0, 0x1, 0x050 };
            }

            return OutputBuffer;
        }
    }
}
