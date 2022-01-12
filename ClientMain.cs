using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using ShootClient.Properties;
using System.Web.Security;

namespace ShootClient
{
    static class ClientMain
    {
        public const string VERSION = "0.8.4";

        private delegate void MessageProcessor(string message);
        static MessageProcessor messageProcessor = new MessageProcessor(processMessage);

        public static frm_login loginScreen;
        public static frm_gamefinder gameFinderScreen;
        public static OpenGLTableScreen gameTableScreen;
        public static frm_newuser newUserScreen;
        public static frm_about aboutScreen;

        private static IPEndPoint localEndPoint;
        private static IPEndPoint remoteEndPoint;
        public static TcpClient socket = new TcpClient();
        static byte[] myReadBuffer;

        private static List<string> messageQueue = new List<string>();

        public static bool connecting = false;
        public static bool loggingout = false;

        private static System.Threading.Timer connectTimer = new System.Threading.Timer(Connect);
        private const int CONNECT_INTERVAL = 2000;
        private static int ConnectAttempt = 1;

        private static System.Threading.Timer warningTimer = new System.Threading.Timer(UpdateWarningTimer);
        private static int secondsLeft = 0;
        private static int threatenedPlayer = -1;

        public static System.Threading.Timer pingTimer = new System.Threading.Timer(PingServer);
        public const int PING_INTERVAL = 5000;

        public static Player me = new Player();

        public static string username = string.Empty;
        public static string password = string.Empty;

        static ClientMain()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            loginScreen = new frm_login();
            IntPtr dummy = loginScreen.Handle; // force window handle creation
            newUserScreen = new frm_newuser();
            gameFinderScreen = new frm_gamefinder();
            gameTableScreen = new OpenGLTableScreen();
            aboutScreen = new frm_about();

            Connect(null);
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.Run(loginScreen);
            //System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.AboveNormal;
        }

        private static void Connect(Object state)
        {
            if (socket.Connected || connecting) return;

            loginScreen.UpdateConnectionStatus(false, "Connecting...");
            gameTableScreen.UpdateUpperStatusBar("Connecting (Attempt " + ConnectAttempt + ") ...");
            ConnectAttempt++;
            try
            {
                connecting = true;
                remoteEndPoint = new IPEndPoint(Dns.GetHostEntry(Settings.Default.ServerAddress).AddressList[0], 38521);
                socket.BeginConnect(remoteEndPoint.Address, remoteEndPoint.Port, ConnectCallBack, null);
                //connectTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
            }
            catch (Exception e)
            {
                connecting = false;
                loginScreen.UpdateConnectionStatus(false, "Can't connect to server.  Is your internet working?");
                gameTableScreen.UpdateUpperStatusBar("Can't connect to server.  Is your internet working?");
                connectTimer.Change(CONNECT_INTERVAL, CONNECT_INTERVAL);
            }
        }

        private static void ConnectCallBack(IAsyncResult asyncresult)
        {
            try
            {
                socket.EndConnect(asyncresult);
                connectTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                connecting = false;
                loginScreen.UpdateConnectionStatus(true, "Connected!");
                gameTableScreen.UpdateUpperStatusBar(string.Empty);
                pingTimer.Change(PING_INTERVAL, PING_INTERVAL);
                ConnectAttempt = 1;
            }
            catch (SocketException e)
            {
                connecting = false;
                loginScreen.UpdateConnectionStatus(false, "Can't connect to server.  Is your internet working?");
                gameTableScreen.UpdateUpperStatusBar("Can't connect to server.  Is your internet working?");
                connectTimer.Change(CONNECT_INTERVAL, CONNECT_INTERVAL);
                return;
            }

            NetworkStream dataStream = socket.GetStream();
            myReadBuffer = new byte[1024];
            dataStream.BeginRead(myReadBuffer, 0, myReadBuffer.Length, ReadCallback, dataStream);
        }

