using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netshout.Communication
{
    static class Packet
    {
        /// <summary>
        /// Serializes the packet into a byte array, allowing it to be sent over a socket.
        /// </summary>
        /// <param name="Flags">The MessageFlag corresponding to the message's Ultravox class and type.</param>
        /// <param name="PayloadData">The payload for the packet itself.</param>
        /// <returns></returns>
        public static byte[] Serialize(MessageFlag Flags, byte[] PayloadData)
        {
            byte[] UltravoxMessage = new byte[] { 0x5A, 0x0, 0x0, 0x0, 0x0, 0x0 };

            byte[] LengthBytes = BitConverter.GetBytes((Int16)(PayloadData.Length));

            if (BitConverter.IsLittleEndian)
            {
                byte[] FlagBytes = BitConverter.GetBytes((Int16)(Flags)).Reverse().ToArray();
                Flags = (MessageFlag)(BitConverter.ToInt16(FlagBytes, 0));

                LengthBytes = LengthBytes.Reverse().ToArray();
            }
            
            Buffer.BlockCopy(BitConverter.GetBytes((short)(Flags)), 0, UltravoxMessage, 2, 2);

            if (PayloadData.Length > 255)
            {
                Buffer.BlockCopy(LengthBytes, 0, UltravoxMessage, 4, 2);
            }

            else
            {
                UltravoxMessage[5] = Convert.ToByte(PayloadData.Length);
            }

            byte[] FinalBytes = new byte[UltravoxMessage.Length + PayloadData.Length + 1];

            Buffer.BlockCopy(UltravoxMessage, 0, FinalBytes, 0, UltravoxMessage.Length);
            Buffer.BlockCopy(PayloadData, 0, FinalBytes, UltravoxMessage.Length, PayloadData.Length);

            UltravoxMessage = new byte[] { 0x5A, 0x0, 0x0, 0x0, 0x0, 0x0 };

            return FinalBytes;
        }
    }
}
