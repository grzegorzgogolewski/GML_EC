namespace GML_EC
{
    partial class FrmAbout
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
            this.lblNazwa = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.lblAdres = new System.Windows.Forms.Label();
            this.lblWWW = new System.Windows.Forms.LinkLabel();
            this.lblLicenseInfo = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // lblNazwa
            // 
            this.lblNazwa.AutoSize = true;
            this.lblNazwa.Font = new System.Drawing.Font("Cambria", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblNazwa.Location = new System.Drawing.Point(12, 95);
            this.lblNazwa.Name = "lblNazwa";
            this.lblNazwa.Size = new System.Drawing.Size(347, 17);
            this.lblNazwa.TabIndex = 1;
            this.lblNazwa.Text = "Gisnet Grzegorz Gogolewski i Wspólnicy Spółka Jawna";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(410, 242);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // lblAdres
            // 
            this.lblAdres.AutoSize = true;
            this.lblAdres.Font = new System.Drawing.Font("Cambria", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblAdres.Location = new System.Drawing.Point(12, 125);
            this.lblAdres.Name = "lblAdres";
            this.lblAdres.Size = new System.Drawing.Size(166, 14);
            this.lblAdres.TabIndex = 3;
            this.lblAdres.Text = "80-273 Gdańsk, ul. Żeglarska 4";
            // 
            // lblWWW
            // 
            this.lblWWW.AutoSize = true;
            this.lblWWW.Location = new System.Drawing.Point(12, 150);
            this.lblWWW.Name = "lblWWW";
            this.lblWWW.Size = new System.Drawing.Size(127, 13);
            this.lblWWW.TabIndex = 4;
            this.lblWWW.TabStop = true;
            this.lblWWW.Text = "http://www.gisnet.com.pl";
            this.lblWWW.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblWWW_LinkClicked);
            // 
            // lblLicenseInfo
            // 
            this.lblLicenseInfo.Location = new System.Drawing.Point(12, 190);
            this.lblLicenseInfo.Name = "lblLicenseInfo";
            this.lblLicenseInfo.Size = new System.Drawing.Size(469, 49);
            this.lblLicenseInfo.TabIndex = 5;
            this.lblLicenseInfo.Text = "lblLicenseInfo";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::GML_EC.Properties.Resources.logo;
            this.pictureBox1.Location = new System.Drawing.Point(16, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(268, 71);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // FrmAbout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(497, 277);
            this.Controls.Add(this.lblLicenseInfo);
            this.Controls.Add(this.lblWWW);
            this.Controls.Add(this.lblAdres);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblNazwa);
            this.Controls.Add(this.pictureBox1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "FrmAbout";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "frmAbout";
            this.Load += new System.EventHandler(this.frmAbout_Load);
            this.Shown += new System.EventHandler(this.FrmAbout_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblNazwa;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label lblAdres;
        private System.Windows.Forms.LinkLabel lblWWW;
        private System.Windows.Forms.Label lblLicenseInfo;
    }
}