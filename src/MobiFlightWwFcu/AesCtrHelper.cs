using System;
using System.Security.Cryptography;

namespace MobiFlightWwFcu
{
    internal class AesCtrHelper
    {

        public byte[] Encrypt(byte[] input, byte[] key, byte[] nonce)
        {
            return Transform(input, key, nonce);
        }

        public byte[] Decrypt(byte[] input, byte[] key, byte[] nonce)
        {
            // Same operation as encryption in CTR mode
            return Transform(input, key, nonce);
        }

        private byte[] Transform(byte[] input, byte[] key, byte[] nonce)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.None;
                aes.Key = key;

                int blockSize = aes.BlockSize / 8;
                byte[] counter = (byte[])nonce.Clone();
                byte[] keystream = new byte[blockSize];
                byte[] output = new byte[input.Length];

                using (ICryptoTransform encryptor = aes.CreateEncryptor())
                {
                    for (int i = 0; i < input.Length; i += blockSize)
                    {
                        encryptor.TransformBlock(counter, 0, blockSize, keystream, 0);

                        int blockLen = Math.Min(blockSize, input.Length - i);
                        for (int j = 0; j < blockLen; j++)
                        {
                            output[i + j] = (byte)(input[i + j] ^ keystream[j]);
                        }

                        // Increment counter (little-endian)
                        for (int j = counter.Length - 1; j >= 0; j--)
                        {
                            if (++counter[j] != 0) break;
                        }
                    }
                }

                return output;
            }
        }
    }
}
