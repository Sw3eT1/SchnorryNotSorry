using System;
using System.Diagnostics;
using System.IO;
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

        public BigInteger[] PodpiszPlik(string sciezkaPliku)
        {
            if (!File.Exists(sciezkaPliku))
                throw new FileNotFoundException("Plik nie istnieje", sciezkaPliku);

            byte[] fileBytes = File.ReadAllBytes(sciezkaPliku);

            do
            {
                r = GenerateRandomBigInteger(158);
            } while (r < 0 || r > q - 1);

            BigInteger x = BigInteger.ModPow(h, r, p);
            byte[] xBytes = x.ToByteArray();

            byte[] combined = new byte[fileBytes.Length + xBytes.Length];
            Buffer.BlockCopy(fileBytes, 0, combined, 0, fileBytes.Length);
            Buffer.BlockCopy(xBytes, 0, combined, fileBytes.Length, xBytes.Length);

            byte[] hash = sha256.ComputeHash(combined);
            BigInteger S1 = new BigInteger(hash, isUnsigned: true, isBigEndian: false);

            BigInteger S2 = (r + a * S1) % q;

            return new BigInteger[] { S1, S2 };
        }

        public bool WeryfikujPlik(string sciezkaPliku, BigInteger s1, BigInteger s2)
        {
            if (!File.Exists(sciezkaPliku))
                throw new FileNotFoundException("Plik nie istnieje", sciezkaPliku);

            byte[] fileBytes = File.ReadAllBytes(sciezkaPliku);

            BigInteger Z = (BigInteger.ModPow(h, s2, p) * BigInteger.ModPow(v, s1, p)) % p;
            byte[] ZBytes = Z.ToByteArray();

            byte[] combined = new byte[fileBytes.Length + ZBytes.Length];
            Buffer.BlockCopy(fileBytes, 0, combined, 0, fileBytes.Length);
            Buffer.BlockCopy(ZBytes, 0, combined, fileBytes.Length, ZBytes.Length);

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
            data[data.Length - 1] = 0;
            return new BigInteger(data);
        }

        public Boolean IsProbablyPrime(BigInteger value, int witnesses = 10)
        {
            if (value <= 1)
                return false;

            if (witnesses <= 0)
                witnesses = 10;

            BigInteger d = value - 1;
            int s = 0;

            while (d % 2 == 0)
            {
                d /= 2;
                s += 1;
            }

            Byte[] bytes = new Byte[value.ToByteArray().LongLength];
            BigInteger a;

            for (int i = 0; i < witnesses; i++)
            {
                do
                {
                    random.NextBytes(bytes);

                    a = new BigInteger(bytes);
                }
                while (a < 2 || a >= value - 2);

                BigInteger x = BigInteger.ModPow(a, d, value);
                if (x == 1 || x == value - 1)
                    continue;

                for (int r = 1; r < s; r++)
                {
                    x = BigInteger.ModPow(x, 2, value);

                    if (x == 1)
                        return false;
                    if (x == value - 1)
                        break;
                }

                if (x != value - 1)
                    return false;
            }

            return true;
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

        public static string BigIntegerToHex(BigInteger value)
        {
            byte[] bytes = value.ToByteArray();
            Array.Reverse(bytes);
            return string.Join("", bytes.Select(b => b.ToString("x2")));
        }

        public static BigInteger HexToBigInteger(string hexString)
        {
            if (string.IsNullOrEmpty(hexString))
            {
                return BigInteger.Zero;
            }
            if (hexString.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                hexString = hexString.Substring(2);
            }

            if (hexString.Length % 2 != 0)
            {
                hexString = "0" + hexString;
            }

            byte[] bytes = new byte[hexString.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            Array.Reverse(bytes);
            return new BigInteger(bytes);
        }
    }
}
