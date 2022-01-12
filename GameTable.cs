using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ShootClient
{
    [Obsolete]
    partial class frm_gametable : Form
    {
        private Button[] seatButtons = new Button[6];
        private Label[] seatLabels = new Label[6];
        private Label[] statusLabels = new Label[6];

        private PictureBox[] cardPictureBoxes = new PictureBox[6];
        private Bitmap[] cardHandBitmaps = new Bitmap[6];
        protected Point[] cardHandPoints = new Point[10];
        private int[] cardHandClickMap;

        private Bitmap cardSurfaceBitmap;
        private Point[] cardSurfacePoints = new Point[6];

        private Control[] bidControls;
        private RadioButton[] bidRadios;

        private Trump currentBidTrump = null;
        private int currentBidTricks = 0;

        Color currentChatColour = Color.DarkOliveGreen;

        protected bool initializing = false;
        protected bool throwingAway = false;
        protected bool choosingTransfer = false;

        public frm_gametable()
        {
            InitializeComponent();

            for (int i = 0; i < 6; i++)
            {
                seatButtons[i] = (Button)this.GetType().GetField("cmd_seat" + i).GetValue(this);
                seatLabels[i] = (Label)this.GetType().GetField("lbl_seat" + i).GetValue(this);
                statusLabels[i] = (Label)this.GetType().GetField("lbl_status" + i).GetValue(this);

                cardPictureBoxes[i] = (PictureBox)this.GetType().GetField("pb_p" + i).GetValue(this);
                cardHandBitmaps[i] = new Bitmap(cardPictureBoxes[i].Size.Width, cardPictureBoxes[i].Size.Height);
                cardSurfacePoints[i] = new Point(75 * ((int)Math.Floor((double)i / 3) * (i % 3) + (1 - (int)Math.Floor((double)i / 3)) * ((5 - i) % 3)), 108 * (1 - (int)Math.Floor((double)i / 3)));
            }
            for (int i = 0; i < cardHandPoints.Length; i++)
            {
                cardHandPoints[i] = new Point(i * 21, 0);
            }
            bidControls = new Control[] { lbl_bid, rdo_hearts, rdo_diamonds, rdo_spades, rdo_clubs, rdo_high, rdo_low, tb_bid, cmd_bid };
            bidRadios = new RadioButton[] { rdo_hearts, rdo_diamonds, rdo_spades, rdo_clubs, rdo_high, rdo_low };
            cardHandClickMap = new int[cardPictureBoxes[0].Size.Width];

            cardSurfaceBitmap = new Bitmap(this.pb_cardSurface.Width, this.pb_cardSurface.Height);
        }

        public virtual void initializeGame()
        {
            initializing = true;

            for (int i = 0; i < 6; i++)
            {
                seatLabels[i].Text = string.Empty;
                statusLabels[i].Show();
                statusLabels[i].Text = string.Empty;
                seatButtons[i].Show();
            }
            chk_ready.Checked = false;
            chk_ready.Hide();
            hideHand();
            hideBidControls();
            hidePlayingSurface();
            
            initializing = false;
        }

        public virtual void initializeBiddingPhase()
        {
            statusLabels[ClientMain.me.position].Hide();
            for (int i = 0; i < 6; i++)
            {
                statusLabels[i].Text = string.Empty;
                seatButtons[i].Hide();
                bidRadios[i].Checked = false;
            }
            currentBidTricks = 0;
            currentBidTrump = null;
            pb_cardSurface.Hide();
            chk_ready.Hide();
            tb_bid.Value = 0;
            drawScores();
            clearBitmaps();
            drawHandCardsOnTable();
        }

        public virtual void initializePlayingPhase()
        {
            showPlayingSurface();
            hideStatusLabels();
            drawHandCardsOnTable();
        }

        public virtual void initializeTrick()
        {
        }

        public virtual void endGame()
        {
            hidePlayingSurface();
        }

        public void startThrowingAwayCards()
        {
            throwingAway = true;
        }

        public void stopThrowingAwayCards()
        {
            throwingAway = false;
        }

        public void startChoosingTransfer()
        {
            choosingTransfer = true;
        }

        public void stopChoosingTransfer()
        {
            choosingTransfer = false;
        }

        public virtual void showBidControls()
        {
            foreach (Control control in bidControls)
            {
                control.Show();
            }
        }

        public virtual void hideBidControls()
        {
            foreach (Control control in bidControls)
            {
                control.Hide();
            }
        }

        public virtual void showStatusLabel(int position, string status)
        {
            statusLabels[position].Text = status;
        }

        public virtual void hideStatusLabels()
        {
            foreach (Label label in statusLabels)
            {
                label.Text = string.Empty;
            }
        }

        public virtual void showHand()
        {
            cardPictureBoxes[ClientMain.me.position].Show();
            drawHandCardsOnTable();
        }

        public virtual void hideHand()
        {
            if (ClientMain.me.position != -1)
            {
                cardPictureBoxes[ClientMain.me.position].Hide();
            }
        }

        protected virtual void hidePlayingSurface()
        {
            pb_cardSurface.Hide();
        }

        protected virtual void showPlayingSurface()
        {
            pb_cardSurface.Show();
        }

        private void clearBitmaps()
        {
            Graphics.FromImage(cardSurfaceBitmap).Clear(BackColor);
            foreach (Bitmap bitmap in cardHandBitmaps)
            {
                Graphics.FromImage(bitmap).Clear(BackColor);
            }
        }

        public virtual void assignSeat(Player player)
        {
            int position = player.position;
            seatButtons[position].Hide();
            seatLabels[position].Text = player.name;
            if (player.ready)
            {
                statusLabels[position].Text = "READY";
            }
            else
            {
                statusLabels[position].Text = string.Empty;
            }

            if (player.Equals(ClientMain.me))
            {
                foreach (Button button in seatButtons)
                {
                    button.Hide();
                }
                chk_ready.Show();
            }
        }

        public virtual void postChatMessage(string speaker, string content)
        {
            string text = speaker + ": " + content;

            if (currentChatColour == Color.DarkOliveGreen)
            {
                currentChatColour = Color.Black;
            }
            else
            {
                currentChatColour = Color.DarkOliveGreen;
            }
            txt_chatbox.Select(0, 0);

            txt_chatbox.SelectedText = text;

            txt_chatbox.Select(0, text.Length);

            txt_chatbox.SelectionColor = currentChatColour;

        }

        private void cmd_seat_Click(object sender, EventArgs e)
        {
            string seatID;
            Button seatButtonClicked = (Button)sender;
            seatID = seatButtonClicked.Name.Substring(8, 1);
            Player.sendMessageToServer("<REQUESTSEAT>" + seatID);
        }

        private void txt_sendMessage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                Player.sendMessageToServer("<CHATMESSAGE>" + txt_sendMessage.Text);
                txt_sendMessage.Clear();
            }
        }

        private void chk_ready_CheckedChanged(object sender, EventArgs e)
        {
            if (!initializing)
            {
                int playerReady;
                if (chk_ready.Checked)
                {
                    playerReady = 1;
                }
                else
                {
                    playerReady = 0;
                }
                Player.sendMessageToServer("<READYSTATUS>" + playerReady);
            }
        }

        private void tb_bid_ValueChanged(object sender, EventArgs e)
        {
            TrackBar tb = (TrackBar)sender;
            int tricks = tb.Value;
            currentBidTricks = tricks;
            updateBid(tricks, currentBidTrump);
        }

        private void rdo_bid_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rdo = (RadioButton)sender;
            if (rdo.Checked)
            {
                int tricks = tb_bid.Value;
                Trump trump = Trump.FromString(rdo.Text);
                currentBidTrump = trump;
                updateBid(tricks, trump);
                drawHandCardsOnTable(trump);
            }
        }

        private void updateBid(int tricks, Trump trump)
        {
            if (trump == null)
            {
                cmd_bid.Text = "Place Bid (Pass)";
            }
            else
            {
                switch (tricks)
                {
                    case 0:
                        cmd_bid.Text = "Place Bid (Pass)";
                        break;
                    case 9:
                        cmd_bid.Text = "Place Bid (Shoot " + trump.ToString() + ")";
                        break;
                    default:
                        cmd_bid.Text = "Place Bid (" + tricks + " " + trump.ToString() + ")";
                        break;
                }
            }
        }

        public virtual void showBid(Bid bid)
        {
            //showStatusLabel(bid.bidder.position, bid.ToString());
        }

        private void cmd_bid_Click(object sender, EventArgs e)
        {
            string trump;
            int shootNum = 0;

            trump = currentBidTrump == null ? string.Empty : currentBidTrump.ToString();

            if (currentBidTricks == 9) shootNum = ClientMain.me.game.nextShootNum;

            Player.sendMessageToServer("<BID>" + currentBidTricks + shootNum + trump);
        }

        private void cardClicked(object sender, MouseEventArgs e)
        {
            int x = e.X;
            int cardNum = cardHandClickMap[x];

            if (cardNum > -1 && (throwingAway || choosingTransfer || ClientMain.me.game.currentPlayer == ClientMain.me.position))
            {
                Card card = ClientMain.me.hand[cardHandClickMap[x]];

                if (throwingAway)
                {
                    ClientMain.me.throwAwayCard(card);
                }
                else if (choosingTransfer)
                {
                    ClientMain.me.transferCard(card);
                }
                else
                {
                    ClientMain.me.playCard(card);
                }
            }
        }

        public virtual void drawHandCardsOnTable()
        {
            drawHandCardsOnTable(ClientMain.me.game.currentTrump);
        }

        public virtual void drawHandCardsOnTable(Trump trump)
        {
            if (ClientMain.me.position == -1 || ClientMain.me.hand == null) return;

            Bitmap canvas = cardHandBitmaps[ClientMain.me.position];
            Graphics g = Graphics.FromImage(canvas);
            Point[] cardPoints = cardHandPoints;
            Image image = null;

            ClientMain.me.sortHand(trump);

            g.Clear(BackColor);

            if (ClientMain.me.hand.Count > 0)
            {
                for (int i = 0; i < ClientMain.me.hand.Count; i++)
                {
                    //image = ClientMain.findCardImage(ClientMain.me.hand[i]);
                    g.DrawImage(image, cardPoints[i]);

                    // keep track of which card is where (only x matters) [TODO: consider bitconverter?]
                    for (int j = cardPoints[i].X; j < cardPoints[i].X + image.Width; j++)
                    {
                        cardHandClickMap[j] = i;
                    }
                }

                // fill the rest of the click map with -1s
                for (int j = cardPoints[ClientMain.me.hand.Count - 1].X + image.Width + 1; j < cardHandClickMap.Length; j++)
                {
                    cardHandClickMap[j] = -1;
                }
            }

            cardPictureBoxes[ClientMain.me.position].Image = canvas;
            cardPictureBoxes[ClientMain.me.position].Show();
        }

        public virtual void drawPlayedCardsOnTable()
        {
            Bitmap canvas = cardSurfaceBitmap;
            Graphics g = Graphics.FromImage(canvas);
            Point[] cardPoints = cardSurfacePoints;
            Image image = null;

            g.Clear(BackColor);
            for (int i = 0; i < cardPoints.Length; i++)
            {
                if (ClientMain.me.game.playedCards[i] != null)
                {
                    //image = ClientMain.findCardImage(ClientMain.me.game.playedCards[i]);
                    g.DrawImage(image, cardPoints[i]);
                }
            }

            pb_cardSurface.Image = canvas;
        }

        public virtual void drawScores()
        {
            int[] tricks = ClientMain.me.game.tricks;
            int[] scores = ClientMain.me.game.scores;
            lbl_tricks.Text = "TRICKS: " + tricks[0] + "-" + tricks[1];
            lbl_score.Text = "SCORE: " + scores[0] + "-" + scores[1];
        }

        private void frm_gametable_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Visible)
            {
                Player.sendMessageToServer("<LEAVINGGAME>");
                e.Cancel = true;
            }
        }
    }
}