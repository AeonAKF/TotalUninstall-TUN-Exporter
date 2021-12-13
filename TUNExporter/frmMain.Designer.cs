
namespace TUNExporter
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.btnInputFile = new System.Windows.Forms.Button();
            this.btnOutput = new System.Windows.Forms.Button();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.prgMain = new System.Windows.Forms.ProgressBar();
            this.lblStatusLeft = new System.Windows.Forms.Label();
            this.lblProg1 = new System.Windows.Forms.Label();
            this.lblFolders = new System.Windows.Forms.Label();
            this.lblFiles = new System.Windows.Forms.Label();
            this.lblRegistry = new System.Windows.Forms.Label();
            this.btnExit = new System.Windows.Forms.Button();
            this.txtInputFile = new System.Windows.Forms.TextBox();
            this.grpInputFile = new System.Windows.Forms.GroupBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblStatusRight = new System.Windows.Forms.Label();
            this.btnExport = new System.Windows.Forms.Button();
            this.lblCurrent = new System.Windows.Forms.Label();
            this.picBoxPRGMain = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.grpInfo = new System.Windows.Forms.GroupBox();
            this.grpInputFile.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxPRGMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.grpInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnInputFile
            // 
            this.btnInputFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnInputFile.Location = new System.Drawing.Point(7, 27);
            this.btnInputFile.Margin = new System.Windows.Forms.Padding(4);
            this.btnInputFile.Name = "btnInputFile";
            this.btnInputFile.Size = new System.Drawing.Size(100, 30);
            this.btnInputFile.TabIndex = 0;
            this.btnInputFile.Text = "Select (1)";
            this.btnInputFile.UseVisualStyleBackColor = true;
            this.btnInputFile.Click += new System.EventHandler(this.btnInputFile_Click);
            // 
            // btnOutput
            // 
            this.btnOutput.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOutput.Location = new System.Drawing.Point(7, 62);
            this.btnOutput.Margin = new System.Windows.Forms.Padding(4);
            this.btnOutput.Name = "btnOutput";
            this.btnOutput.Size = new System.Drawing.Size(100, 30);
            this.btnOutput.TabIndex = 1;
            this.btnOutput.Text = "Select (2)";
            this.btnOutput.UseVisualStyleBackColor = true;
            this.btnOutput.Click += new System.EventHandler(this.btnOutput_Click);
            // 
            // txtOutput
            // 
            this.txtOutput.Location = new System.Drawing.Point(114, 65);
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.Size = new System.Drawing.Size(641, 23);
            this.txtOutput.TabIndex = 2;
            // 
            // prgMain
            // 
            this.prgMain.Location = new System.Drawing.Point(15, 125);
            this.prgMain.Name = "prgMain";
            this.prgMain.Size = new System.Drawing.Size(400, 23);
            this.prgMain.Step = 1;
            this.prgMain.TabIndex = 4;
            // 
            // lblStatusLeft
            // 
            this.lblStatusLeft.AutoSize = true;
            this.lblStatusLeft.Location = new System.Drawing.Point(6, 109);
            this.lblStatusLeft.Name = "lblStatusLeft";
            this.lblStatusLeft.Size = new System.Drawing.Size(52, 17);
            this.lblStatusLeft.TabIndex = 5;
            this.lblStatusLeft.Text = "Status:";
            // 
            // lblProg1
            // 
            this.lblProg1.AutoSize = true;
            this.lblProg1.Location = new System.Drawing.Point(420, 128);
            this.lblProg1.Name = "lblProg1";
            this.lblProg1.Size = new System.Drawing.Size(109, 17);
            this.lblProg1.TabIndex = 6;
            this.lblProg1.Text = "Progress: 0 of 0";
            // 
            // lblFolders
            // 
            this.lblFolders.AutoSize = true;
            this.lblFolders.Location = new System.Drawing.Point(6, 48);
            this.lblFolders.Name = "lblFolders";
            this.lblFolders.Size = new System.Drawing.Size(111, 17);
            this.lblFolders.TabIndex = 8;
            this.lblFolders.Text = "0 Folders Found";
            // 
            // lblFiles
            // 
            this.lblFiles.AutoSize = true;
            this.lblFiles.Location = new System.Drawing.Point(6, 26);
            this.lblFiles.Name = "lblFiles";
            this.lblFiles.Size = new System.Drawing.Size(93, 17);
            this.lblFiles.TabIndex = 9;
            this.lblFiles.Text = "0 Files Found";
            // 
            // lblRegistry
            // 
            this.lblRegistry.AutoSize = true;
            this.lblRegistry.Location = new System.Drawing.Point(6, 70);
            this.lblRegistry.Name = "lblRegistry";
            this.lblRegistry.Size = new System.Drawing.Size(164, 17);
            this.lblRegistry.TabIndex = 10;
            this.lblRegistry.Text = "0 Registry Entries Found";
            // 
            // btnExit
            // 
            this.btnExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExit.Location = new System.Drawing.Point(587, 60);
            this.btnExit.Margin = new System.Windows.Forms.Padding(4);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(170, 38);
            this.btnExit.TabIndex = 11;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // txtInputFile
            // 
            this.txtInputFile.Location = new System.Drawing.Point(114, 30);
            this.txtInputFile.Name = "txtInputFile";
            this.txtInputFile.Size = new System.Drawing.Size(641, 23);
            this.txtInputFile.TabIndex = 12;
            // 
            // grpInputFile
            // 
            this.grpInputFile.Controls.Add(this.btnOutput);
            this.grpInputFile.Controls.Add(this.txtOutput);
            this.grpInputFile.Controls.Add(this.btnInputFile);
            this.grpInputFile.Controls.Add(this.txtInputFile);
            this.grpInputFile.Location = new System.Drawing.Point(12, 12);
            this.grpInputFile.Name = "grpInputFile";
            this.grpInputFile.Size = new System.Drawing.Size(761, 103);
            this.grpInputFile.TabIndex = 13;
            this.grpInputFile.TabStop = false;
            this.grpInputFile.Text = "TUN Export File (1) and Output Folder (2)";
            // 
            // btnCancel
            // 
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(407, 60);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(170, 38);
            this.btnCancel.TabIndex = 15;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblStatusRight
            // 
            this.lblStatusRight.AutoSize = true;
            this.lblStatusRight.Location = new System.Drawing.Point(58, 109);
            this.lblStatusRight.Name = "lblStatusRight";
            this.lblStatusRight.Size = new System.Drawing.Size(49, 17);
            this.lblStatusRight.TabIndex = 16;
            this.lblStatusRight.Text = "Ready";
            // 
            // btnExport
            // 
            this.btnExport.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExport.Location = new System.Drawing.Point(407, 14);
            this.btnExport.Margin = new System.Windows.Forms.Padding(4);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(350, 38);
            this.btnExport.TabIndex = 17;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // lblCurrent
            // 
            this.lblCurrent.Location = new System.Drawing.Point(6, 130);
            this.lblCurrent.Name = "lblCurrent";
            this.lblCurrent.Size = new System.Drawing.Size(740, 20);
            this.lblCurrent.TabIndex = 18;
            this.lblCurrent.Text = "Current:";
            // 
            // picBoxPRGMain
            // 
            this.picBoxPRGMain.BackColor = System.Drawing.SystemColors.Control;
            this.picBoxPRGMain.Location = new System.Drawing.Point(12, 121);
            this.picBoxPRGMain.Name = "picBoxPRGMain";
            this.picBoxPRGMain.Size = new System.Drawing.Size(761, 32);
            this.picBoxPRGMain.TabIndex = 19;
            this.picBoxPRGMain.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.Control;
            this.pictureBox1.Location = new System.Drawing.Point(415, 126);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(353, 22);
            this.pictureBox1.TabIndex = 20;
            this.pictureBox1.TabStop = false;
            // 
            // grpInfo
            // 
            this.grpInfo.Controls.Add(this.lblFolders);
            this.grpInfo.Controls.Add(this.lblCurrent);
            this.grpInfo.Controls.Add(this.lblFiles);
            this.grpInfo.Controls.Add(this.lblStatusRight);
            this.grpInfo.Controls.Add(this.btnExport);
            this.grpInfo.Controls.Add(this.lblRegistry);
            this.grpInfo.Controls.Add(this.btnCancel);
            this.grpInfo.Controls.Add(this.lblStatusLeft);
            this.grpInfo.Controls.Add(this.btnExit);
            this.grpInfo.Location = new System.Drawing.Point(12, 162);
            this.grpInfo.Name = "grpInfo";
            this.grpInfo.Size = new System.Drawing.Size(761, 163);
            this.grpInfo.TabIndex = 21;
            this.grpInfo.TabStop = false;
            this.grpInfo.Text = "Export Information";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 332);
            this.Controls.Add(this.lblProg1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.grpInfo);
            this.Controls.Add(this.grpInputFile);
            this.Controls.Add(this.prgMain);
            this.Controls.Add(this.picBoxPRGMain);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TUN Exporter";
            this.grpInputFile.ResumeLayout(false);
            this.grpInputFile.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxPRGMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.grpInfo.ResumeLayout(false);
            this.grpInfo.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnInputFile;
        private System.Windows.Forms.Button btnOutput;
        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.ProgressBar prgMain;
        private System.Windows.Forms.Label lblStatusLeft;
        private System.Windows.Forms.Label lblProg1;
        private System.Windows.Forms.Label lblFolders;
        private System.Windows.Forms.Label lblFiles;
        private System.Windows.Forms.Label lblRegistry;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.TextBox txtInputFile;
        private System.Windows.Forms.GroupBox grpInputFile;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblStatusRight;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Label lblCurrent;
        private System.Windows.Forms.PictureBox picBoxPRGMain;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.GroupBox grpInfo;
    }
}

