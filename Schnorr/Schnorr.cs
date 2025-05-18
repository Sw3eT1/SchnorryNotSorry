using System;
using System.Diagnostics;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Media;

namespace Schnorrek
{
    public class Schnorr
    {
        public BigInteger p;
        public BigInteger q;
        public BigInteger h;
        public BigInteger a;
        public BigInteger v;
        public BigInteger r;
        public Random random;
        private SHA256 sha256;

        public Schnorr()
        {
            random = new Random();
            sha256 = SHA256.Create();
        }

        public void GenerateKey()
        {
            q = GeneratePrime(140);
            do
            {
                p = GeneratePrime(512);
                var pMinus1 = p - 1;
                p = p - (pMinus1 % q);
            } while (!IsProbablyPrime(p, 2));

            Trace.WriteLine((p-1)/q);
            h = GenerateRandomBigInteger(512);
            h = BigInteger.ModPow(h, (p - 1) / q, p);
            Trace.WriteLine(BigInteger.ModPow(h, q, p));

            do
            {
                a = GenerateRandomBigInteger(140);
            } while (a <= 1 || a > p-1);

            v = BigInteger.ModPow(h, a, p);
            v = ModInverse(v, p);
            Trace.WriteLine(BigInteger.ModPow(h,a,p) * v%p);
        }

        public BigInteger[] podpisuj(string tekst)
        {
            do
            {
                r = GenerateRandomBigInteger(158);
            } while (r < 0 || r > q - 1);
            BigInteger x = BigInteger.ModPow(h, r, p);

            byte[] messageBytes = Encoding.UTF8.GetBytes(tekst);
            byte[] xBytes = x.ToByteArray();

            byte[] combined = new byte[messageBytes.Length + xBytes.Length];
            Buffer.BlockCopy(messageBytes, 0, combined, 0, messageBytes.Length);
            Buffer.BlockCopy(xBytes, 0, combined, messageBytes.Length, xBytes.Length);

            byte[] hash = sha256.ComputeHash(combined);
            BigInteger S1 = new BigInteger(hash, isUnsigned: true, isBigEndian: false);

            BigInteger S2 = (r + a * S1) % q;

            return new BigInteger[] { S1, S2 };
        }

        public bool weryfikujString(string tekstJawny, string podpis)
        {
            var parts = podpis.Split('\n');
            if (parts.Length < 2)
                return false;

            BigInteger s1 = BigInteger.Parse(parts[0]);
            BigInteger s2 = BigInteger.Parse(parts[1]);

            BigInteger Z = (BigInteger.ModPow(h, s2, p) * BigInteger.ModPow(v, s1, p)) % p;

            byte[] messageBytes = Encoding.UTF8.GetBytes(tekstJawny);
            byte[] ZBytes = Z.ToByteArray();

            byte[] combined = new byte[messageBytes.Length + ZBytes.Length];
            Buffer.BlockCopy(messageBytes, 0, combined, 0, messageBytes.Length);
            Buffer.BlockCopy(ZBytes, 0, combined, messageBytes.Length, ZBytes.Length);

            byte[] hash = sha256.ComputeHash(combined);
            BigInteger hashBigInt = new BigInteger(hash, isUnsigned: true, isBigEndian: false);

            return hashBigInt == s1;
        }

        private BigInteger GeneratePrime(int bits)
        {
            while (true)
            {
                BigInteger candidate = GenerateRandomBigInteger(bits);
                if (IsProbablyPrime(candidate, 10))
                    return candidate;
            }
        }

        private BigInteger GenerateRandomBigInteger(int bits)
        {
            int bytes = bits / 8;
            byte[] data = new byte[bytes + 1];
            random.NextBytes(data);
            data[data.Length - 1] = 0; // aby liczba była nieujemna
            return new BigInteger(data);
        }

        public bool IsProbablyPrime(BigInteger value, int witnesses = 10)
        {
            if (value <= 1)
                return false;
            if (value == 2 || value == 3)
                return true;
            if (value.IsEven)
                return false;

            // Dekompozycja: value - 1 = 2^s * d
            BigInteger d = value - 1;
            int s = 0;
            while ((d & 1) == 0)
            {
                d >>= 1;
                s++;
            }

            // Bufor do losowania (wydajniejsze niż nowe byte[] co iterację)
            int byteLength = value.GetByteCount();
            byte[] buffer = new byte[byteLength];

            using var rng = RandomNumberGenerator.Create();

            for (int i = 0; i < witnesses; i++)
            {
                BigInteger a;
                do
                {
                    rng.GetBytes(buffer);
                    buffer[^1] &= 0x7F; // zapewnij, że liczba będzie dodatnia
                    a = new BigInteger(buffer);
                } while (a < 2 || a >= value - 2);

                BigInteger x = BigInteger.ModPow(a, d, value);
                if (x == 1 || x == value - 1)
                    continue;

                bool found = false;
                for (int r = 1; r < s; r++)
                {
                    x = BigInteger.ModPow(x, 2, value);

                    if (x == 1)
                        return false;
                    if (x == value - 1)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                    return false;
            }

            return true;
        }



        private BigInteger GenerateRandomBigIntegerInRange(BigInteger min, BigInteger max)
        {
            BigInteger diff = max - min;
            int bytes = diff.ToByteArray().Length;
            byte[] data = new byte[bytes];
            BigInteger result;
            do
            {
                random.NextBytes(data);
                data[data.Length - 1] &= 0x7F; //bez ujemniaków
                result = new BigInteger(data);
            } while (result > diff || result < 0);
            return result + min;
        }

        private BigInteger ModInverse(BigInteger a, BigInteger modulus)
        {
            BigInteger m0 = modulus, t, q;
            BigInteger x0 = 0, x1 = 1;

            if (modulus == 1)
                return 0;

            while (a > 1)
            {
                q = a / modulus;
                t = modulus;

                modulus = a % modulus;
                a = t;
                t = x0;

                x0 = x1 - q * x0;
                x1 = t;
            }

            if (x1 < 0)
                x1 += m0;

            return x1;
        }
    }
}
