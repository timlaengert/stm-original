namespace ShootClient
{
    partial class frm_login
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
            this.cmd_login = new System.Windows.Forms.Button();
            this.txt_username = new System.Windows.Forms.TextBox();
            this.lbl_name = new System.Windows.Forms.Label();
            this.lbl_welcome = new System.Windows.Forms.Label();
            this.txt_password = new System.Windows.Forms.TextBox();
            this.lbl_password = new System.Windows.Forms.Label();
            this.cmd_newuser = new System.Windows.Forms.Button();
            this.ss_status_bar = new System.Windows.Forms.StatusStrip();
            this.lbl_status = new System.Windows.Forms.ToolStripStatusLabel();
            this.ss_status_bar.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmd_login
            // 
            this.cmd_login.BackColor = System.Drawing.Color.Transparent;
            this.cmd_login.Enabled = false;
            this.cmd_login.Font = new System.Drawing.Font("Trebuchet MS", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmd_login.Location = new System.Drawing.Point(84, 98);
            this.cmd_login.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmd_login.Name = "cmd_login";
            this.cmd_login.Size = new System.Drawing.Size(95, 24);
            this.cmd_login.TabIndex = 3;
            this.cmd_login.Text = "Login";
            this.cmd_login.UseVisualStyleBackColor = false;
            this.cmd_login.Click += new System.EventHandler(this.cmd_login_Click);
            // 
            // txt_username
            // 
            this.txt_username.Font = new System.Drawing.Font("Trebuchet MS", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_username.Location = new System.Drawing.Point(84, 41);
            this.txt_username.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txt_username.Name = "txt_username";
            this.txt_username.Size = new System.Drawing.Size(196, 21);
            this.txt_username.TabIndex = 1;
            this.txt_username.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.frm_login_KeyPress);
            // 
            // lbl_name
            // 
            this.lbl_name.AutoSize = true;
            this.lbl_name.Font = new System.Drawing.Font("Trebuchet MS", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_name.ForeColor = System.Drawing.Color.White;
            this.lbl_name.Location = new System.Drawing.Point(12, 44);
            this.lbl_name.Name = "lbl_name";
            this.lbl_name.Size = new System.Drawing.Size(66, 18);
            this.lbl_name.TabIndex = 0;
            this.lbl_name.Text = "Username:";
            // 
            // lbl_welcome
            // 
            this.lbl_welcome.AutoSize = true;
            this.lbl_welcome.BackColor = System.Drawing.Color.Transparent;
            this.lbl_welcome.Font = new System.Drawing.Font("Trebuchet MS", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_welcome.ForeColor = System.Drawing.Color.White;
            this.lbl_welcome.Location = new System.Drawing.Point(62, 9);
            this.lbl_welcome.Name = "lbl_welcome";
            this.lbl_welcome.Size = new System.Drawing.Size(172, 18);
            this.lbl_welcome.TabIndex = 0;
            this.lbl_welcome.Text = "Welcome to Shoot the Moon!";
            // 
            // txt_password
            // 
            this.txt_password.Font = new System.Drawing.Font("Trebuchet MS", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_password.Location = new System.Drawing.Point(84, 69);
            this.txt_password.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txt_password.Name = "txt_password";
            this.txt_password.Size = new System.Drawing.Size(196, 21);
            this.txt_password.TabIndex = 2;
            this.txt_password.UseSystemPasswordChar = true;
            this.txt_password.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.frm_login_KeyPress);
            // 
            // lbl_password
            // 
            this.lbl_password.AutoSize = true;
            this.lbl_password.Font = new System.Drawing.Font("Trebuchet MS", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_password.ForeColor = System.Drawing.Color.White;
            this.lbl_password.Location = new System.Drawing.Point(15, 72);
            this.lbl_password.Name = "lbl_password";
            this.lbl_password.Size = new System.Drawing.Size(63, 18);
            this.lbl_password.TabIndex = 0;
            this.lbl_password.Text = "Password:";
            // 
            // cmd_newuser
            // 
            this.cmd_newuser.BackColor = System.Drawing.Color.Transparent;
            this.cmd_newuser.Enabled = false;
            this.cmd_newuser.Font = new System.Drawing.Font("Trebuchet MS", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmd_newuser.Location = new System.Drawing.Point(185, 98);
            this.cmd_newuser.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmd_newuser.Name = "cmd_newuser";
            this.cmd_newuser.Size = new System.Drawing.Size(95, 24);
            this.cmd_newuser.TabIndex = 4;
            this.cmd_newuser.Text = "New User";
            this.cmd_newuser.UseVisualStyleBackColor = false;
            this.cmd_newuser.Click += new System.EventHandler(this.cmd_newuser_Click);
            // 
            // ss_status_bar
            // 
            this.ss_status_bar.BackColor = System.Drawing.SystemColors.Control;
            this.ss_status_bar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lbl_status});
            this.ss_status_bar.Location = new System.Drawing.Point(0, 132);
            this.ss_status_bar.Name = "ss_status_bar";
            this.ss_status_bar.Size = new System.Drawing.Size(294, 22);
            this.ss_status_bar.TabIndex = 5;
            this.ss_status_bar.Text = "statusStrip1";
            // 
            // lbl_status
            // 
            this.lbl_status.BackColor = System.Drawing.SystemColors.Control;
            this.lbl_status.Name = "lbl_status";
            this.lbl_status.Size = new System.Drawing.Size(0, 17);
            // 
            // frm_login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DimGray;
            this.ClientSize = new System.Drawing.Size(294, 154);
            this.Controls.Add(this.ss_status_bar);
            this.Controls.Add(this.cmd_newuser);
            this.Controls.Add(this.lbl_password);
            this.Controls.Add(this.txt_password);
            this.Controls.Add(this.lbl_welcome);
            this.Controls.Add(this.lbl_name);
            this.Controls.Add(this.txt_username);
            this.Controls.Add(this.cmd_login);
            this.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "frm_login";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Shoot the Moon - Connect";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frm_login_FormClosed);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.frm_login_KeyPress);
            this.ss_status_bar.ResumeLayout(false);
            this.ss_status_bar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmd_login;
        private System.Windows.Forms.TextBox txt_username;
        private System.Windows.Forms.Label lbl_name;
        private System.Windows.Forms.Label lbl_welcome;
        private System.Windows.Forms.TextBox txt_password;
        private System.Windows.Forms.Label lbl_password;
        private System.Windows.Forms.Button cmd_newuser;
        private System.Windows.Forms.StatusStrip ss_status_bar;
        private System.Windows.Forms.ToolStripStatusLabel lbl_status;
    }
}

