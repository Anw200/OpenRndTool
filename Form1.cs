using System;
using System.ComponentModel.Design;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;

namespace OpenRndTool
{
    public partial class Form1 : Form
    {
        private long kiloBytes = 0;
        private long bytesperRow = 256; // default bytes per row

        public static long rowCount = 1; // static to be accessible from generator methods, tracks number of rows written to file

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load_1(object sender, EventArgs e)
        {
            AskFileSize();
            AskbytesRow();
        }

        private void AskFileSize()
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox(
                "How many kilobytes or random data must be generated from the secret?",
                "Random File Size",
                "64");

            if (!long.TryParse(input, out kiloBytes) || kiloBytes <= 0)
            {
                MessageBox.Show("Invalid size. Using 64 KB.");
                kiloBytes = 64;
            }
            txtOutput.Text += $"\r\nFile size: {kiloBytes} KB ({kiloBytes * 1024} bytes)\r\n";
            txtOutput.SelectionStart = txtOutput.Text.Length;
            txtOutput.ScrollToCaret();
        }
        private void AskbytesRow()
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox(
                "How many Bytes in each row, max 65536 (0 indicate for no line breaks)?",
                "Bytes per Row",
                "256");

            if (!long.TryParse(input, out bytesperRow) || bytesperRow < 0 || bytesperRow > 65536)
            {
                MessageBox.Show("Invalid size. Using 256.");
                bytesperRow = 256;
            }
            txtOutput.Text += $"\r\nBytes per row: {bytesperRow}\r\n";
            txtOutput.SelectionStart = txtOutput.Text.Length;
            txtOutput.ScrollToCaret();

        }
        private void btnRest_Click(object sender, EventArgs e)
        {
            AskFileSize();
            AskbytesRow();

        }

        private async void btnStdRandom_Click(object sender, EventArgs e)
        {

            string secret = txtInput.Text;
            long totalBytes = kiloBytes * 1024L;
            string filename = $"Rnd{DateTime.Now:HHmm}-{kiloBytes}k x{bytesperRow}.txt";

            // calculate and show the SHA of the secret, user can store all info in output box with no risk and can see if the results are correct
            byte[] sha = SHA512.HashData(Encoding.UTF8.GetBytes(secret));
            txtOutput.Text += $"\r\n\r\n******************************************************************\r\n";
            txtOutput.Text += $"Start generating new file from secret with a SHA512 value of: \r\n{BytesToFormattedHex(sha)}\r\n";

            // First SHA-512 of the secret
            byte[] previewBlock = await Task.Run(() =>
            {
                rowCount = 0; // reset row count for new file generation
                return GenerateStdRandomStream(totalBytes, filename, bytesperRow, secret);
            });

            // *** WRITE FILENAME FIRST ***
            txtOutput.Text += $"\r\n\r\nNew random function file: \r\n{filename}\r\n";

            // *** THEN WRITE PREVIEW BLOCK ***
            string previewHex = BytesToFormattedHex(previewBlock);
            txtOutput.Text += "Preview of first 64 bytes:\r\n" + previewHex;

            //calculate the SHA256 of the generated file and display it
            string fileSha256 = ComputeFileSha256(filename);
            txtOutput.Text += "\r\nSHA-256 of generated file:\r\n" + fileSha256;
        }


        private async void btnSha512Random_Click(object sender, EventArgs e)
        {

            string secret = txtInput.Text;
            long totalBytes = kiloBytes * 1024L;
            string filename = $"ShaRnd{DateTime.Now:HHmm}-{kiloBytes}k x{bytesperRow}.txt";

            // calculate and show the SHA of the secret, user can store all info in output box with no risk and can see if the results are correct
            byte[] sha = SHA512.HashData(Encoding.UTF8.GetBytes(secret));

            txtOutput.Text += $"\r\n\r\n******************************************************************\r\n";
            txtOutput.Text += $"Start generating new file from secret with a SHA512 value of: \r\n{BytesToFormattedHex(sha)}\r\n";



            byte[] previewBlock = await Task.Run(() =>
            {
                rowCount = 1; // reset row count for new file generation
                return GenerateSha512Stream(totalBytes, filename, bytesperRow, secret);
            });

            // *** WRITE FILENAME FIRST ***
            txtOutput.Text += $"\r\nNew SHA512 function based file: \r\n{filename} with {Form1.rowCount} rows\r\n";

            // *** THEN WRITE PREVIEW BLOCK ***
            string previewHex = BytesToFormattedHex(previewBlock);
            txtOutput.Text += "\r\nPreview of first 64 bytes:\r\n" + previewHex + "\r\n";

            //calculate the SHA256 of the generated file and display it
            string fileSha256 = ComputeFileSha256(filename);
            txtOutput.Text += "\r\nSHA-256 of the file:\r\n" + fileSha256 + "\r\n";
            txtOutput.SelectionStart = txtOutput.Text.Length;
            txtOutput.ScrollToCaret();
        }




        //generate file directly without holding all data in memory
        private static byte[] GenerateSha512Stream(long totalBytes, string filename, long rowBytes, string secret)
        {
            byte[] firstBlock = new byte[64];
            bool firstBlockCaptured = false;

            byte[] state = SHA512.HashData(Encoding.UTF8.GetBytes(secret));
            byte[] secretBytes = Encoding.UTF8.GetBytes(secret);

            using FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
            using StreamWriter writer = new StreamWriter(fs, Encoding.ASCII);

            const int chunkSize = 1 << 20; // 1 MB
            byte[] buffer = new byte[chunkSize];

            long bytesRemaining = totalBytes;
            int byteCount = 0;



            while (bytesRemaining > 0)
            {
                int offset = 0;

                // Expand SHA-512 state into buffer
                while (offset < chunkSize && bytesRemaining > 0)
                {
                    //generate the next 64 bytes of output by hashing the current state
                    byte[] input = new byte[state.Length + secretBytes.Length];
                    Buffer.BlockCopy(state, 0, input, 0, state.Length);
                    Buffer.BlockCopy(secretBytes, 0, input, state.Length, secretBytes.Length);

                    state = SHA512.HashData(input);

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
                    byteCount++;

                    if (byteCount >= rowBytes && rowBytes > 0)
                    {
                        writer.WriteLine();
                        byteCount = 0;
                        Form1.rowCount++;
                    }
                }
                // After first iteration, preview is captured
                firstBlockCaptured = true;
            }
            return firstBlock;
        }

        //generate file directly without holding all data in memory
        private static byte[] GenerateStdRandomStream(long totalBytes, string filename, long rowBytes, string secret)
        {
            byte[] firstBlock = new byte[64];
            bool firstBlockCaptured = false;

            //ObjectSelectorEditor teh std random function
            byte[] sha = SHA512.HashData(Encoding.UTF8.GetBytes(secret));
            int intSeed = BitConverter.ToInt32(sha, 0);
            Random rnd = new Random(intSeed);

            using FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
            using StreamWriter writer = new StreamWriter(fs, Encoding.ASCII);

            const int chunkSize = 1 << 20; // 1 MB
            byte[] buffer = new byte[chunkSize];

            long bytesRemaining = totalBytes;
            int byteCount = 0;


            while (bytesRemaining > 0)
            {
                int toGen = (int)Math.Min(chunkSize, bytesRemaining);
                rnd.NextBytes(buffer);

                for (int i = 0; i < toGen; i++)
                {
                    // Capture FIRST 64 bytes of output
                    if (!firstBlockCaptured && i < 64)
                        firstBlock[i] = buffer[i];


                    Span<char> hex = stackalloc char[2];
                    buffer[i].TryFormat(hex, out _, "X2");
                    writer.Write(hex);
                    byteCount++;

                    if (byteCount >= rowBytes)
                    {
                        writer.WriteLine();
                        byteCount = 0;
                        Form1.rowCount++;
                    }
                }

                bytesRemaining -= toGen;
            }
            return firstBlock;
        }

        //make the sha value more random by mixing it with the secret
        //      private static byte[] MixSecretWithSha512(string secret, byte[] sha512)
        //      {
        //          if (sha512.Length != 64)
        //              throw new ArgumentException("SHA-512 hash must be 64 bytes.");
        //
        //            byte[] secretBytes = Encoding.UTF8.GetBytes(secret);
        // 
        //
        //
        //            if (secretBytes.Length == 0)
        //                return sha512.ToArray();
        //
        //            byte[] mixed = sha512.ToArray();
        //
        //            int total = Math.Max(64, secretBytes.Length);
        //
        //            for (int i = 0; i < total; i++)
        //            {
        //                int pos = i % 64;                    // SHA index
        //                int j = i % secretBytes.Length;    // secret index (correct!)
        //
        //                mixed[pos] ^= secretBytes[j];
        //            }
        //
        //            return mixed;
        //        }



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
