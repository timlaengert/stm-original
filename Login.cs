using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;

namespace ShootClient
{
    partial class frm_login : Form
    {
        System.Drawing.Text.PrivateFontCollection PFC = new System.Drawing.Text.PrivateFontCollection();
        FontFamily fontFamily;
        internal delegate void TwoParamsDelegate(bool connectionStatus, string connectionMessage);

        public frm_login()
        {
            PFC.AddFontFile("Vera.ttf");
            fontFamily = PFC.Families[0];
            InitializeComponent();
        }

        private void frm_login_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void cmd_login_Click(object sender, EventArgs e)
        {
            tryLogin();
        }

        private void frm_login_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter && cmd_login.Enabled)
            {
                e.Handled = true;
                tryLogin();
            }
        }

        private void cmd_newuser_Click(object sender, EventArgs e)
        {
            Hide();
            ClientMain.newUserScreen.Show();
        }

        private void tryLogin()
        {
            ClientMain.username = txt_username.Text;
            ClientMain.password = txt_password.Text;
            ClientMain.beginLogin(false);
        }

        public void UpdateConnectionStatus(bool status, string message)
        {
            TwoParamsDelegate update = delegate(bool connectionStatus, string connectionMessage)
            {
                lbl_status.Text = message;
                cmd_login.Enabled = status;
                cmd_newuser.Enabled = status;
            };
            Invoke(update, new object[] { status, message });
        }
    }
}