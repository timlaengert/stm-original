using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
//using Oracle.DataAccess.Client;
//using Oracle.DataAccess.Types;
using System.Data.OracleClient;
using System.Text.RegularExpressions;

namespace ShootServer
{
    class Client
    {
        private static Dictionary<string, Client> connectedClients = new Dictionary<string, Client>();

        public TcpClient socket;
        public IPAddress remoteAddress = null;
        byte[] myReadBuffer;
        protected List<string> ReadQueue = new List<string>();
        protected List<string> WriteQueue = new List<string>();
        /// <summary>
        /// For error log purposes
        /// </summary>
        public string lastMessage = null;

        public enum Status
        {
            LOGIN_SCREEN, LOBBY, LOOKING_FOR_SEAT, PREGAME_READY, PREGAME_NOT_READY, CHOOSING_BID, WAITING_FOR_BID, CHOOSING_TRANSFER_CARDS,
            WAITING_FOR_TRANSFER, THROWING_AWAY_CARDS, WAITING_FOR_THROWAWAY, CHOOSING_CARD, WAITING_FOR_PLAY, SITTING_OUT, LOGGED_OUT, OBSERVING,
            POSTGAME_READY, POSTGAME_NOT_READY
        };

        public Status status = Status.LOGIN_SCREEN;
        public bool Connected = true;

        public StatBox statbox = null;

        public bool human;

        /// <summary>
        /// Game this client is in
        /// </summary>
        public Game game;

        /// <summary>
        /// the username of this client
        /// </summary>
        public string name = null;

        /// <summary>
        /// Client's position at the table
        /// </summary>
        public int position;

        /// <summary>
        /// Client's team number
        /// </summary>
        public int team;

        /// <summary>
        /// Client's current hand
        /// </summary>
        protected List<Card> hand;

        /// <summary>
        /// Client's current bid
        /// </summary>
        public Bid bid;

        /// <summary>
        /// true if the Client has already bid this round
        /// </summary>
        public bool hasBid;

        /// <summary>
        /// true if the Client has already played this trick
        /// </summary>
        public bool hasPlayed;

        /// <summary>
        /// Number of times the client has sent the same message in a row.
        /// </summary>
        private int numberOfRepetitions = 0;
        /// <summary>
        /// Hash value of the current message being received.
        /// </summary>
        private int currentMessageHash;
        /// <summary>
        /// Hash value of the last message received.
        /// </summary>
        private int lastMessageHash = 0;

        protected System.Threading.Timer disconnectTimer;
        protected System.Threading.Timer inactivityTimer;
        /// <summary>
        /// Timeout in milliseconds while actively waiting for user to bid before sending warning
        /// </summary>
        protected const int BIDDING_TIMEOUT = 240000;
        protected const int PLAYING_TIMEOUT = 60000;
        protected const int TIMEOUT_AFTER_WARNING = 60000;
        protected const int DISCONNECT_TIMEOUT = 60000;
        private bool warningDelivered = false;
        /// <summary>
        /// Timeout in milliseconds while user is in lobby
        /// </summary>
        protected const int LOBBY_TIMEOUT = 300000;

        /// <summary>
        /// Generic constructor for AI use
        /// </summary>
        protected Client()
        {
        }

        /// <summary>
        /// Networked constructor for human client use
        /// </summary>
        /// <param name="initSocket"></param>
        public Client(TcpClient initSocket)
        {
            human = true;

            inactivityTimer = new System.Threading.Timer(InactivityTimeout);
            disconnectTimer = new System.Threading.Timer(DisconnectTimeout);
            disconnectTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

            socket = initSocket;
            remoteAddress = ((IPEndPoint)socket.Client.RemoteEndPoint).Address;

            NetworkStream dataStream;

            dataStream = socket.GetStream();
            myReadBuffer = new byte[1024];

            if (dataStream.CanRead)
            {
                try { dataStream.BeginRead(myReadBuffer, 0, myReadBuffer.Length, ReadCallback, dataStream); }
                catch (Exception e)
                {
                    disconnect(dataStream);
                    ServerMain.writeToLog("ERROR READING FROM CLIENT -- " + e.Message, this, true);
                }
            }
            else
            {
                disconnect(dataStream);
            }

            sendHandshake(); // Send websocket handshake.
            ServerMain.writeToLog("SENT HANDSHAKE TO CLIENT.", this, false);
        }

        /// <summary>
        /// Call when a client is unreachable.  Cleans up client variables.
        /// </summary>
        /// <param name="dataStream">the NetworkStream associated with the client</param>
        private void disconnect(NetworkStream dataStream)
        {
            ServerMain.writeToLog("USER DISCONNECTED", this, false);

            disconnectTimer.Change(DISCONNECT_TIMEOUT, DISCONNECT_TIMEOUT);

            try
            {
                //leaveGame();
                dataStream.Close();
                socket.Close();
                //status = Status.DISCONNECTED;
                Connected = false;
                //if (game != null) game.disconnectedPlayers.Add(name, position);

                //removeClient(this); // TODO: change this so that client can reconnect later?
            }
            catch (Exception e)
            {
                ServerMain.writeToLog("ERROR DISCONNECTING USER", this, true);
            }
        }

