using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ShootClient
{
    class Game
    {
        public int[] scores = new int[] { 0, 0 };
        public int[] tricks = new int[] { 0, 0 };
        public Player[] players = new Player[6];

        public Card[] playedCards = new Card[6];
        public Trump currentTrump = null;
        public int currentPlayer = -1;
        public int currentBidder = -1;
        public int nextShootNum = 1;
        public Bid leadingBid = null;
        public Card leadCard = null;
        public Card winningCard = null;

        public Player shooter = null;

        public List<string> chatLog = new List<string>();

        public Game()
        {
        }

        public void initializeBiddingPhase()
        {
            tricks = new int[] { 0, 0 };
            playedCards = new Card[6];
            currentBidder = -1;
            currentTrump = null;
            nextShootNum = 1;
            shooter = null;
            leadingBid = null;
        }

        public void endRound(string unprocessedScores)
        {
            scores[0] = int.Parse(unprocessedScores.Substring(0, unprocessedScores.IndexOf("-")));
            scores[1] = int.Parse(unprocessedScores.Substring(unprocessedScores.IndexOf("-") + 1));
            ClientMain.gameTableScreen.UpdateScore();
        }

        public void initializeTrick()
        {
            playedCards = new Card[6];
            leadCard = null;
            winningCard = null;
            currentPlayer = -1;
            //ClientMain.gameTableScreen.UpdatePlayedCards();
        }

        public void endTrick(string unprocessedTricks)
        {
            tricks[0] = int.Parse(unprocessedTricks.Substring(0, 1));
            tricks[1] = int.Parse(unprocessedTricks.Substring(1, 1));
            ClientMain.gameTableScreen.UpdateTricks();
        }

        public void endGame()
        {
            ClientMain.gameTableScreen.endGame();
        }
    }
}
