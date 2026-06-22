namespace OpenRndTool
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            txtInput = new TextBox();
            txtOutput = new TextBox();
            btnStdRandom = new Button();
            btnSha512Random = new Button();
            Reset = new Button();
            label1 = new Label();
            label2 = new Label();
            picLogo = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)picLogo).BeginInit();
            SuspendLayout();
            // 
            // txtInput
            // 
            txtInput.Location = new Point(12, 29);
            txtInput.Multiline = true;
            txtInput.Name = "txtInput";
            txtInput.ScrollBars = ScrollBars.Vertical;
            txtInput.Size = new Size(560, 126);
            txtInput.TabIndex = 0;
            // 
            // txtOutput
            // 
            txtOutput.Location = new Point(12, 192);
            txtOutput.Multiline = true;
            txtOutput.Name = "txtOutput";
            txtOutput.ScrollBars = ScrollBars.Vertical;
            txtOutput.Size = new Size(560, 260);
            txtOutput.TabIndex = 2;
            // 
            // btnStdRandom
            // 
            btnStdRandom.Location = new Point(12, 458);
            btnStdRandom.Name = "btnStdRandom";
            btnStdRandom.Size = new Size(150, 40);
            btnStdRandom.TabIndex = 3;
            btnStdRandom.Text = "Std Random";
            btnStdRandom.UseVisualStyleBackColor = true;
            btnStdRandom.Click += btnStdRandom_Click;
            // 
            // btnSha512Random
            // 
            btnSha512Random.Location = new Point(168, 458);
            btnSha512Random.Name = "btnSha512Random";
            btnSha512Random.Size = new Size(150, 40);
            btnSha512Random.TabIndex = 4;
            btnSha512Random.Text = "SHA512 Random";
            btnSha512Random.UseVisualStyleBackColor = true;
            btnSha512Random.Click += btnSha512Random_Click;
            // 
            // Reset
            // 
            Reset.Location = new Point(324, 458);
            Reset.Name = "Reset";
            Reset.Size = new Size(150, 40);
            Reset.TabIndex = 5;
            Reset.Text = "Reset file sizes";
            Reset.UseVisualStyleBackColor = true;
            Reset.Click += btnRest_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 11);
            label1.Name = "label1";
            label1.Size = new Size(40, 15);
            label1.TabIndex = 6;
            label1.Text = "Inputs";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 174);
            label2.Name = "label2";
            label2.Size = new Size(50, 15);
            label2.TabIndex = 7;
            label2.Text = "Outputs";
            // 
            // picLogo
            // 
            picLogo.Image = (Image)resources.GetObject("picLogo.Image");
            picLogo.Location = new Point(578, 11);
            picLogo.Name = "picLogo";
            picLogo.Size = new Size(71, 71);
            picLogo.SizeMode = PictureBoxSizeMode.Zoom;
            picLogo.TabIndex = 8;
            picLogo.TabStop = false;
            // 
            // Form1
            // 
            ClientSize = new Size(661, 510);
            Controls.Add(picLogo);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(Reset);
            Controls.Add(btnSha512Random);
            Controls.Add(btnStdRandom);
            Controls.Add(txtOutput);
            Controls.Add(txtInput);
            Name = "Form1";
            Text = "OpenRndTool";
            Load += Form1_Load_1;
            ((System.ComponentModel.ISupportInitialize)picLogo).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.Button btnStdRandom;
        private System.Windows.Forms.Button btnSha512Random;
        private Button Reset;
        private Label label1;
        private Label label2;
        private PictureBox picLogo;
    }
}
