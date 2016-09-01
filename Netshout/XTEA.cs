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
            byte[] DataStrBytes = Encoding.Default.GetBytes(dataStr);

            // Declare new var here.
            byte[] DataBytes;

            if (DataStrBytes.Length % 8 > 0)
            {
                /* If the DataStrBytes length isn't a multiple of 8, it will trigger this.
                 * The following will take the remainder of the operation DataStrBytes.Length
                 * / 8, subtract that from 8, and create a byte of that result + the current
                 * size.
                 */
		
                DataBytes = new byte[DataStrBytes.Length + (8 - (DataStrBytes.Length % 8))];
            }

            else
            {
                DataBytes = new byte[DataStrBytes.Length];
            }

            /* Since we cant extend byte arrays (at least, to my knowledge :), we copy
             * the filled array to the new, properly-sized one.
             */

            DataStrBytes.CopyTo(DataBytes, 0);

            Console.WriteLine("DataBytes.Length: " + DataBytes.Length);
            Console.WriteLine("DataBytes[]: " + BitConverter.ToString(DataBytes));

            UInt32[] DataArr = new UInt32[DataBytes.Length / 4];

            for (int i = 0; i < DataArr.Length; i++)
            {
                if (BitConverter.IsLittleEndian)
                {
                    // This ensures that the bytes are read correctly on little endian machines.
                    Array.Reverse(DataBytes, i * 4, 4);
                }

                /* This for loop runs through our array and converts each byte to uint32,
                 * Since every uint32 is four bytes, the index that bitConverter is going
                 * to start reading for the next uint is i * 4. 
                 */
                DataArr[i] = BitConverter.ToUInt32(DataBytes, i * 4);
            }

            // Key

            Console.Write("KEY: ");

            /* We follow the same steps as above, but the key is always going to be 4 uint32s,
             * and, therefore, we can move without the length checks since it will always
             * be 16 bytes long.
             */

            byte[] KeyBytes = new byte[16];
            Encoding.Default.GetBytes(keyStr).CopyTo(KeyBytes, 0);

            Console.WriteLine("keyBytes[]: " + BitConverter.ToString(KeyBytes));

            UInt32[] keyArr = new UInt32[4];

            for (int i = 0; i < 4; i++)
            {
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(KeyBytes, i * 4, 4);
                }

                keyArr[i] = BitConverter.ToUInt32(KeyBytes, i * 4);
            }

            Console.WriteLine(DataArr[1].ToString() + " " + DataArr[0].ToString());
            Console.WriteLine(keyArr[0].ToString() + " " + keyArr[1].ToString() + " " + keyArr[2].ToString() + " " + keyArr[3].ToString());

            uint Y = DataArr[0];
            uint Z = DataArr[1];
            uint Sum = 0;
            uint Delta = 0x9E3779B9;
            uint n = 32;

            while (n-- > 0)
            {
                Y += (Z << 4 ^ Z >> 5) + Z ^ Sum + keyArr[Sum & 3];
                Sum += Delta;
                Z += (Y << 4 ^ Y >> 5) + Y ^ Sum + keyArr[Sum >> 11 & 3];
            }

            DataArr[0] = Y;
            DataArr[1] = Z;

            return DataArr[0].ToString("X") + DataArr[1].ToString("X");
        }
    }
}