        /// <summary>
        /// Merge two Client objects when a user is reconnecting.
        /// </summary>
        /// <param name="existingClient">The older client object.</param>
        private void Reconnect(Client existingClient)
        {
            //game.disconnectedPlayers.Remove(name);

            removeClient(existingClient); // take old client object out of the main list.
            MergeClient(existingClient); // absorb its data into this one.
            addClient(this); // add this client to the list.

            // update references to the old client object.
            game.clients.Remove(existingClient);
            game.addClient(this);
            game.players[position] = this;
            if (game.currentPlayer != null && game.currentPlayer.Equals(existingClient)) game.currentPlayer = this;
            Connected = true;

            sendMessageToClient("<CONFIRMLOGIN>" + name + ",1");
            sendMessageToClient(string.Empty); // clear the Write Queue.

            ServerMain.writeToLog("USER RECONNECTED", this, false);
        }

        /// <summary>
        /// Copy instance data from another Client to this one.
        /// </summary>
        /// <param name="source">The source Client object.</param>
        private void MergeClient(Client source)
        {
            this.bid = source.bid;
            this.Connected = source.Connected;
            this.currentMessageHash = source.currentMessageHash;
            this.disconnectTimer = source.disconnectTimer;
            this.game = source.game;
            this.hand = source.hand;
            this.hasBid = source.hasBid;
            this.hasPlayed = source.hasPlayed;
            this.human = source.human;
            this.inactivityTimer = source.inactivityTimer;
            this.lastMessage = source.lastMessage;
            this.lastMessageHash = source.lastMessageHash;
            this.myReadBuffer = source.myReadBuffer;
            this.name = source.name;
            this.numberOfRepetitions = source.numberOfRepetitions;
            this.position = source.position;
            this.ReadQueue = source.ReadQueue;
            // skip remoteAddress
            // skip socket
            this.statbox = source.statbox;
            this.status = source.status;
            this.team = source.team;
            this.warningDelivered = source.warningDelivered;
            this.WriteQueue = source.WriteQueue;
        }

        /// <summary>
        /// Called at the end of a network read.  Don't call this yourself.
        /// </summary>
        /// <param name="asyncresult">client's NetworkStream object</param>
        private void ReadCallback(IAsyncResult asyncresult)
        {
            string transmission;
            NetworkStream dataStream = asyncresult.AsyncState as NetworkStream;
            int bytesRead;

            #region Finish Reading Message
            try
            {
                bytesRead = dataStream.EndRead(asyncresult);

                inactivityTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                warningDelivered = false;

                if (bytesRead == 0)
                {
                    disconnect(dataStream);
                    return;
                }
            }
            catch (Exception e)
            {
                ServerMain.writeToLog("ERROR READING FROM CLIENT -- " + e.Message, this, true);
                disconnect(dataStream);
                return;
            }

            transmission = Encoding.ASCII.GetString(myReadBuffer, 0, bytesRead);
            //ServerMain.writeToLog("RECEIVED TRANSMISSION FROM " + remoteAddress.ToString() + " -- " + transmission);
            #endregion

            #region DoS Stuff
            if (ServerMain.USE_CLIENT_DOS_PROTECTION)
            {
                // Check to make sure we don't suffer a DoS attack -- TODO: Add a time window to cut out false positives
                currentMessageHash = transmission.GetHashCode();
                numberOfRepetitions = currentMessageHash == lastMessageHash ? numberOfRepetitions + 1 : 0;
                lastMessageHash = currentMessageHash;
                if (numberOfRepetitions >= 50)
                {
                    sendErrorToClient("You've repeated the same message over 50 times, and have been disconnected.");
                    disconnect(dataStream);
                    ServerMain.writeToLog("CLIENT AT " + remoteAddress.ToString() + " IS MESSAGE FLOODING.  DISCONNECTING.", this, true);
                    return;
                }
            }
            #endregion

            #region Message Processing
            if (!string.IsNullOrEmpty(transmission))
            {
                ReadQueue.Clear();
                if (ServerMain.CONSOLE_OUTPUT_ON) System.Console.WriteLine("RX " + name + ":\t" + transmission);
                while (transmission.IndexOf("\0") != -1)
                {
                    ReadQueue.Add(transmission.Substring(0, transmission.IndexOf("\0")));
                    transmission = transmission.Substring(transmission.IndexOf("\0") + 1);
                }

                foreach (string message in ReadQueue)
                {
                    lastMessage = message;
                    try { processMessage(message); }
                    catch (Exception e)
                    {
                        ServerMain.writeToLog("ERROR PROCESSING MESSAGE FROM CLIENT -- " + e.Message, this, true);
                    }
                }
            }
            #endregion

            #region Read Next Message
            if (socket.Connected && dataStream.CanRead)
            {
                myReadBuffer = new byte[1024];
                try { dataStream.BeginRead(myReadBuffer, 0, myReadBuffer.Length, ReadCallback, dataStream); }
                catch (Exception e)
                {
                    disconnect(dataStream);
                    ServerMain.writeToLog("ERROR READING FROM CLIENT -- " + e.Message, this, true);
                }
            }
            else
            {
                if (status != Status.LOGGED_OUT) disconnect(dataStream);
            }
            #endregion
        }

        protected virtual void InactivityTimeout(Object state)
        {
            inactivityTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
            if (ServerMain.USE_INACTIVITY_TIMEOUT)
            {
                if (warningDelivered)
                {
                    leaveGame();
                    sendMessageToClient("<FORCELOGOUT>");
                    logout();
                    warningDelivered = false;
                }
                else
                {
                    string warning = "<TIMEOUTWARNING>" + position.ToString() + "," + (TIMEOUT_AFTER_WARNING / 1000);
                    if (position > -1 && game != null)
                        game.sendMessageToClients(warning);
                    else
                        sendMessageToClient(warning);
                    warningDelivered = true;
                    inactivityTimer.Change(TIMEOUT_AFTER_WARNING, TIMEOUT_AFTER_WARNING);
                }
            }
        }

