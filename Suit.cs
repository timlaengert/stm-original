using System;
using System.Collections.Generic;
using System.Text;

namespace ShootClient
{
    sealed class Suit
    {
        private string name;

        private int id;

        private char character;

        private Suit(string name, int id, char character)
        {
            this.name = name;
            this.id = id;
            this.character = character;

        }

        /*
         * The following static Suits are the only 4 values a Suit variable can be
         * initialized to since the Suit constructor is declared private and the
         * class cannot be extended
         */
        public static readonly Suit SPADES = new Suit("Spades", 1, 'S');

        public static readonly Suit HEARTS = new Suit("Hearts", 2, 'H');

        public static readonly Suit DIAMONDS = new Suit("Diamonds", 3, 'D');

        public static readonly Suit CLUBS = new Suit("Clubs", 4, 'C');

        /// <summary>
        /// an array containing all four suits
        /// </summary>
        public static readonly Suit[] allSuits = { SPADES, HEARTS, CLUBS, DIAMONDS };

        /// <summary>
        /// Returns the unicode character that commonly represents this suit
        /// </summary>
        /// <returns>a character representing this suit</returns>
        public char getChar()
        {
            return character;
        }

        /// <summary>
        /// Get the same coloured suit ie. SPADES.getSameColourSuit() = CLUBS
        /// </summary>
        /// <returns>the other suit of the same colour</returns>
        public Suit getSameColourSuit()
        {
            Suit opposite = null;

            if (this.Equals(SPADES))
            {
                opposite = CLUBS;
            }
            else if (this.Equals(CLUBS))
            {
                opposite = SPADES;
            }
            else if (this.Equals(HEARTS))
            {
                opposite = DIAMONDS;
            }
            else if (this.Equals(DIAMONDS))
            {
                opposite = HEARTS;
            }

            return opposite;
        }

        public bool isBlack()
        {
            return (this.Equals(Suit.CLUBS) || this.Equals(Suit.SPADES));
        }

        public bool isRed()
        {
            return !this.isBlack();
        }

        /// <summary>
        /// Convert suit to a string
        /// </summary>
        /// <returns>the name of the suit</returns>
        public override string ToString()
        {
            return name;
        }

        /// <summary>
        /// Get a suit's id (range 1 - 4)
        /// </summary>
        /// <returns>suit's id</returns>
        public int getId()
        {
            return id;
        }

        /// <summary>
        /// Check if two suits are equal
        /// </summary>
        /// <param name="o">suit to compare to</param>
        /// <returns>true if suits are identical</returns>
        public override bool Equals(Object o)
        {
            if (this == o)
            {
                return true;
            }
            if (!(o.GetType() == typeof(Suit)))
            {
                return false;
            }
            Suit s = (Suit)o;
            return this.id == s.id;
        }

        /// <summary>
        /// hash code = suit id
        /// </summary>
        /// <returns>hash code</returns>
        public override int GetHashCode()
        {
            return this.id;
        }

    }
}
