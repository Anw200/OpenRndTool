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
            txtInput = new TextBox();
            txtSha512 = new TextBox();
            txtOutput = new TextBox();
            btnStdRandom = new Button();
            btnSha512Random = new Button();
            SuspendLayout();
            // 
            // txtInput
            // 
            txtInput.Location = new Point(12, 12);
            txtInput.Multiline = true;
            txtInput.Name = "txtInput";
            txtInput.Size = new Size(560, 60);
            txtInput.TabIndex = 0;
            // 
            // txtSha512
            // 
            txtSha512.Location = new Point(12, 78);
            txtSha512.Multiline = true;
            txtSha512.Name = "txtSha512";
            txtSha512.Size = new Size(560, 60);
            txtSha512.TabIndex = 1;
            // 
            // txtOutput
            // 
            txtOutput.Location = new Point(12, 144);
            txtOutput.Multiline = true;
            txtOutput.Name = "txtOutput";
            txtOutput.ScrollBars = ScrollBars.Vertical;
            txtOutput.Size = new Size(560, 260);
            txtOutput.TabIndex = 2;
            // 
            // btnStdRandom
            // 
            btnStdRandom.Location = new Point(12, 410);
            btnStdRandom.Name = "btnStdRandom";
            btnStdRandom.Size = new Size(150, 40);
            btnStdRandom.TabIndex = 3;
            btnStdRandom.Text = "Std Random";
            btnStdRandom.UseVisualStyleBackColor = true;
            btnStdRandom.Click += btnStdRandom_Click;
            // 
            // btnSha512Random
            // 
            btnSha512Random.Location = new Point(168, 410);
            btnSha512Random.Name = "btnSha512Random";
            btnSha512Random.Size = new Size(150, 40);
            btnSha512Random.TabIndex = 4;
            btnSha512Random.Text = "SHA512 Random";
            btnSha512Random.UseVisualStyleBackColor = true;
            btnSha512Random.Click += btnSha512Random_Click;
            // 
            // Form1
            // 
            ClientSize = new Size(584, 461);
            Controls.Add(btnSha512Random);
            Controls.Add(btnStdRandom);
            Controls.Add(txtOutput);
            Controls.Add(txtSha512);
            Controls.Add(txtInput);
            Name = "Form1";
            Text = "OpenRndTool";
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.TextBox txtSha512;
        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.Button btnStdRandom;
        private System.Windows.Forms.Button btnSha512Random;
    }
}
