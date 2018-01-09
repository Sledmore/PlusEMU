using System;

using Plus.Utilities;

namespace Plus.Communication.Encryption.Crypto.RSA
{
    public class RSAKey
    {
        public int E { get; private set; }
        public BigInteger e { get; private set; }
        public BigInteger N { get; private set; }
        public BigInteger D { get; private set; }
        public BigInteger P { get; private set; }
        public BigInteger Q { get; private set; }
        public BigInteger Dmp1 { get; private set; }
        public BigInteger Dmq1 { get; private set; }
        public BigInteger Coeff { get; private set; }

        protected bool CanDecrypt;
        protected bool CanEncrypt;

        public RSAKey(BigInteger n, int e,
            BigInteger d,
            BigInteger p, BigInteger q,
            BigInteger dmp1, BigInteger dmq1,
            BigInteger coeff)
        {
            E = e;
            this.e = e;
            N = n;
            D = d;
            P = p;
            Q = q;
            Dmp1 = dmp1;
            Dmq1 = dmq1;
            Coeff = coeff;

            CanEncrypt = (N != 0 && E != 0);
            CanDecrypt = (CanEncrypt && D != 0);

            //this.GeneratePair(1024, this.e);
        }

        public void GeneratePair(int b, BigInteger e)
        {
            this.e = e;

            int qs = b >> 1;

            while (true)
            {
                while (true)
                {
                    P = BigInteger.genPseudoPrime(b - qs, 1, new Random());

                    if ((P - 1).gcd(this.e) == 1 && P.isProbablePrime(10))
                    {
                        break;
                    }
                }

                while (true)
                {
                    Q = BigInteger.genPseudoPrime(qs, 1, new Random());

                    if ((Q - 1).gcd(this.e) == 1 && P.isProbablePrime(10))
                    {
                        break;
                    }
                }

                if (P < Q)
                {
                    BigInteger t = P;
                    P = Q;
                    Q = t;
                }

                BigInteger phi = (P - 1) * (Q - 1);
                if (phi.gcd(this.e) == 1)
                {
                    N = P * Q;
                    D = this.e.modInverse(phi);
                    Dmp1 = D % (P - 1);
                    Dmq1 = D % (Q - 1);
                    Coeff = Q.modInverse(P);
                    break;
                }
            }

            CanEncrypt = N != 0 && this.e != 0;
            CanDecrypt = CanEncrypt && D != 0;

            Console.WriteLine(N.ToString(16));
            Console.WriteLine(D.ToString(16));
        }

        public static RSAKey ParsePublicKey(string n, string e)
        {
            return new RSAKey(new BigInteger(n, 16), Convert.ToInt32(e, 16), 0, 0, 0, 0, 0, 0);
        }

        public static RSAKey ParsePrivateKey(string n, string e,
            string d,
            string p = null, string q = null,
            string dmp1 = null, string dmq1 = null,
            string coeff = null)
        {
            if (p == null)
            {
                return new RSAKey(new BigInteger(n, 16), Convert.ToInt32(e, 16), new BigInteger(d, 16), 0, 0, 0, 0, 0);
            }
            else
            {
                return new RSAKey(new BigInteger(n, 16), Convert.ToInt32(e, 16), new BigInteger(d, 16), new BigInteger(p, 16), new BigInteger(q, 16), 
                    new BigInteger(dmp1, 16), new BigInteger(dmq1, 16), new BigInteger(coeff, 16));
            }
        }

        public int GetBlockSize()
        {
            return (N.bitCount() + 7) / 8;
        }

        public byte[] Encrypt(byte[] src)
        {
            return DoEncrypt(new DoCalculateionDelegate(DoPublic), src, Pkcs1PadType.FullByte);
        }

        public byte[] Decrypt(byte[] src)
        {
            return DoDecrypt(new DoCalculateionDelegate(DoPublic), src, Pkcs1PadType.FullByte);
        }

        public byte[] Sign(byte[] src)
        {
            return DoEncrypt(new DoCalculateionDelegate(DoPrivate), src, Pkcs1PadType.FullByte);
        }

        public byte[] Verify(byte[] src)
        {
            return DoDecrypt(new DoCalculateionDelegate(DoPrivate), src, Pkcs1PadType.FullByte);
        }

        private byte[] DoEncrypt(DoCalculateionDelegate method, byte[] src, Pkcs1PadType type)
        {
            try
            {
                int bl = GetBlockSize();

                byte[] paddedBytes = Pkcs1pad(src, bl, type);
                BigInteger m = new BigInteger(paddedBytes);
                if (m == 0)
                {
                    return null;
                }

                BigInteger c = method(m);
                if (c == 0)
                {
                    return null;
                }

                return c.getBytes();
            }
            catch
            {
                return null;
            }
        }

        private byte[] DoDecrypt(DoCalculateionDelegate method, byte[] src, Pkcs1PadType type)
        {
            try
            {
                BigInteger c = new BigInteger(src);
                BigInteger m = method(c);
                if (m == 0)
                {
                    return null;
                }

                int bl = GetBlockSize();

                byte[] bytes = Pkcs1unpad(m.getBytes(), bl, type);

                return bytes;
            }
            catch
            {
                return null;
            }
        }

        private byte[] Pkcs1pad(byte[] src, int n, Pkcs1PadType type)
        {
            byte[] bytes = new byte[n];

            int i = src.Length - 1;
            while (i >= 0 && n > 11)
            {
                bytes[--n] = src[i--];
            }

            bytes[--n] = 0;
            while (n > 2)
            {
                byte x = 0;
                switch (type)
                {
                    case Pkcs1PadType.FullByte: x = 0xFF; break;
                    case Pkcs1PadType.RandomByte: x = Randomizer.NextByte(1, 255); break;
                }
                bytes[--n] = x;
            }

            bytes[--n] = (byte)type;
            bytes[--n] = 0;
            return bytes;
        }

        private byte[] Pkcs1unpad(byte[] src, int n, Pkcs1PadType type)
        {
            int i = 0;
            while (i < src.Length && src[i] == 0)
            {
                ++i;
            }

            if (src.Length - i != n - 1 || src[i] > 2)
            {
                Console.WriteLine("PKCS#1 unpad: i={0}, expected src[i]==[0,1,2], got src[i]={1}", i, src[i].ToString("X"));
                return null;
            }

            ++i;

            while (src[i] != 0)
            {
                if (++i >= src.Length)
                {
                    Console.WriteLine("PKCS#1 unpad: i={0}, src[i-1]!=0 (={1})", i, src[i - 1].ToString("X"));
                }
            }

            byte[] bytes = new byte[src.Length - i - 1];
            for (int p = 0; ++i < src.Length; p++)
            {
                bytes[p] = src[i];
            }

            return bytes;
        }

        protected BigInteger DoPublic(BigInteger m)
        {
            return m.modPow(E, N);
        }

        protected BigInteger DoPrivate(BigInteger m)
        {
            if (P == 0 && Q == 0)
            {
                return m.modPow(D, N);
            }
            else
            {
                return 0;
            }
        }
    }

    public delegate BigInteger DoCalculateionDelegate(BigInteger m);
}
