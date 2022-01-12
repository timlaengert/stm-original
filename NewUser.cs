using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ShootClient
{
    public partial class frm_newuser : Form
    {
        public frm_newuser()
        {
            InitializeComponent();
        }

        private void tryNewUser()
        {
            string hashedpass;

            if (txt_username.Text.Length < 4 || txt_username.Text.Length > 16)
            {
                MessageBox.Show("Username must be between 4 and 16 characters.");
                return;
            }

            if (txt_password1.Text.Length < 4 || txt_password2.Text.Length < 4)
            {
                MessageBox.Show("Password must be at least 4 characters.");
                return;
            }

            if (txt_password1.Text == txt_password2.Text)
            {
                ClientMain.username = txt_username.Text;
                ClientMain.password = txt_password1.Text;
                ClientMain.beginLogin(true);
            }
            else
            {
                MessageBox.Show("Passwords do not match.");
            }
        }

        private void cmd_cancel_Click(object sender, EventArgs e)
        {
            Hide();
            ClientMain.loginScreen.Show();
        }

        private void cmd_ok_Click(object sender, EventArgs e)
        {
            tryNewUser();
        }

        private void frm_newuser_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                tryNewUser();
            }

        }

        private void frm_newuser_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}