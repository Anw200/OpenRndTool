using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace OpenRndTool
{
    public partial class Form1 : Form
    {
        private int kiloBytes = 0;

        public Form1()
        {
            InitializeComponent();
            AskFileSize();
        }

        private void AskFileSize()
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox(
                "How many kilobytes must the random file be?",
                "Random File Size",
                "64");

            if (!int.TryParse(input, out kiloBytes) || kiloBytes <= 0)
            {
                MessageBox.Show("Invalid size. Using 64 KB.");
                kiloBytes = 64;
            }
        }

        private async void btnStdRandom_Click(object sender, EventArgs e)
        {
            txtSha512.Enabled = true;
            txtSha512.ReadOnly = false;
            
            string secret = txtInput.Text;
            long totalBytes = kiloBytes * 1024L;
            string filename = $"Rnd{DateTime.Now:MMddHHmm}.txt";

            // First SHA-512 of the secret
            byte[] sha = SHA512.HashData(Encoding.UTF8.GetBytes(secret));

            // Mix secret with SHA-512 to get deterministic 64-byte seed
            byte[] mixedSeed = MixSecretWithSha512(secret, sha);
            string mixedHex = BytesToFormattedHex(mixedSeed);
            txtSha512.Text = mixedHex;


            byte[] previewBlock = await Task.Run(() =>
            {
                return GenerateStdRandomStream(mixedSeed, totalBytes, filename);
            });

            // *** WRITE FILENAME FIRST ***
            txtOutput.Text += $"\r\n\r\n.Net random function based file: {filename}\r\n";

            // *** THEN WRITE PREVIEW BLOCK ***
            string previewHex = BytesToFormattedHex(previewBlock);
            txtOutput.Text += "Preview of first 64 bytes:\r\n" + previewHex;

            //calculate the SHA256 of the generated file and display it
            string fileSha256 = ComputeFileSha256(filename);
            txtOutput.Text += "\r\nSHA-256 of generated file:\r\n" + fileSha256;
        }


        private async void btnSha512Random_Click(object sender, EventArgs e)
        {
            txtSha512.Enabled = true;
            txtSha512.ReadOnly = false;

            string secret = txtInput.Text;
            long totalBytes = kiloBytes * 1024L;
            string filename = $"Rnd{DateTime.Now:MMddHHmm}.txt";

            // First SHA-512 of the secret
            byte[] sha = SHA512.HashData(Encoding.UTF8.GetBytes(secret));

            // Mix secret with SHA-512 to get deterministic 64-byte seed
            byte[] mixedSeed = MixSecretWithSha512(secret, sha);
            string mixedHex = BytesToFormattedHex(mixedSeed);
            txtSha512.Text = mixedHex;


            byte[] previewBlock = await Task.Run(() =>
            {
                return GenerateSha512Stream(mixedSeed, totalBytes, filename);
            });

            // *** WRITE FILENAME FIRST ***
            txtOutput.Text += $"\r\n\r\nSha512 calculated random file: {filename}\r\n";

            // *** THEN WRITE PREVIEW BLOCK ***
            string previewHex = BytesToFormattedHex(previewBlock);
            txtOutput.Text += "Preview of first 64 bytes:\r\n" + previewHex;

            //calculate the SHA256 of the generated file and display it
            string fileSha256 = ComputeFileSha256(filename);
            txtOutput.Text += "\r\nSHA-256 of generated file:\r\n" + fileSha256;
        }




        //generate file directly without holding all data in memory
        private static byte[] GenerateSha512Stream(byte[] mixedSeed, long totalBytes, string filename)
        {
            byte[] firstBlock = new byte[64];
            bool firstBlockCaptured = false;


            byte[] state = mixedSeed.ToArray(); // start from mixed seed

            using FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
            using StreamWriter writer = new StreamWriter(fs, Encoding.ASCII);

            const int chunkSize = 1 << 20; // 1 MB
            byte[] buffer = new byte[chunkSize];

            long bytesRemaining = totalBytes;
            int hexCount = 0;

          

            while (bytesRemaining > 0)
            {
                int offset = 0;

                // Expand SHA-512 state into buffer
                while (offset < chunkSize && bytesRemaining > 0)
                {
                    state = SHA512.HashData(state);

                    int copy = Math.Min(64, chunkSize - offset);
                    copy = (int)Math.Min(copy, bytesRemaining);

                    Array.Copy(state, 0, buffer, offset, copy);
                    offset += copy;
                    bytesRemaining -= copy;
                }
                // Convert to hex with newline every 1024 bytes (2048 hex chars)
                for (int i = 0; i < offset; i++)
                 
                {
                    // Capture FIRST 64 bytes of output
                    if (!firstBlockCaptured && i < 64)
                        firstBlock[i] = buffer[i];


                    writer.Write(buffer[i].ToString("X2"));
                    hexCount++;

                    if (hexCount == 1024)
                    {
                        writer.WriteLine();
                        hexCount = 0;
                    }
                }
                // After first iteration, preview is captured
                firstBlockCaptured = true;
            }
            return firstBlock;
        }

        //generate file directly without holding all data in memory
       private static byte[] GenerateStdRandomStream(byte[] mixedSeed, long totalBytes, string filename)
        {
            byte[] firstBlock = new byte[64];
            bool firstBlockCaptured = false;
            
            int intSeed = BitConverter.ToInt32(mixedSeed, 0);
            Random rnd = new Random(intSeed);

            using FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
            using StreamWriter writer = new StreamWriter(fs, Encoding.ASCII);

            const int chunkSize = 1 << 20; // 1 MB
            byte[] buffer = new byte[chunkSize];

            long bytesRemaining = totalBytes;
            int hexCount = 0;

            while (bytesRemaining > 0)
            {
                int toGen = (int)Math.Min(chunkSize, bytesRemaining);
                rnd.NextBytes(buffer);

                for (int i = 0; i < toGen; i++)
                {
                    // Capture FIRST 64 bytes of output
                    if (!firstBlockCaptured && i < 64)
                        firstBlock[i] = buffer[i];


                    writer.Write(buffer[i].ToString("X2"));
                    hexCount++;

                    if (hexCount == 1024)
                    {
                        writer.WriteLine();
                        hexCount = 0;
                    }
                }

                bytesRemaining -= toGen;
            }
            return firstBlock;
        }

        //make the sha value more random by mixing it with the secret
        private static byte[] MixSecretWithSha512(string secret, byte[] sha512)
        {
            if (sha512.Length != 64)
                throw new ArgumentException("SHA-512 hash must be 64 bytes.");

            byte[] secretBytes = Encoding.UTF8.GetBytes(secret);

            // Step 1: Expand or fold secret to exactly 64 bytes
            byte[] expanded = new byte[64];

            if (secretBytes.Length == 0)
            {
                // Empty secret ? treat as zeros
                return sha512.ToArray();
            }

            if (secretBytes.Length < 64)
            {
                // Repeat secret until 64 bytes
                int pos = 0;
                while (pos < 64)
                {
                    int copy = Math.Min(secretBytes.Length, 64 - pos);
                    Array.Copy(secretBytes, 0, expanded, pos, copy);
                    pos += copy;
                }
            }
            else
            {
                // Fold long secrets by XORing 64-byte blocks
                for (int i = 0; i < secretBytes.Length; i++)
                {
                    expanded[i % 64] ^= secretBytes[i];
                }
            }

            // Step 2: XOR expanded secret with SHA-512 hash
            byte[] mixed = new byte[64];
            for (int i = 0; i < 64; i++)
            {
                mixed[i] = (byte)(expanded[i] ^ sha512[i]);
            }


            return mixed;
        }


        private string BytesToFormattedHex(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 2);
            int count = 0;

            foreach (byte b in data)
            {
                sb.Append(b.ToString("X2"));
                count++;

                if (count == 1024)
                {
                    sb.AppendLine();
                    count = 0;
                }
            }

            return sb.ToString();
        }

        private static string ComputeFileSha256(string filename)
        {
            using var sha = SHA256.Create();
            using var stream = File.OpenRead(filename);
            byte[] hash = sha.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "");
        }


    }
}
