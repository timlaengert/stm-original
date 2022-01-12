using System;
using System.Collections.Generic;
using System.Text;

namespace ShootClient
{
    class Player
    {
        public enum PlayerStatus
        {
            LOGIN_SCREEN, LOBBY, LOOKING_FOR_SEAT, PREGAME_READY, PREGAME_NOT_READY, CHOOSING_BID, WAITING_FOR_BID, CHOOSING_TRANSFER_CARDS,
            WAITING_FOR_TRANSFER, THROWING_AWAY_CARDS, WAITING_FOR_THROWAWAY, CHOOSING_CARD, WAITING_FOR_PLAY, SITTING_OUT, DISCONNECTED,
            LOGGED_OUT, OBSERVING
        };

        public string name = string.Empty;

        private PlayerStatus status = PlayerStatus.LOGIN_SCREEN;
        public PlayerStatus Status
        {
            get { return status; }
            set
            {
                lastStatus = status;
                status = value;
                ClientMain.gameTableScreen.UpdateConnectionStatus(this);
            }
        }

        private PlayerStatus lastStatus = PlayerStatus.DISCONNECTED;
        public PlayerStatus LastStatus
        {
            get { return lastStatus; }
        }

        public int position = -1;

        public bool ready = false;

        public Game game = null;

        public List<Card> hand = new List<Card>();

        private static List<string> WriteQueue = new List<string>();

        public Player()
        {
        }

        public Player(string initName)
        {
            name = initName;
        }

        public Player(string initName, int initPosition, bool initReady)
        {
            name = initName;
            position = initPosition;
            ready = initReady;
        }

        public void joinGame()
        {
            game = new Game();
            ClientMain.gameFinderScreen.gameListTimer.Stop();
            ClientMain.gameFinderScreen.Hide();
            ClientMain.gameTableScreen.InitializeGame();
            ClientMain.gameTableScreen.Show();
            initializeGame();
            sendMessageToServer("<GETSEATLIST>");
        }

        public void ParseCatchupFeed(string feed)
        {
            game = new Game();
            initializeGame();

            string data = feed;

            // players' position, name, status
            int position;
            string name;
            Player newPlayer;

            while (!data.Substring(0, 1).Equals(";"))
            {
                position = int.Parse(data.Substring(0, 1));
                name = data.Substring(1, data.IndexOf(",") - 1);
                if (name == this.name)
                    newPlayer = ClientMain.me;
                else
                    newPlayer = new Player();

                newPlayer.position = position;
                newPlayer.name = name;
                data = data.Substring(data.IndexOf(",") + 1);
                newPlayer.status = (PlayerStatus)Enum.Parse(typeof(PlayerStatus), data.Substring(0, data.IndexOf(",")));
                newPlayer.lastStatus = PlayerStatus.LOBBY;
                if (newPlayer.Equals(ClientMain.me) && (status == PlayerStatus.CHOOSING_BID || status == PlayerStatus.WAITING_FOR_BID))
                    ClientMain.gameTableScreen.initializeBiddingPhase();
                data = data.Substring(data.IndexOf(",") + 1);

                game.players[newPlayer.position] = newPlayer;
                ClientMain.gameTableScreen.assignSeat(newPlayer);
                ClientMain.gameTableScreen.UpdateConnectionStatus(newPlayer);
            }

            data = data.Substring(data.IndexOf(";") + 1);
            hand.Clear();

            Card newCard;
            // hand cards
            while (!data.Substring(0, 1).Equals(";"))
            {
                newCard = Card.FromString(data.Substring(0, 2));
                newCard.player = ClientMain.me;
                hand.Add(newCard);
                data = data.Substring(2);
            }
            data = data.Substring(1);

            // scores
            game.scores[0] = int.Parse(data.Substring(0, data.IndexOf("-")));
            data = data.Substring(data.IndexOf("-") + 1);

            game.scores[1] = int.Parse(data.Substring(0, data.IndexOf(";")));
            data = data.Substring(data.IndexOf(";") + 1);
            ClientMain.gameTableScreen.UpdateScore();

            // tricks
            game.tricks[0] = int.Parse(data.Substring(0, 1));
            game.tricks[1] = int.Parse(data.Substring(1, 1));
            data = data.Substring(data.IndexOf(";") + 1);
            ClientMain.gameTableScreen.UpdateTricks();

            // bids
            int bidPosition;
            Bid bid;
            Bid highBid = null;
            while (!data.Substring(0, 1).Equals(";"))
            {
                bidPosition = int.Parse(data.Substring(0, 1));
                bid = Bid.FromString(data.Substring(1, data.IndexOf(",") - 1));
                bid.bidder = game.players[bidPosition];
                ClientMain.gameTableScreen.UpdateBid(bid);
                if (highBid == null || bid.isBetterThan(highBid)) highBid = bid;

                data = data.Substring(data.IndexOf(",") + 1);
            }
            data = data.Substring(data.IndexOf(";") + 1);
            game.leadingBid = highBid;

            if (!data.Substring(0, 1).Equals(";"))
                game.currentTrump = Trump.FromString(data.Substring(0, data.IndexOf(";")));
            data = data.Substring(data.IndexOf(";") + 1);

            bool winningCard;
            Card playedCard;
            // played cards
            while (!data.Substring(0, 1).Equals(";"))
            {
                position = int.Parse(data.Substring(0, 1));
                playedCard = Card.FromString(data.Substring(1, 2));
                playedCard.player = game.players[position];
                if (game.leadCard == null) game.leadCard = playedCard;
                ClientMain.gameTableScreen.UpdatePlayedCards(playedCard);

                winningCard = int.Parse(data.Substring(3, 1)) == 1;
                if (winningCard) game.winningCard = playedCard;
                data = data.Substring(5);
            }
            data = data.Substring(data.IndexOf(";") + 1);

            if (status == PlayerStatus.CHOOSING_CARD || status == PlayerStatus.WAITING_FOR_PLAY)
            {
                game.currentTrump = highBid.getTrump();
                ClientMain.gameTableScreen.initializePlayingPhase();
            }
            else
                ClientMain.gameTableScreen.UpdateHand();

            if (status == PlayerStatus.CHOOSING_BID || status == PlayerStatus.CHOOSING_TRANSFER_CARDS ||
                status == PlayerStatus.THROWING_AWAY_CARDS)
                game.currentBidder = this.position;
            else if (status == PlayerStatus.CHOOSING_CARD)
                game.currentPlayer = this.position;

            // TODO: test rejoining
        }

