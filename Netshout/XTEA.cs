using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Netshout
{
    static class XTEA
    {

        public static String Encrypt(String InputString, String KeyString)
        {
            byte[] DataStrBytes = Encoding.Default.GetBytes(InputString);
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

            DataStrBytes.CopyTo(DataBytes, 0);

            /*
            Console.WriteLine("DataBytes.Length: " + DataBytes.Length);
            Console.WriteLine("DataBytes[]: " + BitConverter.ToString(DataBytes));
            */
            
            UInt32[] DataArray = new UInt32[DataBytes.Length / 4];

            for (int i = 0; i < DataArray.Length; i++)
            {
                if (BitConverter.IsLittleEndian)
                {
                    // This ensures that the bytes are read correctly on little endian machines.
                    Array.Reverse(DataBytes, i * 4, 4);
                }

                /* This for loop runs through our array and converts bytes to uint32,
                 * Since every uint32 is four bytes, the index that BitConverter is going
                 * to start reading for the next uint is i * 4. 
                 */

                DataArray[i] = BitConverter.ToUInt32(DataBytes, i * 4);
            }

            // Key

            /*
            Console.WriteLine();
            Console.WriteLine("KEY: ");
            */

            /*
             * We follow the same steps as above, but the key is always going to be 4 uint32s,
             * and, therefore, we can move without the length checks since it will always
             * be 16 bytes long.
             */
            
            byte[] KeyBytes = new byte[16];
            Encoding.Default.GetBytes(KeyString).CopyTo(KeyBytes, 0);

            // Console.WriteLine("KeyBytes[]: " + BitConverter.ToString(KeyBytes));

            UInt32[] KeyArray = new UInt32[4];

            for (int i = 0; i < 4; i++)
            {
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(KeyBytes, i * 4, 4);
                }

                KeyArray[i] = BitConverter.ToUInt32(KeyBytes, i * 4);
            }

            /* Console.Write("Data: ");
            
            foreach(UInt32 U in DataArray)
            {
                // Console.Write(U.ToString() + " ");
            }

            Console.Write('\n');
            Console.WriteLine("Key: " + KeyArray[0].ToString() + " " + KeyArray[1].ToString() + " " + KeyArray[2].ToString() + " " + KeyArray[3].ToString());
            */

            uint Y = DataArray[0];
            uint Z = DataArray[1];


            uint Sum = 0;
            uint Delta = 0x9E3779B9;
            uint N = 32;

            while (N-- > 0)
            {
                Y += (Z << 4 ^ Z >> 5) + Z ^ Sum + KeyArray[Sum & 3];
                Sum += Delta;
                Z += (Y << 4 ^ Y >> 5) + Y ^ Sum + KeyArray[Sum >> 11 & 3];
            }

            DataArray[0] = Y;
            DataArray[1] = Z;

            String Result = DataArray[0].ToString("X") + DataArray[1].ToString("X");

            if (DataBytes.Length > 8)
            {
                uint A = DataArray[2];
                uint B = DataArray[3];

                Sum = 0;
                N = 32;

                while (N-- > 0)
                {
                    A += (B << 4 ^ B >> 5) + B ^ Sum + KeyArray[Sum & 3];
                    Sum += Delta;
                    B += (A << 4 ^ A >> 5) + A ^ Sum + KeyArray[Sum >> 11 & 3];
                }

                DataArray[2] = A;
                DataArray[3] = B;

                Result += DataArray[2].ToString("X") + DataArray[3].ToString("X");
            }

            return Result;
        }
    }
}
