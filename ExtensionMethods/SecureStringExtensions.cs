/*
 * This code is licensed under a Creative Commons Attribution 4.0 International License.
 * See: https://creativecommons.org/licenses/by/4.0/
 * From: https://stackoverflow.com/questions/13600773/how-do-i-encrypt-a-securestring-using-dpapi-for-saving-to-disk-without-first-con#:~:text=The.net%20DPAPI%20class%20is%20the%20ProtectedData%20class%2C%20however%2C,first%20converting%20the%20SecureString%20to%20an%20unsecured%20string
 */

using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;

namespace MobiFlight
{
    /// <summary>Extension methods for <see cref="T:System.Security.SecureString" /> to enable safe serialisation and deserialisation of secure strings. This class cannot be inherited.</summary>
    /// <remarks>The <see cref="M:Protect(SecureString, System.Byte[], DataProtectionScope)" /> and <see cref="M:AppendProtected(SecureString, System.Byte[], System.Byte[], DataProtectionScope)" /> methods can be treated as secure string-based equivalents of <see cref="M:System.Security.Cryptography.ProtectedData.Protect(System.Byte[], System.Byte[], System.Security.Cryptography.DataProtectionScope)" /> and <see cref="M:System.Security.Cryptography.ProtectedData.Unprotect(System.Byte[], System.Byte[], System.Security.Cryptography.DataProtectionScope)" />, respectively.</remarks>
    /// <seealso cref="T:System.Security.Cryptography.ProtectedData" />
    public static class SecureStringExtensions
    {
        /// <summary>Specifies the scope of the data protection to be applied by the <see cref="Protect(SecureString, byte[], DataProtectionScope)" /> and <see cref="AppendProtected(SecureString, byte[], byte[], DataProtectionScope)" /> methods.</summary>
        /// <remarks>This enumeration is equivalent to <see cref="T:System.Security.Cryptography.DataProtectionScope" />.</remarks>
        /// <seealso cref="T:System.Security.Cryptography.DataProtectionScope" />
        public enum DataProtectionScope
        {
            /// <summary>
            /// The protected data is associated with the current user. Only threads running under the current user context can unprotect the data.
            /// </summary>
            CurrentUser,

            /// <summary>
            /// The protected data is associated with the machine context. Any process running on the computer can unprotect data.
            /// This enumeration value is usually used in server-specific applications that run on a server where untrusted users are not allowed access.
            /// </summary>
            LocalMachine
        }

