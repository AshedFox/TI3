using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Numerics;
using RSA;

namespace RSADigitalSignature
{
    class RSADigitalSignatureGenerator
    {
        BigInteger p;
        BigInteger q;

        BigInteger r;
        BigInteger fi;

        BigInteger e;
        BigInteger d;

        public int MaxKeysLength { get; private set; }
        public BigInteger E { get => e; set => e = value; }
        public BigInteger D { get => d; set => d = value; }
        public BigInteger R { get => r; set => r = value; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numOfKeyBits">Максимальное число бит в числе (по умолчанию 256)</param>
        public RSADigitalSignatureGenerator(int numOfKeyBits = 10)
        {
            SetupKeys(numOfKeyBits);
        }

        public void SetupKeys(int numOfKeyBits)
        {
            MaxKeysLength = numOfKeyBits;
            RandomBigInteger random = new RandomBigInteger();

            p = 0; q = 0; 
            do
            {
                do
                {
                    p = random.NextBigInteger(numOfKeyBits);
                } while (!IsPrime(p) || p < 2);

                do
                {
                    q = random.NextBigInteger(numOfKeyBits);
                } while (!IsPrime(q) || q < 2);
            } while (p == q);

            r = BigInteger.Multiply(p, q);
            fi = BigInteger.Multiply(p - 1, q - 1);

            SetupExp();
        }

        public bool IsPrime(BigInteger number)
        {
            return MillerRabinTest(number, 10); 
        }

       bool MillerRabinTest(BigInteger number, BigInteger roundsCount)
        {
            if (number == 2 || number == 3)
                return true;

            if (number < 2 || number.IsEven)
                return false;

            BigInteger t = number - 1;
            RandomBigInteger random = new RandomBigInteger();


            int s = 0;

            while (t % 2 == 0)
            {
                t /= 2;
                s += 1;
            }

            BigInteger a, x;

            for (int i = 0; i < roundsCount; i++)
            {
                a = random.NextBigInteger(2, number - 2);

                x = BigInteger.ModPow(a, t, number);

                if (x == 1 || x == number - 1)
                    continue;

                for (int r = 1; r < s; r++)
                {
                    x = BigInteger.ModPow(x, 2, number);

                    if (x == 1)
                        return false;
                    if (x == number - 1)
                        break;
                }

                if (x != number - 1)
                    return false;
            }

            return true;
        }

        BigInteger GetHash(string input)
        {
            BigInteger prevHash = 99;
            foreach (var item in input)
            {
                prevHash = BigInteger.ModPow(BigInteger.Add(prevHash, item), 2, r);
            }

            return prevHash;
        }

        public BigInteger GetSignature(string input)
        {
            var hash = GetHash(input); 
            return FastModPower(hash, d, r);
        }

        public bool CheckSignature(string input, BigInteger signature, BigInteger d, BigInteger r)
        {
            return GetHash(input) == FastModPower(signature, d, r);

        }

        void SetupExp()
        {
            RandomBigInteger random = new RandomBigInteger();
            do
            {
                e = random.NextBigInteger(2, fi);
            } while (Euclidex(fi, e).divisor != BigInteger.One);


            d = Euclidex(fi, e).y;
            if (d < 0) d += fi;
        }

        (BigInteger x, BigInteger y, BigInteger divisor) Euclidex(BigInteger a, BigInteger b) 
        {
            BigInteger x0 = 1;
            BigInteger x1 = 0;
            BigInteger y0 = 0;
            BigInteger y1 = 1;

            while (b > 1) {
                q = BigInteger.Divide(a, b);
                BigInteger d = BigInteger.Remainder(a, b);
                BigInteger x2 = x0 - q * x1;
                BigInteger y2 = y0 - q * y1;
                a = b; 
                b = d;
                x0 = x1; 
                x1 = x2;
                y0 = y1; 
                y1 = y2;
            }
            return (x1, y1, b);
        }

        BigInteger FastModPower(BigInteger input, BigInteger power, BigInteger modNum)
        {
            BigInteger result = 1;

            while (power != 0) {
                while (power % 2 == 0) {
                    power /= 2;
                    input = input * input % modNum;
                }
                power--;
                result = result * input % modNum;
            }
            return result;
        }
    }
}
