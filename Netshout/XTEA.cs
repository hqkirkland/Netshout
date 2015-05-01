using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Netshout
{
    class XTEA
    {
        public static string Encrypt(String dataStr, String keyStr)
        {
            byte[] dataStrBytes = Encoding.ASCII.GetBytes(dataStr);

            // Declare new var here.
            byte[] dataBytes;

            if (dataStrBytes.Length % 8 > 0)
            {
                /* If the dataStrBytes length isn't a multiple of 8, it will trigger this.
                 * The following will take the remainder of the operation dataStrBytes.Length
                 * / 8, subtract that from 8, and create a byte of that result + the current
                 * size.
                 */
                dataBytes = new byte[dataStrBytes.Length + (8 - (dataStrBytes.Length % 8))];
            }

            else
            {
                dataBytes = new byte[dataStrBytes.Length];
            }

            /* Since we cant extend byte arrays (at least, to my knowledge :), we copy
             * the filled array to the new, properly-sized one.
             */

            dataStrBytes.CopyTo(dataBytes, 0);

            Console.WriteLine("dataBytes.Length: " + dataBytes.Length);
            Console.WriteLine("dataBytes[]: " + BitConverter.ToString(dataBytes));

            UInt32[] dataArr = new UInt32[dataBytes.Length / 4];

            for (int i = 0; i < dataArr.Length; i++)
            {
                if (BitConverter.IsLittleEndian)
                {
                    // This ensures that the bytes are read correctly on little endian machines.
                    Array.Reverse(dataBytes, i * 4, 4);
                }
                /* This for loop runs through our array and converts each byte to uint32,
                 * Since every uint32 is four bytes, the index that bitConverter is going
                 * to start reading for the next uint is i * 4. 
                 */
                dataArr[i] = BitConverter.ToUInt32(dataBytes, i * 4);
            }

            // Key

            Console.Write("KEY: ");

            /* We follow the same steps as above, but the key is always going to be 4 uint32s,
             * and, therefore, we can move without the length checks since it will always
             * be 16 bytes long.
             */

            byte[] keyBytes = new byte[16];
            Encoding.ASCII.GetBytes(keyStr).CopyTo(keyBytes, 0);

            Console.WriteLine("keyBytes[]: " + BitConverter.ToString(keyBytes));

            UInt32[] keyArr = new UInt32[4];

            for (int i = 0; i < 4; i++)
            {
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(keyBytes, i * 4, 4);
                }
                keyArr[i] = BitConverter.ToUInt32(keyBytes, i * 4);
            }

            Console.WriteLine(dataArr[1].ToString() + " " + dataArr[0].ToString());
            Console.WriteLine(keyArr[0].ToString() + " " + keyArr[1].ToString() + " " + keyArr[2].ToString() + " " + keyArr[3].ToString());

            uint y = dataArr[0];
            uint z = dataArr[1];
            uint sum = 0;
            uint delta = 0x9E3779B9;
            uint n = 32;

            while (n-- > 0)
            {
                y += (z << 4 ^ z >> 5) + z ^ sum + keyArr[sum & 3];
                sum += delta;
                z += (y << 4 ^ y >> 5) + y ^ sum + keyArr[sum >> 11 & 3];
            }

            dataArr[0] = y;
            dataArr[1] = z;

            return dataArr[0].ToString("X") + dataArr[1].ToString("X");
        }
    }
}
