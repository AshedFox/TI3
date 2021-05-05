using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Numerics;

namespace RSA
{
    class RSACipher
    {
        BigInteger p;
        BigInteger q;

        BigInteger r;
        BigInteger fi;

        BigInteger e;
        BigInteger d;

        public int MaxKeysLength { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numOfKeyBits">Максимальное число бит в числе (по умолчанию 256)</param>
        public RSACipher(int numOfKeyBits = 256)
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


                //BigInteger x = FastModPower(a, t, number); 
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

/*        bool LucasTest(BigInteger number, BigInteger accuracy)
        {

        }*/

        public string Encrypt(string input)
        {
            string result = string.Empty;

            for(int i = 0; i < input.Length; i++)
            {
                string buff = FastModPower(input[i], e, r).ToString();
                while (buff.Length < r.ToString().Length)
                {
                    buff = '0' + buff;
                }
                result += buff;
            }

            return result;
        }

        public (int resultCode, string output) Decrypt(string input)
        {
            string result = string.Empty;
            int resultCode = 0;
            try
            {
                int maxLength = r.ToString().Length;
                int count = (int)Math.Round((double)input.Length / r.ToString().Length);

                string[] buffs = new string[count];
                for (int i = 0; i < count; i++)
                {
                    if ((i + 1) * maxLength <= input.Length)
                    {
                        buffs[i] = input.Substring(i * maxLength, maxLength);
                    }
                    else
                    {
                        buffs[i] = input.Substring(i * maxLength, input.Length - i * maxLength);
                    }
                }


                foreach (string item in buffs)
                {
                    result += Convert.ToChar((int)FastModPower(BigInteger.Parse(item), d, r));
                }
            }
            catch
            {
                resultCode = -1;
            }

            return (resultCode, result);
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