        private static void ReadCallback(IAsyncResult asyncresult)
        {
            string transmission;
            NetworkStream dataStream = asyncresult.AsyncState as NetworkStream;
            int bytesRead = 0;

            try
            {
                bytesRead = dataStream.EndRead(asyncresult);
            }
            catch (System.IO.IOException e)
            {
                Reconnect();
                return;
            }

            warningTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
            gameTableScreen.UpdateWarningTimer(threatenedPlayer, -1);
            threatenedPlayer = -1;
            secondsLeft = -1;

            pingTimer.Change(PING_INTERVAL, PING_INTERVAL);

            transmission = Encoding.ASCII.GetString(myReadBuffer, 0, bytesRead);

            if (!string.IsNullOrEmpty(transmission))
            {
                messageQueue.Clear();
                //System.Console.WriteLine("Received Transmission:" + transmission);
                while (transmission.IndexOf("\0") != -1)
                {
                    messageQueue.Add(transmission.Substring(0, transmission.IndexOf("\0")));
                    transmission = transmission.Substring(transmission.IndexOf("\0") + 1);
                }

                foreach (string message in messageQueue)
                {
                    loginScreen.BeginInvoke(messageProcessor, new object[] { message });
                }
            }

            myReadBuffer = new byte[1024];
            try
            {
                dataStream.BeginRead(myReadBuffer, 0, myReadBuffer.Length, ReadCallback, dataStream);
            }
            catch (Exception e)
            {
                gameTableScreen.UpdateUpperStatusBar("Connection lost.");
                Reconnect();
            }
        }