        /// <summary>Encrypts the data in a secure string and returns a byte array that contains the encrypted data.</summary>
        /// <remarks>This method can be treated as equivalent to <see cref="M:System.Security.Cryptography.ProtectedData.Protect(System.Byte[], System.Byte[], System.Security.Cryptography.DataProtectionScope)" />, except that it encrypts a secure string instead of a byte array.</remarks>
        /// <param name="secureString">The secure string.</param>
        /// <param name="optionalEntropy">An optional additional byte array used to increase the complexity of the encryption, or <see langword="null" /> for no additional complexity.</param>
        /// <param name="scope">One of the enumeration values that specifies the scope of encryption.</param>
        /// <returns>A byte array representing the encrypted data.</returns>
        /// <exception cref="T:System.Security.Cryptography.CryptographicException">The encryption failed.</exception>
        /// <exception cref="T:System.OutOfMemoryException">The system ran out of memory while encrypting the data.</exception>
        /// <seealso cref="M:System.Security.Cryptography.ProtectedData.Protect(System.Byte[], System.Byte[], System.Security.Cryptography.DataProtectionScope)" />
        public static byte[] Protect(this SecureString secureString, byte[] optionalEntropy, DataProtectionScope scope)
        {
            byte[] result = null;
            NativeMethods.DATA_BLOB dataIn = null;
            NativeMethods.DATA_BLOB entropy = null;
            NativeMethods.DATA_BLOB dataOut = new NativeMethods.DATA_BLOB();
            GCHandle ptrOptionalEntropy = new GCHandle();

            try
            {
                // +++ Handle secureString
                dataIn = new NativeMethods.DATA_BLOB
                {
                    cbData = secureString.Length * sizeof(char),
                    pbData = Marshal.SecureStringToGlobalAllocUnicode(secureString)
                };
                // ---

                // +++ Handle optionalEntropy
                if (optionalEntropy != null)
                {
                    ptrOptionalEntropy = GCHandle.Alloc(optionalEntropy, GCHandleType.Pinned);

                    entropy = new NativeMethods.DATA_BLOB
                    {
                        cbData = optionalEntropy.Length,
                        pbData = ptrOptionalEntropy.AddrOfPinnedObject()
                    };
                }
                // ---

                // +++ Handle scope
                NativeMethods.CryptProtectFlags flags = NativeMethods.CryptProtectFlags.CRYPTPROTECT_UI_FORBIDDEN;

                if (scope.HasFlag(DataProtectionScope.LocalMachine))
                    flags |= NativeMethods.CryptProtectFlags.CRYPTPROTECT_LOCAL_MACHINE;
                // ---

                if (!NativeMethods.CryptProtectData(dataIn, IntPtr.Zero, entropy, IntPtr.Zero, IntPtr.Zero, flags, dataOut))
                    throw new CryptographicException(Marshal.GetLastWin32Error());
                else
                {
                    if (dataOut.pbData == IntPtr.Zero)
                        throw new OutOfMemoryException();

                    result = new byte[dataOut.cbData];
                    Marshal.Copy(dataOut.pbData, result, 0, dataOut.cbData);
                }
            }
            finally
            {
                if (dataOut.pbData != IntPtr.Zero)
                {
                    NativeMethods.ZeroMemory(dataOut.pbData, (UIntPtr)dataOut.cbData);
                    Marshal.FreeHGlobal(dataOut.pbData);
                }

                if (ptrOptionalEntropy.IsAllocated)
                    ptrOptionalEntropy.Free();

                if (dataIn != null)
                    Marshal.ZeroFreeGlobalAllocUnicode(dataIn.pbData);
            }

            return (result);
        }

