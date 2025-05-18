using System;
using System.Numerics;
using System.Security.Cryptography;
using Microsoft.Win32;
using System.IO;
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
                tBoxP.Text = Schnorr.BigIntegerToHex(schnorr.p);
                tBoxQ.Text = Schnorr.BigIntegerToHex(schnorr.q);
                tBoxH.Text = Schnorr.BigIntegerToHex(schnorr.h);
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
                tBoxPrivate.Text = Schnorr.BigIntegerToHex(privateKey);
                tBoxPublic.Text = Schnorr.BigIntegerToHex(publicKey);

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
                tBoxE.Text = Schnorr.BigIntegerToHex(signature[0]);
                tBoxY.Text = Schnorr.BigIntegerToHex(signature[1]);
                tBoxLog.Text = "Wiadomość podpisana.";
            }
            catch (Exception ex)
            {
                tBoxLog.Text = "Błąd podpisywania: " + ex.Message;
            }
        }

        private void bSignFile_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    BigInteger[] signature = schnorr.PodpiszPlik(openFileDialog.FileName);
                    string sigFile = openFileDialog.FileName + ".sig";

                    

                    File.WriteAllText(sigFile, Schnorr.BigIntegerToHex(signature[0]) + "\n" + Schnorr.BigIntegerToHex(signature[1]));
                    tBoxLog.Text = $"Plik podpisany. Podpis zapisany w: {sigFile}";
                }
                catch (Exception ex)
                {
                    tBoxLog.Text = "Błąd podpisywania pliku: " + ex.Message;
                }
            }
        }

        private void bVerifyFile_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                string sigPath = filePath.Replace(".sig","");

                if (!File.Exists(sigPath))
                {
                    tBoxLog.Text = "Brak pliku z podpisem (.sig).";
                    return;
                }

                try
                {
                    string[] sigParts = File.ReadAllLines(filePath);
                    if (sigParts.Length < 2)
                    {
                        tBoxLog.Text = "Plik z podpisem jest nieprawidłowy.";
                        return;
                    }

                    BigInteger s1 = Schnorr.HexToBigInteger(sigParts[0]);
                    BigInteger s2 = Schnorr.HexToBigInteger(sigParts[1]);

                    bool result = schnorr.WeryfikujPlik(sigPath, s1, s2);
                    tBoxLog.Text = result ? "Podpis pliku jest poprawny." : "Podpis pliku jest NIEpoprawny.";
                }
                catch (Exception ex)
                {
                    tBoxLog.Text = "Błąd podczas weryfikacji podpisu: " + ex.Message;
                }
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
                BigInteger s1 = Schnorr.HexToBigInteger(tBoxE.Text);
                BigInteger s2 = Schnorr.HexToBigInteger(tBoxY.Text);

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
