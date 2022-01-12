using System;
using System.Collections.Generic;
using System.Text;

namespace ShootClient
{
    public sealed class Rank
    {
        private int ranking;

        private string shortName;

        private string fullName;

        private ContextualRank cRank;

        private Rank(int ranking, string shortName, string fullName, ContextualRank cRank)
        {
            this.ranking = ranking;
            this.shortName = shortName;
            this.fullName = fullName;
            this.cRank = cRank;
        }

        public static readonly Rank NINE = new Rank(9, "9", "Nine", ContextualRank.NINE);

        public static readonly Rank TEN = new Rank(10, "0", "Ten", ContextualRank.TEN);

        public static readonly Rank JACK = new Rank(11, "J", "Jack", ContextualRank.JACK);

        public static readonly Rank QUEEN = new Rank(12, "Q", "Queen", ContextualRank.QUEEN);

        public static readonly Rank KING = new Rank(13, "K", "King", ContextualRank.KING);

        public static readonly Rank ACE = new Rank(14, "A", "Ace", ContextualRank.ACE);

        /// <summary>
        /// an array containing all 6 ranks
        /// </summary>
        public static readonly Rank[] allRanks = { NINE, TEN, JACK, QUEEN, KING, ACE };

        /// <summary>
        /// Get the rank's full name
        /// </summary>
        /// <returns>rank's name</returns>
        public string getFullName()
        {
            return fullName;
        }

        /// <summary>
        /// Get ranking as an integer (range 9 - 14)
        /// </summary>
        /// <returns>integer rank</returns>
        public int getRanking()
        {
            return ranking;
        }

        /// <summary>
        /// the short-form name for the rank ie. J, Q, K, etc.
        /// </summary>
        /// <returns>rank's short name</returns>
        public string getShortName()
        {
            return shortName;
        }

        /// <summary>
        /// Get the corresponding ContextualRank for this rank
        /// </summary>
        /// <returns>contextual rank</returns>
        public ContextualRank getContextualRank()
        {
            return cRank;
        }

        /// <summary>
        /// Check if two ranks are equal
        /// </summary>
        /// <param name="o">rank to compare to</param>
        /// <returns>true if two ranks are equal</returns>
        public override bool Equals(Object o)
        {
            if (this == o)
            {
                return true;
            }
            if (!(o.GetType() == typeof(Rank)))
            {
                return false;
            }
            Rank r = (Rank)o;
            return this.ranking == r.ranking;
        }

        /// <summary>
        /// Hash code = ranking
        /// </summary>
        /// <returns>hash code as integer</returns>
        public override int GetHashCode()
        {
            return this.ranking;
        }
    }
}