        /// <summary>Decrypts the data in a specified byte array and appends it to a secure string.</summary>
        /// <remarks>This method can be treated as equivalent to <see cref="M:System.Security.Cryptography.ProtectedData.Unprotect(System.Byte[], System.Byte[], System.Security.Cryptography.DataProtectionScope)" />, except that it appends the decrypted data to a secure string instead returning it in a byte array.</remarks>
        /// <param name="secureString">The secure string.</param>
        /// <param name="encryptedData">A byte array containing data encrypted using the <see cref="M:Protect(SecureString, System.Byte[], DataProtectionScope)" /> method.</param>
        /// <param name="optionalEntropy">An optional additional byte array that was used to encrypt the data, or <see langword="null" /> if the additional byte array was not used.</param>
        /// <param name="scope">One of the enumeration values that specifies the scope of data protection that was used to encrypt the data.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="encryptedData" /> parameter is <see langword="null" />.</exception>
        /// <exception cref="T:System.InvalidOperationException">The secure string is read only.</exception>
        /// <exception cref="T:System.Security.Cryptography.CryptographicException">The decryption failed.</exception>
        /// <exception cref="T:System.OutOfMemoryException">The system ran out of memory while decrypting the data.</exception>
        /// <seealso cref="M:System.Security.Cryptography.ProtectedData.Unprotect(System.Byte[], System.Byte[], System.Security.Cryptography.DataProtectionScope)" />
        public static void AppendProtected(this SecureString secureString, byte[] encryptedData, byte[] optionalEntropy, DataProtectionScope scope)
        {
            NativeMethods.DATA_BLOB entropy = null;
            NativeMethods.DATA_BLOB dataOut = new NativeMethods.DATA_BLOB();
            GCHandle ptrEncryptedData = new GCHandle();
            GCHandle ptrOptionalEntropy = new GCHandle();

            if (encryptedData == null)
                throw new ArgumentNullException("encryptedData");

            if (encryptedData.IsReadOnly)
                throw new InvalidOperationException();

            try
            {
                // +++ Handle encryptedData
                ptrEncryptedData = GCHandle.Alloc(encryptedData, GCHandleType.Pinned);

                NativeMethods.DATA_BLOB dataIn = new NativeMethods.DATA_BLOB
                {
                    cbData = encryptedData.Length,
                    pbData = ptrEncryptedData.AddrOfPinnedObject()
                };
                // ---

                // +++ Handle optionalEntropy
                if (optionalEntropy != null)
                {
                    ptrOptionalEntropy = GCHandle.Alloc(optionalEntropy, GCHandleType.Pinned);

                    entropy = new NativeMethods.DATA_BLOB
                    {
                        cbData = optionalEntropy.Length,
                        pbData = ptrOptionalEntropy.AddrOfPinnedObject()
                    };
                }
                // ---

                // +++ Handle scope
                NativeMethods.CryptProtectFlags flags = NativeMethods.CryptProtectFlags.CRYPTPROTECT_UI_FORBIDDEN;

                if (scope.HasFlag(DataProtectionScope.LocalMachine))
                    flags |= NativeMethods.CryptProtectFlags.CRYPTPROTECT_LOCAL_MACHINE;
                // ---

                if (!NativeMethods.CryptUnprotectData(dataIn, IntPtr.Zero, entropy, IntPtr.Zero, IntPtr.Zero, flags, dataOut))
                    throw new CryptographicException(Marshal.GetLastWin32Error());
                else
                {
                    if (dataOut.pbData == IntPtr.Zero)
                        throw new OutOfMemoryException();

                    // Sanity check: can't be a valid string if length is not a multiple of sizeof(char)
                    if ((dataOut.cbData % sizeof(char)) != 0)
                        throw new CryptographicException();

                    for (int i = 0; i < dataOut.cbData; i += sizeof(char))
                        secureString.AppendChar((char)Marshal.ReadInt16(dataOut.pbData, i));
                }
            }
            finally
            {
                if (dataOut.pbData != IntPtr.Zero)
                {
                    NativeMethods.ZeroMemory(dataOut.pbData, (UIntPtr)dataOut.cbData);
                    Marshal.FreeHGlobal(dataOut.pbData);
                }

                if (ptrOptionalEntropy.IsAllocated)
                    ptrOptionalEntropy.Free();

                if (ptrEncryptedData.IsAllocated)
                    ptrEncryptedData.Free();
            }
        }

        private static class NativeMethods
        {
            [StructLayout(LayoutKind.Sequential)]
            public class DATA_BLOB
            {
                public int cbData;
                public IntPtr pbData;
            }

            [Flags]
            public enum CryptProtectFlags : uint
            {
                CRYPTPROTECT_UI_FORBIDDEN = 0x01,
                CRYPTPROTECT_LOCAL_MACHINE = 0x04,
                CRYPTPROTECT_VERIFY_PROTECTION = 0x40
            }

            [DllImport("kernel32.dll", EntryPoint = "RtlZeroMemory")]
            public static extern void ZeroMemory(IntPtr destination, UIntPtr length);

            [DllImport("crypt32.dll", SetLastError = true)]
            public static extern bool CryptProtectData(DATA_BLOB pDataIn, IntPtr szDataDescr, DATA_BLOB pOptionalEntropy, IntPtr pvReserved, IntPtr pPromptStruct, CryptProtectFlags dwFlags, DATA_BLOB pDataOut);

            [DllImport("crypt32.dll", SetLastError = true)]
            public static extern bool CryptUnprotectData(DATA_BLOB pDataIn, IntPtr ppszDataDescr, DATA_BLOB pOptionalEntropy, IntPtr pvReserved, IntPtr pPromptStruct, CryptProtectFlags dwFlags, DATA_BLOB pDataOut);
        }
    }
}