        protected virtual void DisconnectTimeout(Object state)
        {
            disconnectTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
            try
            {
                //if (game != null) game.disconnectedPlayers.Add(name, position);
                leaveGame();
                removeClient(this);
            }
            catch (Exception e)
            {
                ServerMain.writeToLog("ERROR PROCESSING DISCONNECT TIMEOUT -- " + e.Message, this, true);
            }
        }

        /// <summary>
        /// Add a new user to the database.
        /// </summary>
        /// <param name="username">desired username</param>
        /// <param name="password">hash of user's password</param>
        /// <returns>true if user was added successfully, false if user already exists</returns>
        private bool tryAddUser(string username, string password)
        {
            bool ok = false;

            OracleDataReader reader;
            OracleCommand command;

            string selectText = "SELECT USERNAME FROM SHOOTADMIN.USERS WHERE UPPER(USERNAME) = '" + username.ToUpper() + "'";

            string insertText = "INSERT INTO SHOOTADMIN.USERS (USERNAME, PASSHASH, WINS, LOSSES, INCOMPLETE, "
            + "EUCHRES_FOR, EUCHRES_AGAINST, TOTAL_CALLS_FOR, TOTAL_CALLS_AGAINST, TOTAL_POINTS_FOR, TOTAL_POINTS_AGAINST, "
            + "TRICK_DIFFERENTIAL) VALUES ('"
                + username + "', '" + password + "', 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)";

            command = new OracleCommand(selectText, ServerMain.db);

            try
            {
                ServerMain.db.Open();
                reader = command.ExecuteReader();

                ok = !reader.HasRows;

                reader.Close();
            }
            catch (Exception e)
            {
                ServerMain.writeToLog("ERROR READING FROM DATABASE -- " + e.Message, this, true);
            }
            finally
            {
                ServerMain.db.Close();
            }

            if (ok) ServerMain.writeToDB(insertText);

            return ok;
        }

        /// <summary>
        /// Validate a username
        /// </summary>
        /// <param name="username">username to check</param>
        /// <param name="password">hash of password</param>
        /// <returns>true if username and password are valid</returns>
        private bool validateUsername(ref string username, string password)
        {
            string dbUsername;
            string dbPassword;
            bool matches = false;

            OracleDataReader reader;
            OracleCommand command;
            string commandText = "SELECT USERNAME, PASSHASH FROM SHOOTADMIN.USERS WHERE UPPER(USERNAME) = '" + username.ToUpper() + "'";
            command = new OracleCommand(commandText, ServerMain.db);

            try
            {
                ServerMain.db.Open();
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    dbUsername = reader.GetString(0);
                    dbPassword = reader.GetString(1);
                    username = dbUsername;
                    matches = password == dbPassword;
                }
                else
                {
                    matches = false;
                }
            }
            catch (Exception e)
            {
                ServerMain.writeToLog("ERROR READING FROM DATABASE -- " + e.Message, this, true);
            }
            finally
            {
                ServerMain.db.Close();
            }

            return matches;
        }

