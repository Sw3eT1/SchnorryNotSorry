using System;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace Schnorrek
{
    public partial class MainWindow : Window
    {
        Schnorr schnorr;

        public MainWindow()
        {
            InitializeComponent();
            schnorr = new Schnorr();
        }

        private void bGeneratePQH_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
                schnorr.GenerateKey();
                tBoxP.Text = schnorr.p.ToString();
                tBoxQ.Text = schnorr.q.ToString();
                tBoxH.Text = schnorr.h.ToString();
                tBoxA.Text = schnorr.a.ToString();
                tBoxLog.Text = "Parametry p, q, h oraz a wygenerowane.";
            //}
            //catch (Exception ex)
            //{
            //    tBoxLog.Text = "Błąd generowania parametrów: " + ex.Message;
            //}
        }

        private void bGenerateKeys_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BigInteger privateKey = schnorr.a;
                BigInteger publicKey = schnorr.v;

                // Pokazuj również surowy klucz prywatny w tBoxA
                tBoxA.Text = privateKey.ToString();

                // Wyświetl hashe kluczy
                tBoxPrivate.Text = GetSha512Hex(privateKey.ToString());
                tBoxPublic.Text = GetSha512Hex(publicKey.ToString());

                tBoxLog.Text = "Klucze wygenerowane i zahashowane SHA-512.";
            }
            catch (Exception ex)
            {
                tBoxLog.Text = "Błąd generowania kluczy: " + ex.Message;
            }
        }


        private void bSignMessage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string message = tBoxMessage.Text;
                if (string.IsNullOrWhiteSpace(message))
                {
                    tBoxLog.Text = "Wprowadź wiadomość do podpisania.";
                    return;
                }

                BigInteger[] signature = schnorr.podpisuj(message);
                tBoxE.Text = signature[0].ToString();
                tBoxY.Text = signature[1].ToString();
                tBoxLog.Text = "Wiadomość podpisana.";
            }
            catch (Exception ex)
            {
                tBoxLog.Text = "Błąd podpisywania: " + ex.Message;
            }
        }

        private void bVerifySignature_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string message = tBoxMessage.Text;
                if (string.IsNullOrWhiteSpace(message))
                {
                    tBoxLog.Text = "Wprowadź wiadomość do weryfikacji.";
                    return;
                }

                if (string.IsNullOrEmpty(tBoxE.Text) || string.IsNullOrEmpty(tBoxY.Text))
                {
                    tBoxLog.Text = "Podpis (e, y) jest pusty.";
                    return;
                }

                BigInteger s1 = BigInteger.Parse(tBoxE.Text);
                BigInteger s2 = BigInteger.Parse(tBoxY.Text);

                bool valid = schnorr.weryfikujString(message, $"{s1}\n{s2}");
                tBoxLog.Text = valid ? "Podpis jest poprawny." : "Podpis jest NIEpoprawny.";
            }
            catch (Exception ex)
            {
                tBoxLog.Text = "Błąd podczas weryfikacji: " + ex.Message;
            }
        }
        private string GetSha512Hex(string input)
        {
            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha512.ComputeHash(inputBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "");
            }
        }

    }
}