        private static void processMessage(string message)
        {
            if (message.Length <= 0) return;

            int start = message.IndexOf("<") + 1;
            int end = message.IndexOf(">") - 1;
            int length = end - start + 1;
            string header = message.Substring(start, length);
            string content = message.Substring(end + 2);

            Player player;
            string playerName;
            bool readyStatus;
            int playerIndex;
            Card card = null;

            // each case contains an explanation and the format of the message. An asterisk indicates the pattern may repeat.
            switch (header)
            {
                // username and password have been accepted.
                // <username>,<reconnect (int 1)>
                case "CONFIRMLOGIN":
                    me.name = content.Substring(0, content.IndexOf(","));
                    bool reconnected = int.Parse(content.Substring(content.IndexOf(",") + 1, 1)) == 1;
                    if (!reconnected) completeLogin();
                    break;

                // receive the list of joinable games.
                // <gameid (int)>,<gamename>,<numberofplayers (int 1)>,*
                case "GAMESLIST":
                    gameFinderScreen.buildGamesList(content);
                    me.Status = Player.PlayerStatus.LOBBY;
                    break;

                case "USERSLIST":
                    gameFinderScreen.buildUsersList(content);
                    break;

                // ok to join requested game
                // <gameid (int)>
                case "CONFIRMJOIN":
                    me.joinGame();
                    initializeGame();
                    Settings.Default.LastGame = int.Parse(content);
                    break;

                // generic error.  TODO: Consider adding error codes
                // <errormessage>
                case "ERROR":
                    handleError(content);
                    break;

                // receive seat list
                // <position (int 1)><ready (int 1)><playername>,*
                case "SEATLIST":
                    while (content != string.Empty)
                    {
                        playerIndex = int.Parse(content.Substring(0, 1));
                        readyStatus = int.Parse(content.Substring(1, 1)) == 1;

                        content = content.Substring(2);
                        playerName = content.Substring(0, content.IndexOf(","));

                        player = new Player(playerName, playerIndex, readyStatus);
                        me.game.players[player.position] = player;
                        gameTableScreen.assignSeat(player);

                        content = content.Substring(content.IndexOf(",") + 1);
                    }
                    break;

                // a player has taken a seat.
                // <position (int 1)><playername>
                case "CONFIRMSEAT":
                    playerIndex = int.Parse(content.Substring(0, 1));
                    playerName = content.Substring(1);

                    if (playerName.Equals(me.name))
                    {
                        player = me;

                    }
                    else if (playerName.Equals("NUL"))
                    {
                        me.game.players[playerIndex] = null;
                        gameTableScreen.emptySeat(playerIndex);
                        break;
                    }
                    else
                    {
                        player = new Player(playerName);
                    }
                    player.position = playerIndex;
                    me.game.players[player.position] = player;
                    player.Status = Player.PlayerStatus.PREGAME_NOT_READY;

                    gameTableScreen.assignSeat(player);
                    break;

                // receive a chat message
                // <playername>,<message>
                case "CHATMESSAGE":
                    playerName = content.Substring(0, content.IndexOf(","));
                    string chatMessage = content.Substring(content.IndexOf(",") + 1);
                    me.game.chatLog.Add(playerName + ": " + chatMessage);
                    gameTableScreen.UpdateChatLog();
                    break;

                // a player's ready status has changed
                // <position (int 1)><ready (int 1)>
                case "READYSTATUS":
                    playerIndex = int.Parse(content.Substring(0, 1));
                    bool playerReady = int.Parse(content.Substring(1, 1)) == 1;

                    //me.game.players[playerIndex].ready = playerReady;
                    //gameTableScreen.UpdateReadyStatus();
                    me.game.players[playerIndex].Status = playerReady ? Player.PlayerStatus.PREGAME_READY : Player.PlayerStatus.PREGAME_NOT_READY;
                    break;

                // game is starting
                case "STARTGAME":
                    foreach (Player p in me.game.players)
                    {
                        p.Status = Player.PlayerStatus.WAITING_FOR_BID;
                    }
                    break;

                // receive current game state (used to rejoin a game)
                case "CATCHUP":
                    me.ParseCatchupFeed(content);
                    break;

                // receive a new hand
                // <cardrank (char)><cardsuit (char)>*
                case "NEWHAND":
                    string cardShortName;
                    initializeBiddingPhase(); // receiving new hand - start bidding phase
                    while (content != string.Empty)
                    {
                        cardShortName = content.Substring(0, 2);
                        content = content.Substring(2);
                        card = Card.FromString(cardShortName);
                        card.player = me;
                        me.hand.Add(card);
                    }

                    gameTableScreen.UpdateHand();
                    break;

                // server is waiting for our bid
                // <position (int 1)>
                case "REQUESTBID":
                    playerIndex = int.Parse(content);
                    me.game.currentBidder = playerIndex;

                    me.game.players[playerIndex].Status = Player.PlayerStatus.CHOOSING_BID;

                    break;

                // someone's bid has been accepted
                // <position (int 1)><tricksbid (int 1)><shootnum (int 1)><trump>
                case "CONFIRMBID":
                    playerIndex = int.Parse(content.Substring(0, 1));

                    Bid bid = Bid.FromString(content.Substring(1));
                    bid.bidder = me.game.players[playerIndex];
                    me.game.currentBidder = playerIndex;
                    if (!bid.isPass()) me.game.leadingBid = bid;

                    if (bid.getShootNumber() > 0) me.game.nextShootNum++;

                    bid.bidder.Status = Player.PlayerStatus.WAITING_FOR_BID;
                    gameTableScreen.UpdateBid(bid);

                    break;

                // our bid has been rejected. -- REPLACED WITH <ERROR>
                // <errormessage>
                //case "REJECTBID":
                //    break;

                // trump has been finalized.
                // <position><tricks><shootnum><trump>
                case "CONFIRMTRUMP":
                    me.game.leadingBid = Bid.FromString(content.Substring(1));
                    me.game.leadingBid.bidder = me.game.players[int.Parse(content.Substring(0, 1))];
                    me.game.currentTrump = me.game.leadingBid.getTrump();
                    //me.game.currentTrump = Trump.FromString(content);

                    EndBiddingPhase();
                    initializePlayingPhase(); // bidding is finished - start playing
                    initializeTrick();
                    break;

                // server is waiting for someone to transfer a card
                // <fromposition (int 1)><toposition (int 1)>
                case "REQUESTTRANSFER":
                    playerIndex = int.Parse(content.Substring(0, 1));
                    //gameTableScreen.showStatusLabel(playerIndex, "Choosing card");
                    //if (playerIndex == me.position) gameTableScreen.startChoosingTransfer();
                    me.game.players[playerIndex].Status = Player.PlayerStatus.CHOOSING_TRANSFER_CARDS;

                    playerIndex = int.Parse(content.Substring(1, 1));
                    //gameTableScreen.showStatusLabel(playerIndex, "Waiting for cards");
                    //me.game.players[playerIndex].Status = Player.PlayerStatus.WAITING_FOR_TRANSFER;
                    me.game.shooter = me.game.players[playerIndex];

                    foreach (Player p in me.game.players)
                        if (p.Status != Player.PlayerStatus.CHOOSING_TRANSFER_CARDS)
                            p.Status = Player.PlayerStatus.WAITING_FOR_TRANSFER;
                    break;

                // someone's transfer card has been accepted.
                // <fromposition (int 1)><toposition (int 1)>[<cardrank (char)><cardsuit (char)>]
                case "CONFIRMTRANSFER":
                    int fromPlayer = int.Parse(content.Substring(0, 1));
                    int toPlayer = int.Parse(content.Substring(1, 1));
                    //gameTableScreen.showStatusLabel(fromPlayer, string.Empty);
                    me.game.players[fromPlayer].Status = Player.PlayerStatus.WAITING_FOR_TRANSFER;

                    content = content.Substring(2);

                    if (content.Length > 0) card = Card.FromString(content.Substring(0, 2));

                    if (fromPlayer == me.position)
                    {
                        //gameTableScreen.stopChoosingTransfer();
                        gameTableScreen.TransferCard();
                        me.hand.Remove(card);
                        card.player = me.game.players[toPlayer];
                        me.Status = Player.PlayerStatus.SITTING_OUT;
                        gameTableScreen.UpdateConnectionStatus(me);
                    }
                    else if (toPlayer == me.position)
                    {
                        card.player = me;
                        me.hand.Add(card);
                    }

                    gameTableScreen.UpdateHand();

                    break;

                // server is waiting for someone to throw away cards
                // <position (int 1)>
                case "REQUESTTHROWAWAY":
                    playerIndex = int.Parse(content);
                    //gameTableScreen.showStatusLabel(playerIndex, "Throwing away");
                    //if (playerIndex == me.position) gameTableScreen.startThrowingAwayCards();
                    me.game.players[playerIndex].Status = Player.PlayerStatus.THROWING_AWAY_CARDS;
                    break;

                // someone's throwaway card has been accepted.
                // <position (int 1)><finished (int 1 - bool)>[<cardrank (char)><cardsuit (char)>]
                case "CONFIRMTHROWAWAY":
                    playerIndex = int.Parse(content.Substring(0, 1));
                    bool finished = int.Parse(content.Substring(1, 1)) == 1;
                    if (finished)
                    {
                        //if (playerIndex == me.position) gameTableScreen.stopThrowingAwayCards();
                        //gameTableScreen.showStatusLabel(playerIndex, string.Empty);
                        me.game.players[playerIndex].Status = Player.PlayerStatus.WAITING_FOR_THROWAWAY;
                    }

                    if (content.Length > 2 && playerIndex == me.position) // these two predicates should be equivalent
                    {
                        // server sent us the card info
                        card = Card.FromString(content.Substring(2, 2));
                        gameTableScreen.ThrowAwayCard();
                        me.hand.Remove(card);
                        gameTableScreen.UpdateHand();
                    }
                    break;

                // server is waiting for someone's card.
                // <position (int 1)>
                case "REQUESTCARD":
                    me.game.currentPlayer = int.Parse(content);
                    //gameTableScreen.showStatusLabel(me.game.currentPlayer, "Choosing card");
                    me.game.players[me.game.currentPlayer].Status = Player.PlayerStatus.CHOOSING_CARD;
                    break;

                // someone's card has been accepted.
                // <position (int 1)><cardrank (char)><cardsuit (char)><winning (int 1 - bool)>
                case "CONFIRMCARD":
                    playerIndex = int.Parse(content.Substring(0, 1));
                    card = Card.FromString(content.Substring(1, 2));
                    bool winning = int.Parse(content.Substring(3, 1)) == 1;

                    card.player = me.game.players[playerIndex];
                    me.game.playedCards[playerIndex] = card;
                    if (me.game.leadCard == null) me.game.leadCard = card;
                    if (winning) me.game.winningCard = card;

                    gameTableScreen.UpdatePlayedCards(card);

                    if (me.game.currentPlayer == me.position)
                    {
                        me.hand.Remove(card);
                        //gameTableScreen.drawHandCardsOnTable();
                        gameTableScreen.UpdateHand();
                    }
                    //gameTableScreen.showStatusLabel(me.game.currentPlayer, string.Empty);
                    me.game.players[playerIndex].Status = Player.PlayerStatus.WAITING_FOR_PLAY;

                    //gameTableScreen.drawPlayedCardsOnTable();
                    break;

                // receive score so far this round
                // <team1tricks (int 1)><team2tricks (int 1)>
                case "TRICKS":
                    EndTrick();
                    me.game.endTrick(content);
                    initializeTrick();
                    break;

                // receive score so far this game
                // <team1score (int)>-<team2score (int)>
                case "SCORES":
                    EndPlayingPhase();
                    me.game.endRound(content);
                    break;

                // game is over
                // <winningteamnumber (int 1)>
                case "ENDGAME":
                    me.game.endGame();
                    break;

                // server knows we're leaving the game
                // empty
                case "CONFIRMLEAVE":
                    leaveGame();
                    break;

                // server is threatening to disconnect us if no transmission is sent
                // <secondsleft (int)>
                case "TIMEOUTWARNING":
                    playerIndex = int.Parse(content.Substring(0, content.IndexOf(",")));
                    content = content.Substring(content.IndexOf(",") + 1);
                    int seconds = int.Parse(content);
                    ActivateWarningTimer(playerIndex, seconds);
                    break;

                // server has disconnected us
                // empty
                case "FORCELOGOUT":
                    loggingout = true;
                    logout();
                    break;

                // message not recognized
                default:
                    break;
            }
        }