        /// <summary>
        /// process a message received from the client
        /// </summary>
        /// <param name="message">a message</param>
        protected virtual void processMessage(string message)
        {
            Regex format = new Regex("^<[A-Z]+>.*$");

            if (!format.IsMatch(message))
            {
                sendErrorToClient("Transmission format incorrect in message -- " + message);
                return;
            }

            int start = message.IndexOf("<") + 1;
            int end = message.IndexOf(">") - 1;
            int length = end - start + 1;
            string type = message.Substring(start, length);
            string content = message.Substring(end + 2);

            string username;
            string password;
            int playerIndex;
            int readyStatus;
            bool successful;

            if (type != "PING" && type != "NEWUSER" && type != "USERNAME" && name == null)
            {
                sendErrorToClient("You have not been authenticated. Log in first.");
                return;
            }

            // each case contains an explanation and the format of the message. An asterisk indicates the pattern may repeat.
            switch (type)
            {
                // a new user is sending login information.
                // <username><passwordhash>
                case "NEWUSER":
                    format = new Regex("^[A-Za-z0-9]{4,16},[A-Za-z0-9]{40}$");

                    if (!format.IsMatch(content))
                    {
                        sendErrorToClient("Login credentials are invalid. Usernames must be between 4 and 16 alphanumeric characters.");
                        break;
                    }

                    if (status != Status.LOGIN_SCREEN)
                    {
                        sendErrorToClient("Unable to create new user.  You are already logged in.");
                        break;
                    }

                    username = content.Substring(0, content.IndexOf(","));
                    password = content.Substring(content.IndexOf(",") + 1);
                    successful = !ServerMain.USE_DB || tryAddUser(username, password);
                    if (successful)
                    {
                        name = username;
                        addClient(this);
                        status = Status.LOBBY;
                        sendMessageToClient("<CONFIRMLOGIN>" + username + ",0");
                        ServerMain.writeToLog("NEW USER CREATED", this, false);
                        inactivityTimer.Change(LOBBY_TIMEOUT, LOBBY_TIMEOUT);
                    }
                    else
                    {
                        sendErrorToClient("Username is already taken.");
                    }
                    break;

                // an existing user is sending credentials.
                // <username><passwordhash>
                case "USERNAME":
                    format = new Regex("^[A-Za-z0-9]{4,16},[A-Za-z0-9]{40}$");

                    if (!format.IsMatch(content))
                    {
                        sendErrorToClient("Login credentials are invalid. Usernames must be between 4 and 16 alphanumeric characters.");
                        break;
                    }

                    //if (status != Status.LOGIN_SCREEN)
                    //{
                    //    sendErrorToClient("Unable to log in.  You are already logged in.");
                    //    break;
                    //}

                    username = content.Substring(0, content.IndexOf(","));
                    password = content.Substring(content.IndexOf(",") + 1);
                    successful = !ServerMain.USE_DB || validateUsername(ref username, password);

                    lock (connectedClients)
                        if (connectedClients.ContainsKey(username)) // user is logged in but may be disconnected.
                        {
                            Client existingClient = getClient(username);
                            if (!existingClient.Connected) Reconnect(existingClient); // user is disconnected.
                            else sendErrorToClient("That user is already signed in."); // user is still active.

                            break;
                        }

                    if (successful)
                    {
                        name = username;
                        addClient(this);
                        status = Status.LOBBY;

                        sendMessageToClient("<CONFIRMLOGIN>" + username + ",0");

                        ServerMain.writeToLog("USER CONNECTED", this, false);
                        inactivityTimer.Change(LOBBY_TIMEOUT, LOBBY_TIMEOUT);
                    }
                    else
                    {
                        sendErrorToClient("Login information is invalid.  Did you forget your password?");
                    }
                    break;

                // a client is requesting the list of joinable games.
                // empty
                case "GETGAMESLIST":
                    //if (status != Status.LOBBY)
                    //{
                    //    sendErrorToClient("Can't send games list.  You aren't in the lobby.");
                    //    break;
                    //}

                    sendMessageToClient("<GAMESLIST>" + buildGamesList());

                    sendMessageToClient("<USERSLIST>" + buildUsersList());
                    inactivityTimer.Change(LOBBY_TIMEOUT, LOBBY_TIMEOUT);
                    break;

                // a client is requesting to host a new game
                // <gamename>
                case "REQUESTHOST":
                    format = new Regex("^[A-Za-z0-9]+$");  // TODO: add length restriction

                    if (!format.IsMatch(content))
                    {
                        sendErrorToClient("Game name format is incorrect.");
                        break;
                    }

                    if (status != Status.LOBBY && status != Status.POSTGAME_READY)
                    {
                        sendErrorToClient("Can't create game.  You aren't in the lobby.");
                        break;
                    }

                    Game newGame = new Game(name);
                    lock (ServerMain.gamesList) ServerMain.gamesList.Add(newGame.id, newGame);
                    ServerMain.writeToLog("NEW GAME CREATED -- " + newGame.id + ":" + newGame.name, this, false);
                    joinGame(newGame);
                    break;

                // a client is requesting to join an existing game
                // <gameid (int)>
                case "REQUESTJOIN":
                    format = new Regex("^[0-9]+$");

                    if (!format.IsMatch(content))
                    {
                        sendErrorToClient("Game id format is incorrect.");
                        break;
                    }

                    if (status != Status.LOBBY)
                    {
                        sendErrorToClient("Can't join game.  You aren't in the lobby.");
                        break;
                    }

                    Game gameToJoin;
                    int gameNumber = int.Parse(content);
                    lock (ServerMain.gamesList)
                    {
                        if (ServerMain.gamesList.ContainsKey(gameNumber)) gameToJoin = ServerMain.gamesList[gameNumber];
                        else
                        {
                            sendErrorToClient("Game does not exist.");
                            break;
                        }
                    }

                    if (gameToJoin.inProgress && !gameToJoin.disconnectedPlayers.ContainsKey(name)) sendErrorToClient("Game has already started.");
                    else if (gameToJoin.numPlayers >= 6) sendErrorToClient("Game is full.");
                    else joinGame(gameToJoin);

                    break;

                // a client is requesting which seats are available
                // empty
                case "GETSEATLIST": // send available seats
                    if (status != Status.LOOKING_FOR_SEAT)
                    {
                        sendErrorToClient("Can't send seat list.  You don't seem to be looking for a seat.");
                        break;
                    }

                    sendMessageToClient("<SEATLIST>" + game.getSeatList());
                    break;

                // a client is requesting a seat
                // <position (int 1)>
                case "REQUESTSEAT":
                    format = new Regex("^[0-5]$");

                    if (!format.IsMatch(content))
                    {
                        sendErrorToClient("Seat number format is incorrect");
                        break;
                    }

                    if (status != Status.LOOKING_FOR_SEAT)
                    {
                        sendErrorToClient("Can't sit down.  You don't seem to be looking for a seat.");
                        break;
                    }

                    int seatNum = int.Parse(content);
                    if (game.getPlayer(seatNum) == null)
                    {
                        status = Status.PREGAME_NOT_READY;
                        game.addPlayer(this, seatNum);
                    }
                    else
                    {
                        sendErrorToClient("Seat is occupied.");
                    }
                    break;

                // receive a chat message from client (and forward it to the other clients)
                // <message>
                case "CHATMESSAGE":
                    game.sendMessageToClients("<CHATMESSAGE>" + name + "," + content);
                    break;

                // receive a ready status change from a client.
                // <ready (int 1)>
                case "READYSTATUS":
                    format = new Regex("^[01]$");

                    if (!format.IsMatch(content))
                    {
                        sendErrorToClient("Ready status format is incorrect");
                        break;
                    }

                    if (status != Status.PREGAME_NOT_READY && status != Status.PREGAME_READY
                        && status != Status.POSTGAME_NOT_READY && status != Status.POSTGAME_READY)
                    {
                        sendErrorToClient("Can't set ready status.  You're not in pregame.");
                        break;
                    }

                    bool postgame = status == Status.POSTGAME_NOT_READY || status == Status.POSTGAME_READY;
                    readyStatus = int.Parse(content.Substring(0, 1));

                    if (readyStatus == 1)
                    {
                        if (status == Status.PREGAME_NOT_READY) status = Status.PREGAME_READY;
                        else if (status == Status.POSTGAME_NOT_READY) status = Status.POSTGAME_READY;
                        else
                        {
                            sendErrorToClient("Can't set ready status.  Your status is already set to Ready.");
                            break;
                        }
                    }
                    else
                    {
                        if (status == Status.PREGAME_READY) status = Status.PREGAME_NOT_READY;
                        else if (status == Status.POSTGAME_READY) status = Status.POSTGAME_NOT_READY;
                        else
                        {
                            sendErrorToClient("Can't set ready status.  Your status is already set to Not Ready.");
                        }
                    }

                    game.sendMessageToClients("<READYSTATUS>" + position + readyStatus);

                    if (game.allPlayersReady())
                    {
                        if (postgame)
                        {
                            int pos = position;
                            string gamename = game.name;
                            leaveGame();
                            processMessage("<REQUESTHOST>" + gamename);
                            processMessage("<REQUESTSEAT>" + pos);
                            foreach (Client client in game.players)
                            {
                                if (client != null && client.human && !client.Equals(this))
                                {
                                    pos = client.position;
                                    client.leaveGame();
                                    client.processMessage("<REQUESTJOIN>" + this.game.id);
                                    client.processMessage("<REQUESTSEAT>" + pos);
                                }
                            }
                        }
                        game.startGame();
                    }
                    break;

                // receive a bid from a client - note that if tricks = 0 then trump may be omitted (00 = PASS)
                // <tricks (int 1)><shootnum (int 1)><trump>
                case "BID":
                    format = new Regex("^[0-9][0-6](High|Low|Hearts|Diamonds|Clubs|Spades)?$");

                    if (!format.IsMatch(content))
                    {
                        sendErrorToClient("Bid format is incorrect");
                        break;
                    }

                    if (status != Status.CHOOSING_BID)
                    {
                        sendErrorToClient("You're trying to bid, but it's not your turn.");
                        break;
                    }

                    Bid bid = Bid.FromString(content);
                    bid.bidder = this;
                    game.receiveBid(bid);
                    break;

                // receive a card from a client
                // <cardrank (char)><cardsuit (char)>
                case "PLAYCARD":
                    format = new Regex("^[90JQKA][HDCS]$");

                    if (!format.IsMatch(content))
                    {
                        sendErrorToClient("Card format is incorrect");
                        break;
                    }

                    if (status != Status.CHOOSING_CARD)
                    {
                        sendErrorToClient("It's not your turn to play a card.");
                        break;
                    }

                    Card card = Card.FromString(content);

                    tryCard(card);
                    break;

                // transfer a card from one client to another
                // <toposition (int 1)><cardrank (char)><cardsuit (char)>
                case "TRANSFERCARD":
                    format = new Regex("^[0-5][90JQKA][HDCS]$");

                    if (!format.IsMatch(content))
                    {
                        sendErrorToClient("Transfer format is incorrect");
                        break;
                    }

                    if (status != Status.CHOOSING_TRANSFER_CARDS)
                    {
                        sendErrorToClient("You're not supposed to be choosing a transfer card.");
                        break;
                    }

                    playerIndex = int.Parse(content.Substring(0, 1));
                    card = Card.FromString(content.Substring(1, 2));
                    Client shooter = game.players[playerIndex];

                    if (shooter.status == Status.WAITING_FOR_TRANSFER && hand.Contains(card))
                    {
                        hand.Remove(card);
                        shooter.hand.Add(card);

                        sendMessageToClient("<CONFIRMTRANSFER>" + position + shooter.position + card.ToString());
                        status = Status.SITTING_OUT;

                        shooter.sendMessageToClient("<CONFIRMTRANSFER>" + position + shooter.position + card.ToString());
                        foreach (Client player in game.players)
                        {
                            if (!player.Equals(this) && !player.Equals(shooter))
                            {
                                player.sendMessageToClient("<CONFIRMTRANSFER>" + position + shooter.position);
                            }
                        }

                        if (shooter.hand.Count == 10) // TODO: better way to figure out if transfers are finished?
                        {
                            foreach (Client player in game.players)
                            {
                                if (player.status != Status.SITTING_OUT)
                                {
                                    if (player.Equals(shooter)) player.status = Status.THROWING_AWAY_CARDS;
                                    else if (player.Equals(this)) player.status = Status.SITTING_OUT;
                                    else player.status = Status.WAITING_FOR_THROWAWAY;
                                }
                            }
                            shooter.requestThrowaway();
                        }
                    }
                    else
                    {
                        sendErrorToClient("Error transferring card.");
                    }
                    break;

                // throwaway a card from a client
                // <cardrank (char)><cardsuit (char)>
                case "THROWAWAYCARD":
                    format = new Regex("^[90JQKA][HDCS]$");

                    if (!format.IsMatch(content))
                    {
                        sendErrorToClient("Card format is incorrect");
                        break;
                    }

                    if (status != Status.THROWING_AWAY_CARDS)
                    {
                        sendErrorToClient("You're not supposed to be throwing away cards.");
                        break;
                    }

                    card = Card.FromString(content);
                    int finished = -1;

                    if (status == Status.THROWING_AWAY_CARDS && hand.Contains(card))
                    {
                        hand.Remove(card);
                        if (hand.Count == 8) finished = 1;
                        else finished = 0;
                        sendMessageToClient("<CONFIRMTHROWAWAY>" + position + finished + card.ToString());

                        if (finished == 1) game.startTrick();

                    }
                    else
                    {
                        sendErrorToClient("Error throwing away card.");
                    }
                    break;

                // client is leaving the game
                // empty
                case "LEAVINGGAME":
                    if (game == null)
                    {
                        sendErrorToClient("Can't leave game.  You're not in one.");
                        break;
                    }

                    sendMessageToClient("<CONFIRMLEAVE>");
                    leaveGame();
                    break;

                // client is logging out
                // empty
                case "LOGGINGOUT":
                    if (name == null)
                    {
                        sendErrorToClient("Can't log out.  You aren't logged in.");
                        break;
                    }

                    logout();
                    break;

                // no matching case was found for the sent header
                default:
                    sendMessageToClient("<MESSAGENOTRECOGNIZED>");
                    break;
            }
        }

