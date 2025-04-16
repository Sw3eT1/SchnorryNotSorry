using System.Numerics;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

namespace SchnorryNotSorry
{
    public partial class Form1 : Form
    {
        private void Form1_Load(object sender, EventArgs e)
        {
            AllocConsole();
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();
        public Form1()
        {
            InitializeComponent();
        }

        private async void bGGeneration_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Rozpoczynanie generowania wartości...");
            bGGeneration.Enabled = false;
            pBarGenerating.Style = ProgressBarStyle.Marquee; // animacja "ładowania"
            pBarGenerating.MarqueeAnimationSpeed = 30;

            try
            {
                BigInteger q = BigInteger.Zero;
                BigInteger p = BigInteger.Zero;
                BigInteger h = BigInteger.Zero;

                // Uruchamiamy proces generowania wartości w oddzielnym wątku
                await Task.Run(() =>
                {
                    // Generowanie q
                    q = GenerateQ();
                    Console.WriteLine($"Wygenerowane q: {q}");

                    // Generowanie p: p = kq + 1 i p jest pierwsze
                    p = GeneratePrimeP(q);
                    Console.WriteLine($"Wygenerowane p: {p}");

                    // Generowanie h: h ≠ 1 i h^q ≡ 1 mod p
                    h = GenerateH(p, q);
                    Console.WriteLine($"Wygenerowane h: {h}");
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Coś poszło nie tak: {ex.Message}");
            }
            finally
            {
                bGGeneration.Enabled = true;
                pBarGenerating.Style = ProgressBarStyle.Blocks; // reset
            }
        }


        //private async void bGGeneration_Click(object sender, EventArgs e)
        //{
        //    Console.WriteLine("Gowno jebane");
        //    bGGeneration.Enabled = false;
        //    pBarGenerating.Style = ProgressBarStyle.Marquee; // animacja "ładowania"
        //    pBarGenerating.MarqueeAnimationSpeed = 30;

        //    try
        //    {
        //        BigInteger q = BigInteger.Zero;
        //        BigInteger p = BigInteger.Zero;
        //        BigInteger h = BigInteger.Zero;

        //        // Uruchamiamy proces generowania wartości w oddzielnym wątku
        //        await Task.Run(() =>
        //        {
        //            // Generowanie q
        //            q = GenerateQ();
        //            Console.WriteLine($"Generated q: {q}");

        //            // Generowanie p: p = kq + 1 i p jest pierwsze
        //            p = GeneratePrimeP(q);
        //            Console.WriteLine($"Generated p: {p}");

        //            // Generowanie h: h ≠ 1 i h^q ≡ 1 mod p
        //            h = GenerateH(p, q);
        //            Console.WriteLine($"Generated h: {h}");
        //        });

        //        // Po wszystkim — aktualizujemy UI
        //        tBoxPValue.Invoke((Action)(() => tBoxPValue.Text = p.ToString()));
        //        tBoxGGenerated.Invoke((Action)(() => tBoxGGenerated.Text = q.ToString()));
        //        tBoxHValue.Invoke((Action)(() => tBoxHValue.Text = h.ToString()));
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Coś poszło nie tak:\n{ex.Message}");
        //    }
        //    finally
        //    {
        //        bGGeneration.Enabled = true;
        //        pBarGenerating.Style = ProgressBarStyle.Blocks; // reset
        //    }
        //}



        public static bool IsProbablyPrime(BigInteger n, int rounds = 5)
        {
            if (n < 2) return false;
            if (n == 2 || n == 3) return true;
            if (n % 2 == 0) return false;

            BigInteger d = n - 1;
            int s = 0;

            while (d % 2 == 0)
            {
                d /= 2;
                s++;
            }

            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] bytes = new byte[256];  // Zmniejszamy długość bajtów dla szybszego losowania

            for (int i = 0; i < rounds; i++)
            {
                BigInteger a;
                do
                {
                    rng.GetBytes(bytes);
                    a = new BigInteger(bytes);
                } while (a < 2 || a >= n - 2);

                BigInteger x = BigInteger.ModPow(a, d, n);
                if (x == 1 || x == n - 1)
                    continue;

                bool continueOuter = false;
                for (int r = 0; r < s - 1; r++)
                {
                    x = BigInteger.ModPow(x, 2, n);
                    if (x == 1) return false;
                    if (x == n - 1)
                    {
                        continueOuter = true;
                        break;
                    }
                }

                if (continueOuter)
                    continue;

                return false;
            }

            return true;
        }

        public static BigInteger GenerateQ()
        {
            BigInteger minValue = BigInteger.One << 140;  // 2^140
            BigInteger maxValue = BigInteger.One << 141;  // 2^141

            Random rng = new Random(); // Generator liczb losowych
            byte[] bytes = new byte[141 / 8 + 1]; // Tablica bajtów, wystarczająca do wygenerowania liczby w zakresie 2^141

            // Wygeneruj losowe bajty
            rng.NextBytes(bytes);

            // Utwórz BigInteger z tych bajtów
            BigInteger q = new BigInteger(bytes);

            // Upewnij się, że liczba mieści się w przedziale
            q = BigInteger.Abs(q % (maxValue - minValue)) + minValue;

            Console.WriteLine($"Generated q inside method: {q}");  // Debug log

            return q;
        }




        public static BigInteger GeneratePrimeP(BigInteger q)
        {
            BigInteger minP = BigInteger.One << 512;
            BigInteger k =  BigInteger.Divide(minP - BigInteger.One, q + BigInteger.One);
            //BigInteger k = (minP - 1) / q + 1;
            BigInteger p;

            while (true)
            {
                p = BigInteger.Multiply(k ,q) + BigInteger.One;
                if (IsProbablyPrime(p, 5))  // Zmniejszono liczbę rund w testach pierwszości
                    return p;

                k++;
            }
        }

        public static BigInteger GenerateH(BigInteger p, BigInteger q)
        {
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] bytes = new byte[p.ToByteArray().LongLength];
            BigInteger h = 1;

            BigInteger exponent = (p - 1) / q;

            while (h == 1)
            {
                BigInteger g;
                do
                {
                    rng.GetBytes(bytes);
                    g = new BigInteger(bytes);
                    g = BigInteger.Abs(g % (p - 3)) + 2;
                } while (g <= 1 || g >= p - 1);

                h = BigInteger.ModPow(g, exponent, p);
            }

            return h;
        }
    }

}
