using System;
using System.Collections.Generic;
using System.Text;

namespace ShootClient
{
    sealed class Trump
    {
        private string name;

        private bool isASuit;

        private Suit suit;

        /// <summary>
        /// contains one of each trump accessible by name
        /// </summary>
        private static Dictionary<string, Trump> masterList = new Dictionary<string, Trump>();

        static Trump()
        {
            foreach (Trump t in allTrumps)
            {
                masterList.Add(t.ToString(), t);
            }
        }

        private Trump(string name, bool isSuit, Suit suit)
        {
            this.name = name;
            this.isASuit = isSuit;
            this.suit = suit;
        }

        public static readonly Trump SPADES = new Trump("Spades", true, Suit.SPADES);

        public static readonly Trump HEARTS = new Trump("Hearts", true, Suit.HEARTS);

        public static readonly Trump DIAMONDS = new Trump("Diamonds", true,
                Suit.DIAMONDS);

        public static readonly Trump CLUBS = new Trump("Clubs", true, Suit.CLUBS);

        public static readonly Trump HIGH = new Trump("High", false, null);

        public static readonly Trump LOW = new Trump("Low", false, null);

        public static readonly Trump[] allTrumps = { SPADES, HEARTS, DIAMONDS, CLUBS, HIGH, LOW };

        /// <summary>
        /// return whether this trump is a suit
        /// </summary>
        /// <returns>true if not high or low</returns>
        public bool isSuit()
        {
            return isASuit;
        }

        /// <summary>
        /// get the name of the trump
        /// </summary>
        /// <returns>trump's name</returns>
        public override string ToString()
        {
            return name;
        }

        /// <summary>
        /// get the suit associated with this trump
        /// </summary>
        /// <returns>a suit or null if trump is high or low</returns>
        public Suit getSuit()
        {
            return suit;
        }

        /// <summary>
        /// check if two trumps are identical
        /// </summary>
        /// <param name="o">trump to compare to</param>
        /// <returns>true if two trumps are identical</returns>
        public override bool Equals(Object o)
        {
            if (this == o)
            {
                return true;
            }
            if (!(o.GetType() == typeof(Trump)))
            {
                return false;
            }
            Trump t = (Trump)o;
            return this.name == t.name;
        }

        /// <summary>
        /// hash code = name's hash code
        /// </summary>
        /// <returns>trump's hash code</returns>
        public override int GetHashCode()
        {
            return this.name.GetHashCode();
        }

        /// <summary>
        /// get a Trump object from its name
        /// </summary>
        /// <param name="t">a string containing the trump's name</param>
        /// <returns>the corresponding Trump object</returns>
        public static Trump FromString(string t)
        {
            if (t == string.Empty)
            {
                return null;
            }
            else
            {
                return masterList[t];
            }
        }
    }
}
