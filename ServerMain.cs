using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
//using Oracle.DataAccess.Client;
//using Oracle.DataAccess.Types;
using System.Data.OracleClient;

namespace ShootServer
{
    class ServerMain
    {
        // Network variables
        private const bool USE_LOOPBACK_ADDRESS = true;
        public const bool SIMULATION_MODE = false;
        public const bool CONSOLE_OUTPUT_ON = false;
        public const bool USE_DB = false; // change to false if no database access
        public const bool USE_CLIENT_DOS_PROTECTION = false;
        public const bool USE_INACTIVITY_TIMEOUT = true;

        private static IPAddress[] IP_ADDR = USE_LOOPBACK_ADDRESS ? new IPAddress[] { IPAddress.Loopback } : Dns.GetHostEntry(string.Empty).AddressList;
        private static int PORT_NUM = 38521;
        private static TcpListener listener = new TcpListener(IP_ADDR[0], PORT_NUM);

        // Database variables
        private static string connectionString = "user id=shootuser;password=stinger;data source=laengert-hs/XE";
        public static OracleConnection db = new OracleConnection(connectionString);

        // Log variables
        private const string LOGFILENAME = "log.txt";
        private static StreamWriter logWriter;

        // client id of next AI player
        public static int nextAI = 0;

        // list of games
        public static Dictionary<int, Game> gamesList = new Dictionary<int, Game>();
        public static int gameCounter = 0;

        static ServerMain()
        {
            logWriter = File.AppendText(LOGFILENAME);
        }

        /// <summary>
        /// New and improved event loop
        /// </summary>
        /// <param name="args">command line arguments</param>
        static void Main(string[] args)
        {
            if (SIMULATION_MODE) runSimulation();

            try
            {
                listener.Start(); // start waiting for client connections
                writeToLog("SERVER STARTED", null, false);
                if (ServerMain.CONSOLE_OUTPUT_ON) System.Console.WriteLine("Listener started.");
            }
            catch (Exception e)
            {
                writeToLog("SERVER FAILED TO START -- " + e.Message, null, true);
            }

            while (true) waitForNewClient(); // this thread can just collect new clients.
        }

        /// <summary>
        /// Block until a new client connects and then process them
        /// </summary>
        private static void waitForNewClient()
        {
            TcpClient newSocket;
            Client newClient;
            IPEndPoint remoteEndPoint;

            try
            {
                newSocket = listener.AcceptTcpClient();
                remoteEndPoint = (IPEndPoint)newSocket.Client.RemoteEndPoint;
                writeToLog("NEW CLIENT CONNECTED -- " + remoteEndPoint.Address + ":" + remoteEndPoint.Port, null, false);
                if (CONSOLE_OUTPUT_ON) System.Console.WriteLine("RX : New Client connected - {0}:{1}", remoteEndPoint.Address, remoteEndPoint.Port);
                newClient = new Client(newSocket);
            }
            catch (Exception e)
            {
                writeToLog("ERROR ADDING CLIENT -- " + e.Message, null, true);
            }
        }

        /// <summary>
        /// Write an entry to the log
        /// </summary>
        /// <param name="text">The message for the log.</param>
        /// <param name="client">The relevant client.</param>
        /// <param name="error">True if an error resulted in this message.</param>
        public static void writeToLog(string text, Client client, bool error)
        {
            string user = client == null ? "UNKNOWN USER" : (client.name == null ? client.remoteAddress.ToString() : client.name);
            user = user.PadRight(16);

            lock (logWriter)
            {
                logWriter.WriteLine(DateTime.Now.ToString("G") + " " + user + " : " + text);
                if (error && client != null && client.lastMessage != null)
                    logWriter.WriteLine("\t\tLAST MESSAGE FROM CLIENT: " + client.lastMessage);
                logWriter.Flush();
            }
        }

        /// <summary>
        /// Begin a new AI simulation game.
        /// </summary>
        private static void runSimulation()
        {
            Game game = new Game("SIMULATION");
            game.startGame();
        }

        /// <summary>
        /// Write a sql statement to the database.
        /// </summary>
        /// <param name="sql">a sql nonquery statement.</param>
        public static void writeToDB(string sql)
        {
            OracleCommand command;
            string commandText = sql;
            command = new OracleCommand(commandText, db);

            try
            {
                ServerMain.db.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                writeToLog("ERROR WRITING TO DATABASE -- " + e.Message, null, true);
            }
            finally
            {
                ServerMain.db.Close();
            }
        }
    }
}