        /// <summary>
        /// Assembles a list of online users
        /// </summary>
        /// <returns>comma-separated list of users</returns>
        private string buildUsersList()
        {
            string result = string.Empty;

            lock (connectedClients)
                foreach (string player in connectedClients.Keys)
                    result += player + ",";

            return result;
        }

        /// <summary>
        /// Assembles a list of joinable games
        /// </summary>
        /// <returns>comma-separated list of games</returns>
        private string buildGamesList()
        {
            string result = string.Empty;

            lock (ServerMain.gamesList)
                foreach (Game g in ServerMain.gamesList.Values)
                {
                    if (g.numPlayers < 6) //only show games with room for new players
                    {
                        result += g.id + "," + g.name + "," + g.numPlayers + "," + (g.inProgress ? "1" : "0") + ",";
                    }
                }

            return result;
        }

        private void catchUp(EvisceratorAI departingAI)
        {
            game = departingAI.game;

            position = departingAI.position;
            team = departingAI.team;
            hasBid = departingAI.hasBid;
            hasPlayed = departingAI.hasPlayed;

            status = departingAI.status;

            hand = departingAI.hand;
            if (hand != null) foreach (Card card in hand)
                    if (card != null) card.player = this;

            bid = departingAI.bid;
            if (bid != null) bid.bidder = this;
        }

