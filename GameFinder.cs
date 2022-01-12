using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ShootClient
{
    partial class frm_gamefinder : Form
    {
        private const int GAME_LIST_REFRESH_INTERVAL = 1000;
        public Timer gameListTimer;

        public frm_gamefinder()
        {
            InitializeComponent();

            gameListTimer = new Timer();
            gameListTimer.Tick += new EventHandler(TimerEventProcessor);
            gameListTimer.Interval = GAME_LIST_REFRESH_INTERVAL;
        }

        public void buildGamesList(string unprocessedList)
        {
            Dictionary<int, string> gamesListNames = new Dictionary<int, string>();
            Dictionary<int, int> gamesListPlayers = new Dictionary<int, int>();
            Dictionary<int, bool> gamesListStarted = new Dictionary<int, bool>();
            int gameID;
            string gameName;
            int numPlayers;
            bool inProgress;

            while (unprocessedList != string.Empty)
            {
                gameID = int.Parse(unprocessedList.Substring(0, unprocessedList.IndexOf(",")));
                unprocessedList = unprocessedList.Substring(unprocessedList.IndexOf(",") + 1);

                gameName = unprocessedList.Substring(0, unprocessedList.IndexOf(","));
                unprocessedList = unprocessedList.Substring(unprocessedList.IndexOf(",") + 1);

                numPlayers = int.Parse(unprocessedList.Substring(0, unprocessedList.IndexOf(",")));
                unprocessedList = unprocessedList.Substring(unprocessedList.IndexOf(",") + 1);

                inProgress = unprocessedList.Substring(0, unprocessedList.IndexOf(",")) == "1";
                unprocessedList = unprocessedList.Substring(unprocessedList.IndexOf(",") + 1);
                
                gamesListNames.Add(gameID, gameName);
                gamesListPlayers.Add(gameID, numPlayers);
                gamesListStarted.Add(gameID, inProgress);
            }

            dgv_gameslist.Rows.Clear();
            foreach (int id in gamesListNames.Keys)
            {
                // don't include started games unless rejoining
                if (!gamesListStarted[id] || id == ShootClient.Properties.Settings.Default.LastGame) 
                    dgv_gameslist.Rows.Add(id, gamesListNames[id], gamesListPlayers[id]);
            }

            cmd_join.Enabled = dgv_gameslist.SelectedRows.Count > 0;
        }

        public void buildUsersList(string unprocessedList)
        {
            string player;

            dgvPlayersOnline.Rows.Clear();

            while (unprocessedList != string.Empty)
            {
                player = unprocessedList.Substring(0, unprocessedList.IndexOf(","));
                unprocessedList = unprocessedList.Substring(unprocessedList.IndexOf(",") + 1);
                dgvPlayersOnline.Rows.Add(player);
            }
        }

        private void TimerEventProcessor(Object myObject, EventArgs myEventArgs)
        {
            Player.sendMessageToServer("<GETGAMESLIST>");
        }

        private void frm_gamefinder_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!ClientMain.loggingout)
            {
                Player.sendMessageToServer("<LOGGINGOUT>");
                ClientMain.logout();
            }
        }

        private void cmd_host_Click(object sender, EventArgs e)
        {
            Player.sendMessageToServer("<REQUESTHOST>" + ClientMain.me.name);
        }

        private void cmd_join_Click(object sender, EventArgs e)
        {
            int gameID;
            const int GAME_ID_COL = 0;

            if (dgv_gameslist.SelectedRows.Count > 0)
            {
                gameID = (int)dgv_gameslist.SelectedRows[0].Cells[GAME_ID_COL].Value;
                Player.sendMessageToServer("<REQUESTJOIN>" + gameID);
            }
            else
            {
                MessageBox.Show("No game selected.");
            }
        }

        private void dgv_gameslist_DoubleClick(object sender, EventArgs e)
        {
            int gameID;
            const int GAME_ID_COL = 0;

            if (dgv_gameslist.SelectedRows.Count > 0)
            {
                gameID = (int)dgv_gameslist.SelectedRows[0].Cells[GAME_ID_COL].Value;
                Player.sendMessageToServer("<REQUESTJOIN>" + gameID);
            }
        }

        private void frm_gamefinder_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible == true)
            {
                Player.sendMessageToServer("<GETGAMESLIST>");
                gameListTimer.Start();
            }
        }

        private void dgv_gameslist_SelectionChanged(object sender, EventArgs e)
        {
            cmd_join.Enabled = dgv_gameslist.SelectedRows.Count > 0;
        }
    }
}