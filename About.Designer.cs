namespace ShootClient
{
    partial class frm_about
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
            this.lblCreated = new System.Windows.Forms.Label();
            this.lblThanks = new System.Windows.Forms.Label();
            this.lblLink = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // lblCreated
            // 
            this.lblCreated.AutoSize = true;
            this.lblCreated.Location = new System.Drawing.Point(12, 9);
            this.lblCreated.Name = "lblCreated";
            this.lblCreated.Size = new System.Drawing.Size(123, 13);
            this.lblCreated.TabIndex = 0;
            this.lblCreated.Text = "Created by Tim Laengert";
            // 
            // lblThanks
            // 
            this.lblThanks.AutoSize = true;
            this.lblThanks.Location = new System.Drawing.Point(12, 34);
            this.lblThanks.Name = "lblThanks";
            this.lblThanks.Size = new System.Drawing.Size(331, 13);
            this.lblThanks.TabIndex = 1;
            this.lblThanks.Text = "Special thanks to Ben Willard, Fraser Kuyvenhoven and David Smith";
            // 
            // lblLink
            // 
            this.lblLink.AutoSize = true;
            this.lblLink.Location = new System.Drawing.Point(12, 59);
            this.lblLink.Name = "lblLink";
            this.lblLink.Size = new System.Drawing.Size(205, 13);
            this.lblLink.TabIndex = 2;
            this.lblLink.TabStop = true;
            this.lblLink.Text = "sites.google.com/site/shootthemoongame";
            this.lblLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblLink_LinkClicked);
            // 
            // frm_about
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(356, 90);
            this.Controls.Add(this.lblLink);
            this.Controls.Add(this.lblThanks);
            this.Controls.Add(this.lblCreated);
            this.Name = "frm_about";
            this.Text = "About Shoot the Moon";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_about_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblCreated;
        private System.Windows.Forms.Label lblThanks;
        private System.Windows.Forms.LinkLabel lblLink;
    }
}