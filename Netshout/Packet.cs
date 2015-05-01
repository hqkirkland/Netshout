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
            byte[] header = new byte[4];
            header[0] = 0x5A;
            header[1] = 0x0;
            header[2] = this.Class;
            header[3] = this.Type;

            byte[] rawPayload = Encoding.ASCII.GetBytes(Payload + (char)(0));

            int len = Convert.ToInt16(rawPayload.Length - 1);
            byte[] msgLength = BitConverter.GetBytes((short)(len));

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(msgLength);
            }

            byte[] bytesOut = new byte[rawPayload.Length + 6];

            header.CopyTo(bytesOut, 0);
            msgLength.CopyTo(bytesOut, 4);
            rawPayload.CopyTo(bytesOut, 6);

            if (this.Type == 0x050)
            {
                bytesOut = new byte[] { 0x5A, 0x0, 0x1, 0x050 };
            }

            return bytesOut;
        }
    }
}
