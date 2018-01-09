using System;

namespace Plus.Utilities
{
    public static class Converter
    {
        public static string BytesToHexString(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", string.Empty);
        }

        public static byte[] HexStringToBytes(string characters)
        {
            int length = characters.Length;
            byte[] bytes = new byte[length / 2];
            for (int i = 0; i < length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(characters.Substring(i, 2), 16);
            }
            return bytes;
        }
    }
}
