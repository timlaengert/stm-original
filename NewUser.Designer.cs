namespace ShootClient
{
    partial class frm_newuser
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
            this.txt_password1 = new System.Windows.Forms.TextBox();
            this.txt_password2 = new System.Windows.Forms.TextBox();
            this.txt_username = new System.Windows.Forms.TextBox();
            this.cmd_ok = new System.Windows.Forms.Button();
            this.cmd_cancel = new System.Windows.Forms.Button();
            this.lbl_username = new System.Windows.Forms.Label();
            this.lbl_password2 = new System.Windows.Forms.Label();
            this.lbl_password1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txt_password1
            // 
            this.txt_password1.Location = new System.Drawing.Point(84, 42);
            this.txt_password1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txt_password1.Name = "txt_password1";
            this.txt_password1.Size = new System.Drawing.Size(196, 21);
            this.txt_password1.TabIndex = 2;
            this.txt_password1.UseSystemPasswordChar = true;
            this.txt_password1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.frm_newuser_KeyPress);
            // 
            // txt_password2
            // 
            this.txt_password2.Location = new System.Drawing.Point(84, 71);
            this.txt_password2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txt_password2.Name = "txt_password2";
            this.txt_password2.Size = new System.Drawing.Size(196, 21);
            this.txt_password2.TabIndex = 3;
            this.txt_password2.UseSystemPasswordChar = true;
            this.txt_password2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.frm_newuser_KeyPress);
            // 
            // txt_username
            // 
            this.txt_username.Location = new System.Drawing.Point(84, 13);
            this.txt_username.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txt_username.Name = "txt_username";
            this.txt_username.Size = new System.Drawing.Size(196, 21);
            this.txt_username.TabIndex = 1;
            this.txt_username.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.frm_newuser_KeyPress);
            // 
            // cmd_ok
            // 
            this.cmd_ok.Location = new System.Drawing.Point(84, 100);
            this.cmd_ok.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmd_ok.Name = "cmd_ok";
            this.cmd_ok.Size = new System.Drawing.Size(95, 24);
            this.cmd_ok.TabIndex = 4;
            this.cmd_ok.Text = "OK";
            this.cmd_ok.UseVisualStyleBackColor = true;
            this.cmd_ok.Click += new System.EventHandler(this.cmd_ok_Click);
            this.cmd_ok.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.frm_newuser_KeyPress);
            // 
            // cmd_cancel
            // 
            this.cmd_cancel.Location = new System.Drawing.Point(185, 100);
            this.cmd_cancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmd_cancel.Name = "cmd_cancel";
            this.cmd_cancel.Size = new System.Drawing.Size(95, 24);
            this.cmd_cancel.TabIndex = 5;
            this.cmd_cancel.Text = "Cancel";
            this.cmd_cancel.UseVisualStyleBackColor = true;
            this.cmd_cancel.Click += new System.EventHandler(this.cmd_cancel_Click);
            // 
            // lbl_username
            // 
            this.lbl_username.AutoSize = true;
            this.lbl_username.BackColor = System.Drawing.Color.Transparent;
            this.lbl_username.ForeColor = System.Drawing.Color.White;
            this.lbl_username.Location = new System.Drawing.Point(12, 13);
            this.lbl_username.Name = "lbl_username";
            this.lbl_username.Size = new System.Drawing.Size(66, 18);
            this.lbl_username.TabIndex = 0;
            this.lbl_username.Text = "Username:";
            // 
            // lbl_password2
            // 
            this.lbl_password2.AutoSize = true;
            this.lbl_password2.BackColor = System.Drawing.Color.Transparent;
            this.lbl_password2.ForeColor = System.Drawing.Color.White;
            this.lbl_password2.Location = new System.Drawing.Point(24, 71);
            this.lbl_password2.Name = "lbl_password2";
            this.lbl_password2.Size = new System.Drawing.Size(54, 18);
            this.lbl_password2.TabIndex = 0;
            this.lbl_password2.Text = "Confirm:";
            // 
            // lbl_password1
            // 
            this.lbl_password1.AutoSize = true;
            this.lbl_password1.BackColor = System.Drawing.Color.Transparent;
            this.lbl_password1.ForeColor = System.Drawing.Color.White;
            this.lbl_password1.Location = new System.Drawing.Point(15, 42);
            this.lbl_password1.Name = "lbl_password1";
            this.lbl_password1.Size = new System.Drawing.Size(63, 18);
            this.lbl_password1.TabIndex = 0;
            this.lbl_password1.Text = "Password:";
            // 
            // frm_newuser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DimGray;
            this.ClientSize = new System.Drawing.Size(292, 137);
            this.Controls.Add(this.lbl_password1);
            this.Controls.Add(this.lbl_password2);
            this.Controls.Add(this.lbl_username);
            this.Controls.Add(this.cmd_cancel);
            this.Controls.Add(this.cmd_ok);
            this.Controls.Add(this.txt_username);
            this.Controls.Add(this.txt_password2);
            this.Controls.Add(this.txt_password1);
            this.Font = new System.Drawing.Font("Trebuchet MS", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "frm_newuser";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Register";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frm_newuser_FormClosed);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.frm_newuser_KeyPress);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txt_password1;
        private System.Windows.Forms.TextBox txt_password2;
        private System.Windows.Forms.TextBox txt_username;
        private System.Windows.Forms.Button cmd_ok;
        private System.Windows.Forms.Button cmd_cancel;
        private System.Windows.Forms.Label lbl_username;
        private System.Windows.Forms.Label lbl_password2;
        private System.Windows.Forms.Label lbl_password1;
    }
}