        private string buildCatchupFeed(EvisceratorAI departingAI)
        {
            string feed = string.Empty;
            Game gameBeingJoined = departingAI.game;
            int alreadyPlayed = departingAI.hasPlayed ? 1 : 0;
            int alreadyBid = departingAI.hasBid ? 1 : 0;

            // position, team, hasBid, hasPlayed
            //feed += departingAI.position.ToString() + departingAI.team.ToString(); // + alreadyBid.ToString() + alreadyPlayed.ToString();

            // status
            //feed += departingAI.status.ToString() + ";";

            // players
            foreach (Client player in gameBeingJoined.players)
                //if (!player.Equals(departingAI)) 
                feed += player.position.ToString() + player.name + "," + player.status.ToString() + ",";
            feed += ";";

            // cards in hand
            foreach (Card card in departingAI.hand) feed += card.ToString();
            feed += ";";

            // score
            feed += gameBeingJoined.score[0] + "-" + gameBeingJoined.score[1] + ";";

            // tricks
            feed += gameBeingJoined.tricks[0].ToString() + gameBeingJoined.tricks[1].ToString() + ";";

            // bids
            foreach (Bid bid in gameBeingJoined.bids) feed += bid.bidder.position + bid.ToString() + ",";
            feed += ";";

            // trump
            if (game.currentTrump != null) feed += game.currentTrump.ToString();
            feed += ";";

            // cards played
            if (gameBeingJoined.playedCards != null)
                foreach (Card card in gameBeingJoined.playedCards)
                    feed += card.player.position + card.ToString() + (card.player.Equals(game.highCard.player) ? "1" : "0") + ",";
            feed += ";";

            return feed;
        }

        /// <summary>
        /// Join a game that hasn't started
        /// </summary>
        /// <param name="gameToJoin">the game to join</param>
        public void joinGame(Game gameToJoin)
        {
            if (GetType() != typeof(EvisceratorAI)) gameToJoin.numPlayers++;
            gameToJoin.addClient(this);
            game = gameToJoin;
            status = Status.LOOKING_FOR_SEAT;
            sendMessageToClient("<CONFIRMJOIN>" + game.id);
            initializeGame();

            if (game.disconnectedPlayers.ContainsKey(name))
            {
                Client evictee = game.players[game.disconnectedPlayers[name]]; // find old seat number
                game.disconnectedPlayers.Remove(name);
                if (!(evictee is EvisceratorAI)) throw new Exception(); // make sure evicted player is an AI
                game.addPlayer(this, evictee.position);
                catchUp((EvisceratorAI)evictee); // set server-side state data for client
                lock (game.clients) game.clients.Remove(evictee);

                if (game.currentPlayer.Equals(evictee)) game.currentPlayer = this;

                string catchUpFeed = buildCatchupFeed((EvisceratorAI)evictee); // collect state information
                sendMessageToClient("<CATCHUP>" + catchUpFeed); // send state to client
                if (ServerMain.CONSOLE_OUTPUT_ON) System.Console.WriteLine("SENT NEW CATCHUP FEED: " + catchUpFeed);

                ServerMain.writeToLog("USER REJOINED GAME -- " + gameToJoin.name, this, false);
            }
            else if (human)
            {
                ServerMain.writeToLog("USER JOINED GAME -- " + gameToJoin.name, this, false);
            }
        }

