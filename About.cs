using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ShootClient
{
    public partial class frm_about : Form
    {
        public frm_about()
        {
            InitializeComponent();
            Text += " - v " + ClientMain.VERSION;
        }

        private void frm_about_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Visible && !ClientMain.loggingout)
            {
                e.Cancel = true;
            }
            Hide();
        }

        private void lblLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            const string TARGET = "http://sites.google.com/site/shootthemoongame";
            System.Diagnostics.Process.Start(TARGET);
        }
    }
}