        private static void handleError(string error)
        {
            if (me.Status == Player.PlayerStatus.LOBBY || me.Status == Player.PlayerStatus.LOGIN_SCREEN)
                MessageBox.Show(error);
            //if (me.Status == Player.PlayerStatus.LOGIN_SCREEN) loginScreen.UpdateConnectionStatus("Login failed.  Did you forget your password?");
        }

        public static void beginLogin(bool newUser)
        {
            string hashedPass = FormsAuthentication.HashPasswordForStoringInConfigFile(username.ToUpper() + password, "SHA1");
            string header = newUser ? "<NEWUSER>" : "<USERNAME>";
            Player.sendMessageToServer(header + username + "," + hashedPass);
        }

        private static void completeLogin()
        {
            loginScreen.Hide();
            newUserScreen.Hide();
            gameFinderScreen.Show();
        }

        private static void initializeGame()
        {
            gameTableScreen.InitializeGame();
            me.initializeGame();
        }

        private static void initializeBiddingPhase()
        {
            gameTableScreen.initializeBiddingPhase();
            me.game.initializeBiddingPhase();
            me.initializeBiddingPhase();
        }

        private static void EndBiddingPhase()
        {
            gameTableScreen.EndBiddingPhase();
        }

        private static void initializePlayingPhase()
        {
            gameTableScreen.initializePlayingPhase();
            me.initializePlayingPhase();
        }

