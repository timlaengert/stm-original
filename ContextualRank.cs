using System;
using System.Collections.Generic;
using System.Text;

namespace ShootClient
{
    /*
     * Represents the states a Euchre card's contextual rank can be:
     * 9, 10, J, Q, K, A, Left, Right
     * This can & should be treated like an enum:
     * ie. ContextualRank r = ContextualRank.RIGHT
     */
    public sealed class ContextualRank
    {

        private int ranking;

        private String name;

        private ContextualRank(int ranking, String name)
        {
            this.ranking = ranking;
            this.name = name;
        }

        /*
         * The following define the only states a ContextualRank reference can be
         * since the constructor is declared private and the class is declared final
         */
        public static readonly ContextualRank NINE = new ContextualRank(9, "Nine");

        public static readonly ContextualRank TEN = new ContextualRank(10, "Ten");

        public static readonly ContextualRank JACK = new ContextualRank(11, "Jack");

        public static readonly ContextualRank QUEEN = new ContextualRank(12, "Queen");

        public static readonly ContextualRank KING = new ContextualRank(13, "King");

        public static readonly ContextualRank ACE = new ContextualRank(14, "Ace");

        public static readonly ContextualRank LEFT = new ContextualRank(15, "Left");

        public static readonly ContextualRank RIGHT = new ContextualRank(16, "Right");

        /*
         * allContextualRanks[] is an array of all the contextual ranks which can be
         * useful if you need to iterate over every possible rank
         */
        public static readonly ContextualRank[] allContextualRanks = { NINE, TEN, JACK,
			QUEEN, KING, ACE, LEFT, RIGHT };

        /**
         * @return the full name of the rank ie. Queen
         */
        public string getName()
        {
            return name;
        }

        /**
         * @return the integer ranking of the card ie. JACK.getRanking() = 11
         */
        public int getRanking()
        {
            return ranking;
        }

        public string toString()
        {
            return name;
        }

        /*
         * Equality test
         */
        public override bool Equals(Object o)
        {
            if (this == o)
            {
                return true;
            }
            if (!(o.GetType() == typeof(ContextualRank)))
            {
                return false;
            }
            ContextualRank r = (ContextualRank)o;
            return this.ranking == r.ranking;
        }

        public override int GetHashCode()
        {
            return this.ranking;
        }
    }
}
