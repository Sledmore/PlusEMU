using System;

namespace Plus.Utilities
{
    public static class HabboEncoding
    {
        /* Encoding Functions */
        #region Encoding/Cipher Methods
        /// <summary>
        /// Encodes the Int32 data given.
        /// </summary>
        /// <param name="v">The raw/decoded data.</param>
        /// <returns>Encoded data.</returns>
        public static string EncodeInt32(int v)
        {
            string t = "";
            return ((t + ((char)(v >> 0x18)) + ((char)(v >> 0x10))) + ((char)(v >> 8)) + ((char)v));
        }

        /// <summary>
        /// Encodes the Short/Int16 data given.
        /// </summary>
        /// <param name="v">The raw/decoded data.</param>
        /// <returns>Encoded data.</returns>
        public static string EncodeInt16(int v)
        {
            string t = "";
            return (t + ((char)(v >> 8)) + ((char)v));
        }
        #endregion

        /* Decoding Functions */
        #region Decoding/DeCipher Methods
        /// <summary>
        /// Decodes the Encoded int32 data.
        /// </summary>
        /// <param name="v">Encoded Data.</param>
        /// <returns>Decoded Data.</returns>
        public static int DecodeInt32(string v)
        {
            if ((((v[0] | v[1]) | v[2]) | v[3]) < 0)
            {
                return -1;
            }
            return ((((v[0] << 0x18) + (v[1] << 0x10)) + (v[2] << 8)) + v[3]);
        }

        /// <summary>
        /// Decodes the Encoded int32 data.
        /// </summary>
        /// <param name="v">Encoded Data.</param>
        /// <returns>Decoded Data.</returns>
        public static int DecodeInt32(byte[] v)
        {
            if ((((v[0] | v[1]) | v[2]) | v[3]) < 0)
            {
                return -1;
            }
            return ((((v[0] << 0x18) + (v[1] << 0x10)) + (v[2] << 8)) + v[3]);
        }

        /// <summary>
        /// Decodes the Encoded short data.
        /// </summary>
        /// <param name="v">Encoded data.</param>
        /// <returns>Decoded data.</returns>
        public static int DecodeInt16(byte[] v)
        {
            if ((v[0] | v[1]) < 0)
            {
                return -1;
            }
            return ((v[0] << 8) + v[1]);
        }


        /// <summary>
        /// This method decodes data in string for short.
        /// </summary>
        /// <param name="v">Encoded data.</param>
        /// <returns>Decoded data.</returns>
        public static short DecodeInt16(string v)
        {
            if ((v[0] | v[1]) < 0)
            {
                return -1;
            }
            return (short)((v[0] << 8) + v[1]);
        }

        public static bool DecodeBool(string v)
        {
            try
            {
                int i = Convert.ToInt32(Convert.ToChar(v.Substring(0, 1)));
                return (i == 1);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
        #endregion
    }
}