        private static void EndPlayingPhase()
        {
            gameTableScreen.EndPlayingPhase();
        }

        private static void initializeTrick()
        {
            gameTableScreen.initializeTrick();
            me.game.initializeTrick();
            me.initializeTrick();
        }

        private static void EndTrick()
        {
            gameTableScreen.EndTrick();
        }

        private static void leaveGame()
        {
            gameTableScreen.Hide();
            gameFinderScreen.Show();
            gameTableScreen.endGame();
        }

        public static void logout()
        {
            gameTableScreen.CleanUp();
            Application.Exit();
        }

        private static void ActivateWarningTimer(int position, int secondsRemaining)
        {
            threatenedPlayer = position;
            secondsLeft = secondsRemaining;
            warningTimer.Change(0, 1000);
        }

        private static void UpdateWarningTimer(Object state)
        {
            secondsLeft--;
            gameTableScreen.UpdateWarningTimer(threatenedPlayer, secondsLeft);
        }

        public static void Restart()
        {
            loginScreen.Hide();
            newUserScreen.Hide();
            gameFinderScreen.Hide();
            gameTableScreen.Hide();
            aboutScreen.Hide();

            loginScreen = new frm_login();
            newUserScreen = new frm_newuser();
            gameFinderScreen = new frm_gamefinder();
            gameTableScreen = new OpenGLTableScreen();
            aboutScreen = new frm_about();

            socket = new TcpClient();
            messageQueue = new List<string>();
            connecting = false;
            loggingout = false;
            connectTimer = new System.Threading.Timer(Connect);
            warningTimer = new System.Threading.Timer(UpdateWarningTimer);
            secondsLeft = 0;
            me = new Player();

            loginScreen.Show();

            Connect(null);
        }

        public static void Reconnect()
        {
            socket = new TcpClient();
            Connect(null);
            while (!socket.Connected)
            {
                System.Threading.Thread.Sleep(5);
            }
            if (username != string.Empty) beginLogin(false);
        }

        private static void PingServer(Object state)
        {
            Player.sendMessageToServer("<PING>");
        }
    }
}