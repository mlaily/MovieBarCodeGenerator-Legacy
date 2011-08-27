namespace MovieBarCode
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtPathIn = new System.Windows.Forms.TextBox();
            this.btnBrowseIn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnProcess = new System.Windows.Forms.Button();
            this.ofd1 = new System.Windows.Forms.OpenFileDialog();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label2 = new System.Windows.Forms.Label();
            this.txtWidth = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtHeight = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnBrowseOut = new System.Windows.Forms.Button();
            this.txtPathOut = new System.Windows.Forms.TextBox();
            this.sfd1 = new System.Windows.Forms.SaveFileDialog();
            this.link1 = new System.Windows.Forms.LinkLabel();
            this.label5 = new System.Windows.Forms.Label();
            this.txtIterations = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtBarWidth = new System.Windows.Forms.TextBox();
            this.chkAutoCorrect = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // txtPathIn
            // 
            this.txtPathIn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPathIn.Location = new System.Drawing.Point(12, 31);
            this.txtPathIn.Name = "txtPathIn";
            this.txtPathIn.Size = new System.Drawing.Size(336, 20);
            this.txtPathIn.TabIndex = 0;
            // 
            // btnBrowseIn
            // 
            this.btnBrowseIn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseIn.Location = new System.Drawing.Point(354, 29);
            this.btnBrowseIn.Name = "btnBrowseIn";
            this.btnBrowseIn.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseIn.TabIndex = 1;
            this.btnBrowseIn.Text = "Browse...";
            this.btnBrowseIn.UseVisualStyleBackColor = true;
            this.btnBrowseIn.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Video path :";
            // 
            // btnProcess
            // 
            this.btnProcess.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnProcess.Location = new System.Drawing.Point(354, 147);
            this.btnProcess.Name = "btnProcess";
            this.btnProcess.Size = new System.Drawing.Size(75, 23);
            this.btnProcess.TabIndex = 3;
            this.btnProcess.Text = "Generate!";
            this.btnProcess.UseVisualStyleBackColor = true;
            this.btnProcess.Click += new System.EventHandler(this.btnProcess_Click);
            // 
            // ofd1
            // 
            this.ofd1.Filter = "Avi files|*.avi|Wmv files|*.wmv|All files|*.*";
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(12, 176);
            this.progressBar1.Maximum = 1000;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(417, 23);
            this.progressBar1.Step = 1;
            this.progressBar1.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 106);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Image size :";
            // 
            // txtWidth
            // 
            this.txtWidth.Location = new System.Drawing.Point(12, 122);
            this.txtWidth.Name = "txtWidth";
            this.txtWidth.Size = new System.Drawing.Size(55, 20);
            this.txtWidth.TabIndex = 7;
            this.txtWidth.Text = "1000";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(68, 125);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(12, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "x";
            // 
            // txtHeight
            // 
            this.txtHeight.Location = new System.Drawing.Point(81, 122);
            this.txtHeight.Name = "txtHeight";
            this.txtHeight.Size = new System.Drawing.Size(55, 20);
            this.txtHeight.TabIndex = 9;
            this.txtHeight.Text = "500";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 57);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Output path :";
            // 
            // btnBrowseOut
            // 
            this.btnBrowseOut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseOut.Location = new System.Drawing.Point(354, 71);
            this.btnBrowseOut.Name = "btnBrowseOut";
            this.btnBrowseOut.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseOut.TabIndex = 11;
            this.btnBrowseOut.Text = "Browse...";
            this.btnBrowseOut.UseVisualStyleBackColor = true;
            this.btnBrowseOut.Click += new System.EventHandler(this.btnBrowseOut_Click);
            // 
            // txtPathOut
            // 
            this.txtPathOut.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPathOut.Location = new System.Drawing.Point(12, 73);
            this.txtPathOut.Name = "txtPathOut";
            this.txtPathOut.Size = new System.Drawing.Size(336, 20);
            this.txtPathOut.TabIndex = 10;
            // 
            // sfd1
            // 
            this.sfd1.Filter = "Bitmap|*.bmp|Jpeg|*.jpg|Png|*.png|Gif|*.gif|All files|*.*";
            // 
            // link1
            // 
            this.link1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.link1.AutoSize = true;
            this.link1.LinkColor = System.Drawing.Color.SteelBlue;
            this.link1.Location = new System.Drawing.Point(12, 206);
            this.link1.Name = "link1";
            this.link1.Size = new System.Drawing.Size(101, 13);
            this.link1.TabIndex = 13;
            this.link1.TabStop = true;
            this.link1.Text = "ArcaneSanctum.net";
            this.link1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.link1_LinkClicked);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(170, 106);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Iterations:";
            // 
            // txtIterations
            // 
            this.txtIterations.Location = new System.Drawing.Point(173, 122);
            this.txtIterations.Name = "txtIterations";
            this.txtIterations.Size = new System.Drawing.Size(55, 20);
            this.txtIterations.TabIndex = 15;
            this.txtIterations.Text = "1000";
            this.txtIterations.TextChanged += new System.EventHandler(this.txtIterations_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(239, 106);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(54, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "Bar width:";
            // 
            // txtBarWidth
            // 
            this.txtBarWidth.Location = new System.Drawing.Point(242, 122);
            this.txtBarWidth.Name = "txtBarWidth";
            this.txtBarWidth.Size = new System.Drawing.Size(55, 20);
            this.txtBarWidth.TabIndex = 17;
            this.txtBarWidth.Text = "1";
            this.txtBarWidth.TextChanged += new System.EventHandler(this.txtBarWidth_TextChanged);
            // 
            // chkAutoCorrect
            // 
            this.chkAutoCorrect.AutoSize = true;
            this.chkAutoCorrect.Checked = true;
            this.chkAutoCorrect.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoCorrect.Location = new System.Drawing.Point(173, 153);
            this.chkAutoCorrect.Name = "chkAutoCorrect";
            this.chkAutoCorrect.Size = new System.Drawing.Size(118, 17);
            this.chkAutoCorrect.TabIndex = 18;
            this.chkAutoCorrect.Text = "Auto correct values";
            this.chkAutoCorrect.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(441, 228);
            this.Controls.Add(this.chkAutoCorrect);
            this.Controls.Add(this.txtBarWidth);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtIterations);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.link1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnBrowseOut);
            this.Controls.Add(this.txtPathOut);
            this.Controls.Add(this.txtHeight);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtWidth);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.btnProcess);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnBrowseIn);
            this.Controls.Add(this.txtPathIn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(416, 249);
            this.Name = "Form1";
            this.Text = "Movie BarCode Generator - ";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtPathIn;
        private System.Windows.Forms.Button btnBrowseIn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnProcess;
        private System.Windows.Forms.OpenFileDialog ofd1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtWidth;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtHeight;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnBrowseOut;
        private System.Windows.Forms.TextBox txtPathOut;
        private System.Windows.Forms.SaveFileDialog sfd1;
        private System.Windows.Forms.LinkLabel link1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtIterations;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtBarWidth;
        private System.Windows.Forms.CheckBox chkAutoCorrect;
    }
}