        public void initializeGame()
        {
            position = -1;
            ready = false;
            status = PlayerStatus.LOOKING_FOR_SEAT;
        }

        public void initializeBiddingPhase()
        {
            hand.Clear();
            status = PlayerStatus.WAITING_FOR_BID;
        }

        public void initializePlayingPhase()
        {
        }

        public void initializeTrick()
        {
            status = PlayerStatus.WAITING_FOR_PLAY;
        }

        public static void sendMessageToServer(string message)
        {
            message += "\0";

            if (WriteQueue.Count > 0)
            {
                string prefix = string.Empty;
                string suffix = string.Empty;

                if (message.Contains("<USERNAME>")) prefix = message;
                else suffix = message;

                message = string.Empty;

                foreach (string savedMessage in WriteQueue)
                    if (savedMessage.Contains("<USERNAME>")) prefix = savedMessage; // login credentials must be first
                    else message += savedMessage;

                WriteQueue.Clear();
                message = prefix + message + suffix;
            }

            byte[] myWriteBuffer = Encoding.ASCII.GetBytes(message);

            if (ClientMain.socket.Connected)
            {
                ClientMain.socket.GetStream().Write(myWriteBuffer, 0, myWriteBuffer.Length); //TODO: Add exception handling
                ClientMain.pingTimer.Change(ClientMain.PING_INTERVAL, ClientMain.PING_INTERVAL);
            }
            else
            {
                //ClientMain.Restart();
                //ClientMain.Reconnect();
                if (!message.Contains("<PING>")) WriteQueue.Add(message);
            }
        }

        /// <summary>
        /// tell the server which card the client is playing
        /// </summary>
        /// <param name="card">card to play</param>
        public void playCard(Card card)
        {
            if (game.currentPlayer == position)
            {
                sendMessageToServer("<PLAYCARD>" + card.ToString());
            }
        }

        public void throwAwayCard(Card card)
        {
            sendMessageToServer("<THROWAWAYCARD>" + card.ToString());
        }

        public void transferCard(Card card)
        {
            sendMessageToServer("<TRANSFERCARD>" + game.shooter.position + card.ToString());
        }

        /// <summary>
        /// sort the unsorted hand into different suits (in order within the suit).  Note - trump may not be fixed yet, so use parameter.
        /// </summary>
        /// <returns>Dictionary containing cards by suit</returns>
        public void sortHand(Trump t)
        {
            List<Card> temp = new List<Card>(hand);
            List<Card> oneSuitsWorth = new List<Card>();
            Trump trump = t;

            if (trump == null)
            {
                trump = Trump.HIGH;
            }

            hand.Clear();

            foreach (Suit suit in Suit.allSuits)
            {
                oneSuitsWorth = Card.getCardsOfContextualSuit(temp, suit, trump);
                oneSuitsWorth = sortSuit(oneSuitsWorth, trump);

                foreach (Card card in oneSuitsWorth)
                {
                    hand.Add(card);
                }
            }
        }

        /// <summary>
        /// Sort the cards of one suit in descending order. Note - trump may not be fixed yet, so use parameter.
        /// </summary>
        /// <param name="unsortedHand">a list of cards of the same suit</param>
        /// <param name="trump">trump to account for</param>
        /// <returns>an ordered list of cards</returns>
        private List<Card> sortSuit(List<Card> unsortedHand, Trump trump)
        {
            List<Card> sortedSuit = new List<Card>(unsortedHand);

            for (int i = 0; i < sortedSuit.Count; i++)
            {
                Card lowestCard = sortedSuit[i];
                int lowestCardSpot = i;
                for (int j = i + 1; j < sortedSuit.Count; j++)
                {
                    Card targetCard = sortedSuit[j];
                    if (Rules.isCardBetter(targetCard, lowestCard, trump, lowestCard.getSuit()))
                    {
                        lowestCard = targetCard;
                        lowestCardSpot = j;
                    }
                }

                sortedSuit.RemoveAt(lowestCardSpot);
                sortedSuit.Insert(i, lowestCard);
            }
            return sortedSuit;
        }

        public override bool Equals(object obj)
        {
            if (this == null || obj == null) return false;

            if (obj.GetType() != typeof(Player))
            {
                return false;
            }

            Player player = (Player)obj;

            if (player.name == name)
            {
                return true;
            }

            return false;
        }
    }
}