        /// <summary>
        /// Leave a game
        /// </summary>
        public virtual void leaveGame()
        {
            if (game == null) return;

            if (!human)
            {
                game.players[position] = null;
                game.sendMessageToClients("<CONFIRMSEAT>" + position + "NUL");
                return;
            }

            lock (game) // TODO: Is this lock necessary?
            {
                if (status != Status.OBSERVING)
                {
                    if (game.inProgress)
                    {
                        string sql = "UPDATE SHOOTADMIN.USERS SET INCOMPLETE = INCOMPLETE + 1 WHERE USERNAME = '" + name + "'";
                        if (ServerMain.USE_DB) ServerMain.writeToDB(sql);

                        game.disconnectedPlayers.Add(name, position);
                        EvisceratorAI newAI = new EvisceratorAI(game, hand, EvisceratorProfile.DEFAULT);
                        newAI.position = position;
                        newAI.joinGame(game);
                        game.addPlayer(newAI, position);
                        newAI.adjustForContext(status);
                    }
                    else if (position != -1)
                    {
                        game.players[position] = null;
                        game.sendMessageToClients("<CONFIRMSEAT>" + position + "NUL");
                    }
                    game.numPlayers--;
                    if (game.numPlayers == 0)
                    {
                        lock (ServerMain.gamesList) ServerMain.gamesList.Remove(game.id);
                    }
                }

                lock (game.clients) game.clients.Remove(this);
                game = null;
                status = Status.LOBBY;
            }
        }

        /// <summary>
        /// Call when a client is logging out to clean up variables.
        /// </summary>
        private void logout()
        {
            // TODO: Do I need a lock here?
            ServerMain.writeToLog("USER LOGGED OUT", this, false);
            leaveGame();
            try
            {
                socket.GetStream().Close();
                socket.Close();
            }
            catch (Exception e)
            {
            }
            removeClient(this);
            status = Status.LOGGED_OUT;
        }

        /// <summary>
        /// send a message to the client
        /// </summary>
        /// <param name="message">message to send</param>
        /// <param name="clientsInvolved">any clients required to assemble a message</param>
        public virtual void sendMessageToClient(string message)
        {
            NetworkStream dataStream;

            message += "\0";

            if (!Connected)
            {
                WriteQueue.Add(message);
                if (ServerMain.CONSOLE_OUTPUT_ON) System.Console.WriteLine("MSG QUEUED: " + message);
                return;
            }
            else if (WriteQueue.Count > 0)
            {
                string prefix = string.Empty;
                string suffix = string.Empty;

                if (message.Contains("<CONFIRMLOGIN>")) prefix = message;
                else suffix = message;

                message = string.Empty;

                foreach (string savedMessage in WriteQueue) if (savedMessage.Contains("<CONFIRMLOGIN>")) prefix = message;
                else message += savedMessage;

                WriteQueue.Clear();
                message = prefix + message + suffix;
            }

            if (ServerMain.CONSOLE_OUTPUT_ON) System.Console.WriteLine("TX " + name + ":\t" + message);
            byte[] myWriteBuffer = Encoding.ASCII.GetBytes(message);

            try { dataStream = socket.GetStream(); }
            catch (Exception e)
            {
                WriteQueue.Add(message);
                if (ServerMain.CONSOLE_OUTPUT_ON) System.Console.WriteLine("MSG QUEUED: " + message);

                ServerMain.writeToLog("ERROR WRITING TO CLIENT -- " + e.Message, this, true);
                return;
            }

            if (socket.Connected && dataStream.CanWrite)
            {
                try { dataStream.Write(myWriteBuffer, 0, myWriteBuffer.Length); }
                catch (Exception e)
                {
                    WriteQueue.Add(message);
                    if (ServerMain.CONSOLE_OUTPUT_ON) System.Console.WriteLine("MSG QUEUED: " + message);

                    disconnect(dataStream);
                    ServerMain.writeToLog("ERROR WRITING TO CLIENT -- " + e.Message, this, true);
                }
            }
            else
            {
                disconnect(dataStream);
            }
        }

        /// <summary>
        /// Send an error message to the client.
        /// </summary>
        /// <param name="error">an error message</param>
        public void sendErrorToClient(string error)
        {
            string client;
            if (name != null) client = name;
            else client = ((IPEndPoint)socket.Client.RemoteEndPoint).Address.ToString();
            ServerMain.writeToLog("SENDING ERROR TO USER -- " + error.ToUpper(), this, true);
            sendMessageToClient("<ERROR>" + error);
        }

        /// <summary>
        /// reset all game variables
        /// </summary>
        public virtual void initializeGame()
        {
            position = -1;
            statbox = new StatBox();
        }

        public virtual void EndGame()
        {
            status = human? Status.POSTGAME_NOT_READY : Status.POSTGAME_READY;
            //if (!human) leaveGame();
        }

        /// <summary>
        /// reset all round variables
        /// </summary>
        public virtual void initializeRound()
        {
            hand = new List<Card>();
            bid = null;
            hasBid = false;
        }

        /// <summary>
        /// Take care of round-end business
        /// </summary>
        public virtual void endRound()
        {
        }

        /// <summary>
        /// reset all trick variables
        /// </summary>
        public virtual void initializeTrick()
        {
            hasPlayed = false;
        }

        /// <summary>
        /// Take care of trick-end business
        /// </summary>
        public virtual void endTrick()
        {
            game.playedCards = null;
        }

        /// <summary>
        /// add a card to player's hand
        /// </summary>
        /// <param name="card">card to add</param>
        public void acceptCard(Card card)
        {
            hand.Add(card);
        }

