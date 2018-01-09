using System;

using Plus.Utilities;

namespace Plus.Communication.Encryption.KeyExchange
{
    public class DiffieHellman
    {
        public readonly int BITLENGTH = 32;

        public BigInteger Prime { get; private set; }
        public BigInteger Generator { get; private set; }

        private BigInteger PrivateKey;
        public BigInteger PublicKey { get; private set; }

        public DiffieHellman()
        {
            Initialize();
        }

        public DiffieHellman(int b)
        {
            BITLENGTH = b;

            Initialize();
        }

        public DiffieHellman(BigInteger prime, BigInteger generator)
        {
            Prime = prime;
            Generator = generator;

            Initialize(true);
        }

        private void Initialize(bool ignoreBaseKeys = false)
        {
            PublicKey = 0;

            Random rand = new Random();
            while (PublicKey == 0)
            {
                if (!ignoreBaseKeys)
                {
                    Prime = BigInteger.genPseudoPrime(BITLENGTH, 10, rand);
                    Generator = BigInteger.genPseudoPrime(BITLENGTH, 10, rand);
                }

                byte[] bytes = new byte[BITLENGTH / 8];
                Randomizer.NextBytes(bytes);
                PrivateKey = new BigInteger(bytes);

                if (Generator > Prime)
                {
                    BigInteger temp = Prime;
                    Prime = Generator;
                    Generator = temp;
                }

                PublicKey = Generator.modPow(PrivateKey, Prime);

                if (!ignoreBaseKeys)
                {
                    break;
                }
            }
        }

        public BigInteger CalculateSharedKey(BigInteger m)
        {
            return m.modPow(PrivateKey, Prime);
        }
    }
}

