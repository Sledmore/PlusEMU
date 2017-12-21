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
            this.E = e;
            this.e = e;
            this.N = n;
            this.D = d;
            this.P = p;
            this.Q = q;
            this.Dmp1 = dmp1;
            this.Dmq1 = dmq1;
            this.Coeff = coeff;

            this.CanEncrypt = (this.N != 0 && this.E != 0);
            this.CanDecrypt = (this.CanEncrypt && this.D != 0);

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
                    this.P = BigInteger.genPseudoPrime(b - qs, 1, new Random());

                    if ((this.P - 1).gcd(this.e) == 1 && this.P.isProbablePrime(10))
                    {
                        break;
                    }
                }

                while (true)
                {
                    this.Q = BigInteger.genPseudoPrime(qs, 1, new Random());

                    if ((this.Q - 1).gcd(this.e) == 1 && this.P.isProbablePrime(10))
                    {
                        break;
                    }
                }

                if (this.P < this.Q)
                {
                    BigInteger t = this.P;
                    this.P = this.Q;
                    this.Q = t;
                }

                BigInteger phi = (this.P - 1) * (this.Q - 1);
                if (phi.gcd(this.e) == 1)
                {
                    this.N = this.P * this.Q;
                    this.D = this.e.modInverse(phi);
                    this.Dmp1 = this.D % (this.P - 1);
                    this.Dmq1 = this.D % (this.Q - 1);
                    this.Coeff = this.Q.modInverse(this.P);
                    break;
                }
            }

            this.CanEncrypt = this.N != 0 && this.e != 0;
            this.CanDecrypt = this.CanEncrypt && this.D != 0;

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
            return (this.N.bitCount() + 7) / 8;
        }

        public byte[] Encrypt(byte[] src)
        {
            return this.DoEncrypt(new DoCalculateionDelegate(this.DoPublic), src, Pkcs1PadType.FullByte);
        }

        public byte[] Decrypt(byte[] src)
        {
            return this.DoDecrypt(new DoCalculateionDelegate(this.DoPublic), src, Pkcs1PadType.FullByte);
        }

        public byte[] Sign(byte[] src)
        {
            return this.DoEncrypt(new DoCalculateionDelegate(this.DoPrivate), src, Pkcs1PadType.FullByte);
        }

        public byte[] Verify(byte[] src)
        {
            return this.DoDecrypt(new DoCalculateionDelegate(this.DoPrivate), src, Pkcs1PadType.FullByte);
        }

        private byte[] DoEncrypt(DoCalculateionDelegate method, byte[] src, Pkcs1PadType type)
        {
            try
            {
                int bl = this.GetBlockSize();

                byte[] paddedBytes = this.Pkcs1pad(src, bl, type);
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

                int bl = this.GetBlockSize();

                byte[] bytes = this.Pkcs1unpad(m.getBytes(), bl, type);

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
            return m.modPow(this.E, this.N);
        }

        protected BigInteger DoPrivate(BigInteger m)
        {
            if (this.P == 0 && this.Q == 0)
            {
                return m.modPow(this.D, this.N);
            }
            else
            {
                return 0;
            }
        }
    }

    public delegate BigInteger DoCalculateionDelegate(BigInteger m);
}