        /// <summary>
        /// transmit dealt cards to client
        /// </summary>
        public void showCardsToClient()
        {
            string message = "<NEWHAND>";

            foreach (Card card in hand)
            {
                message += card.ToString();
            }

            sendMessageToClient(message);
        }

        /// <summary>
        /// request a bid from the client and notify other clients this client is bidding
        /// </summary>
        public void requestBid()
        {
            foreach (Client player in game.players)
            {
                if (player.Equals(this)) player.status = Status.CHOOSING_BID;
                else player.status = Status.WAITING_FOR_BID;
            }
            game.sendMessageToClients("<REQUESTBID>" + position);
            inactivityTimer.Change(BIDDING_TIMEOUT, BIDDING_TIMEOUT);
        }

        public void requestCard()
        {
            foreach (Client player in game.players)
            {
                if (player.status != Client.Status.SITTING_OUT)
                {
                    if (player.Equals(this)) player.status = Client.Status.CHOOSING_CARD;
                    else player.status = Client.Status.WAITING_FOR_PLAY;
                }
            }
            game.sendMessageToClients("<REQUESTCARD>" + position);
            inactivityTimer.Change(PLAYING_TIMEOUT, PLAYING_TIMEOUT);
        }

        public void requestThrowaway()
        {
            game.sendMessageToClients("<REQUESTTHROWAWAY>" + position);
            inactivityTimer.Change(BIDDING_TIMEOUT, BIDDING_TIMEOUT);
        }

        public void requestTransfer()
        {
            game.sendMessageToClients("<REQUESTTRANSFER>" + position.ToString() + game.highBid.bidder.position.ToString());
            inactivityTimer.Change(BIDDING_TIMEOUT, BIDDING_TIMEOUT);
        }

        /// <summary>
        /// Try to play a card
        /// </summary>
        /// <param name="card">card being played</param>
        public void tryCard(Card card)
        {
            string reason = "It's not your turn.";
            bool winningCard = false;

            if (game.currentPlayer.Equals(this) && Rules.isCardLegal(hand, card, game.leadCard, game.currentTrump, out reason))
            {
                card.player = this;
                game.playedCards.Add(card);
                hand.Remove(card);
                hasPlayed = true;

                if (game.leadCard == null)
                {
                    game.leadCard = card;
                }
                if (game.highCard == null || Rules.isCardBetter(
                    game.highCard, card, game.currentTrump,
                    game.leadCard.getContextualSuit(game.currentTrump)))
                {
                    game.highCard = card;
                    winningCard = true;
                }


                game.sendMessageToClients("<CONFIRMCARD>" + position + card.ToString() + (winningCard ? "1" : "0"));

                game.cardCounter.playCard(card);

                game.currentPlayer = game.players[(game.currentPlayer.position + 1) % game.players.Length]; // go to the next player in order

                while (game.currentPlayer.status == Status.SITTING_OUT) // someone is shooting, so skip people who aren't playing
                {
                    game.currentPlayer = game.players[(game.currentPlayer.position + 1) % game.players.Length]; // go to the next player in order
                }

                game.requestNextCard();
            }
            else
            {
                sendMessageToClient("<ERROR>" + reason);
            }
        }

        /// <summary>
        /// Add a new client to the main list
        /// </summary>
        /// <param name="client">A client that just logged in</param>
        private static void addClient(Client client)
        {
            lock (connectedClients)
            {
                if (client.name != null && !connectedClients.ContainsKey(client.name)) connectedClients.Add(client.name, client);
            }
        }

        /// <summary>
        /// Remove a client from the main list
        /// </summary>
        /// <param name="client">A client that is logging out</param>
        private static void removeClient(Client client)
        {
            lock (connectedClients)
            {
                if (client.name != null && connectedClients.ContainsKey(client.name)) connectedClients.Remove(client.name);
            }
        }

        /// <summary>
        /// Get an online client from their username
        /// </summary>
        /// <param name="username">client's username</param>
        /// <returns>the corresponding client object</returns>
        public static Client getClient(string username)
        {
            Client result;

            lock (connectedClients)
                result = connectedClients.ContainsKey(username) ? connectedClients[username] : null;

            return result;
        }

        private void sendHandshake()
        {
            string handshake =
                   "HTTP/1.1 101 Web Socket Protocol Handshake\r\n" +
                    "WebSocket-Location: ws://" + socket.Client.LocalEndPoint.ToString() + "/server\r\n" +
                   "\r\n";

            sendMessageToClient(handshake);
        }

        /// <summary>
        /// Find out if two clients are identical.
        /// </summary>
        /// <param name="obj">client to compare to</param>
        /// <returns>true if identical; false otherwise</returns>
        public override bool Equals(object obj)
        {
            Client otherClient;

            if (obj.GetType() != GetType()) return false; // different types - definitely not equal

            if (obj.GetType() == GetType() && GetType() == typeof(EvisceratorAI)) // both objects are AIs - can't compare usernames
            {
                return ((EvisceratorAI)this).id == ((EvisceratorAI)obj).id; // compare IDs
            }

            otherClient = (Client)obj;

            return otherClient.name == name; // assume clients are human - compare usernames
        }

        /// <summary>
        /// Get a client's hash code.
        /// </summary>
        /// <returns>client's hash code</returns>
        public override int GetHashCode()
        {
            // username is unique for human clients - therefore should give an acceptable hash code.
            // for AIs, use id
            return human ? name.GetHashCode() : ((EvisceratorAI)this).id;
        }
    }
}
