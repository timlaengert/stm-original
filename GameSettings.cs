using System;
using System.Collections.Generic;
using System.Text;

namespace ShootClient
{
    public class GameSettings
    {
        private int numPlayersPerTeam;

        private int numDuplicateCards;

        private int scoreNeededToWin;

        /**
         * Creates a new GameSettings object in case you don't want to use the
         * default settings for the rules in a shoot the moon game. Currently
         * 
         * @param numPlayersPerTeam
         *            the number of players on each team
         * @param numDuplicateCards
         *            the number of copies of each card in the deck
         */
        private GameSettings(int numPlayersPerTeam, int numDuplicateCards, int scoreNeededToWin)
        {
            this.numPlayersPerTeam = numPlayersPerTeam;
            this.numDuplicateCards = numDuplicateCards;
            this.scoreNeededToWin = scoreNeededToWin;

            //if (getDeckSize() % numPlayersPerTeam != 0)
            //{
            //    throw new Exception("GameSettings created illegaly, cards would not deal evenly.");
            //}
        }

        /**
         * The settings for a standard four player game
         * (2 players per team, 2 euchre decks, 51 point game)
         */
        public static readonly GameSettings FOURPLAYER = new GameSettings(2, 1, 51);

        /**
         * The settings for a standard six player game
         * (3 players per team, 2 euchre decks, 51 point game)
         */
        public static readonly GameSettings SIXPLAYER = new GameSettings(3, 2, 51);

        /**
         * The settings for a (standard?) eight player game
         * (4 players per team, 3 euchre decks, 51 point game)
         */
        public static readonly GameSettings EIGHTPLAYER = new GameSettings(4, 3, 51);

        public static readonly GameSettings SIMULATION = new GameSettings(3, 2, 15100);

        /**
         * @return the size of a deck given the current settings
         */
        public int getDeckSize()
        {
            return Suit.allSuits.Length * Rank.allRanks.Length * numDuplicateCards;
        }

        /**
         * @return the number of copies of each card in the deck
         */
        public int getNumDuplicateCards()
        {
            return numDuplicateCards;
        }

        /**
         * @return the number of players on each team
         */
        public int getNumPlayersPerTeam()
        {
            return numPlayersPerTeam;
        }

        /**
         * @return the score needed to win a game
         */
        public int getScoreNeededToWin()
        {
            return scoreNeededToWin;
        }

        public String toString()
        {
            return "\tNumber of players per team:\t" + numPlayersPerTeam + "\n" +
                    "\tNumber of duplicate cards:\t" + numDuplicateCards + "\n" +
                    "\tScore needed to win:\t\t" + scoreNeededToWin;
        }


    }
}
