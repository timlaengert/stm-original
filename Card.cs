using System;
using System.Collections.Generic;
using System.Text;

namespace ShootClient
{
    sealed class Card
    {
        private Rank rank;

        private Suit suit;

        private const float CARD_PIXEL_WIDTH = 77;
        private const float CARD_PIXEL_HEIGHT = 115;
        private const float TEXTURE_WIDTH = 1024;
        private const float TEXTURE_HEIGHT = 1024;
        public const float aspectRatio = CARD_PIXEL_WIDTH / CARD_PIXEL_HEIGHT;
        public float textureX1;
        public float textureX2;
        public float textureY1;
        public float textureY2;

        /// <summary>
        /// Player who played this card
        /// </summary>
        public Player player = null;

        /// <summary>
        /// contains one copy of each card, accessible by name
        /// </summary>
        private static Dictionary<string, Card> masterList = new Dictionary<string, Card>();
        
        static Card()
        {
            Card card;
            masterList = new Dictionary<string, Card>();
            foreach (Suit suit in Suit.allSuits)
            {
                foreach (Rank rank in Rank.allRanks)
                {
                    card = new Card(rank, suit);
                    masterList.Add(card.ToString(), card);
                }
            }
        }

        private Card(Rank rank, Suit suit)
        {
            this.rank = rank;
            this.suit = suit;
            textureX1 = 0.125f * (rank.getRanking() - 9);
            textureX2 = textureX1 + CARD_PIXEL_WIDTH / TEXTURE_WIDTH;
            textureY1 = 0.125f * (suit.getId() - 1);
            textureY2 = textureY1 + CARD_PIXEL_HEIGHT / TEXTURE_HEIGHT;
        }

        public Rank getRank()
        {
            return rank;
        }

        public Suit getSuit()
        {
            return suit;
        }

        /// <summary>
        /// Get a card's rank in the context of a trump
        /// </summary>
        /// <param name="trump">current trump</param>
        /// <returns>rank accounting for trump</returns>
        public ContextualRank getContextualRank(Trump trump)
        {
            // if trump is a suit and the card is a Jack, we need to check if it's a
            // left or a right
            if (rank.Equals(Rank.JACK) && trump.isSuit())
            {
                Suit trumpSuit = trump.getSuit();

                if (suit.Equals(trumpSuit))
                {
                    return ContextualRank.RIGHT;
                }
                else if (suit.Equals(trumpSuit.getSameColourSuit()))
                {
                    return ContextualRank.LEFT;
                }
                else
                {
                    return ContextualRank.JACK;
                }
                // for all other cards we can just return the same rank as the
                // original card
            }
            else
            {
                return rank.getContextualRank();
            }
        }

        /// <summary>
        /// Get a card's suit in the context of a trump
        /// </summary>
        /// <param name="trump">current trump</param>
        /// <returns>suit accounting for trump</returns>
        public Suit getContextualSuit(Trump trump)
        {
            // if trump is a suit and the card is a Jack, we need to check if it's a
            // left or a right
            if (rank.Equals(Rank.JACK) && trump.isSuit())
            {
                // get a Suit enum type from our trump enum
                Suit trumpSuit = trump.getSuit();

                if (suit.Equals(trumpSuit))
                {
                    return trumpSuit;
                }
                else if (suit.Equals(trumpSuit.getSameColourSuit()))
                {
                    return trumpSuit;
                }
                else
                {
                    return suit;
                }
                // for all other cards we can just set the rank and suit the same as
                // the actual card
            }
            else
            {
                return suit;
            }
        }

        /// <summary>
        /// Check if two cards are the same
        /// </summary>
        /// <param name="o">card to compare to</param>
        /// <returns>true if card is identical</returns>
        public override bool Equals(Object o)
        {
            if (this == o)
            {
                return true;
            }
            if (o.GetType() != typeof(Card))
            {
                return false;
            }
            Card c = (Card)o;
            return this.rank.Equals(c.rank) && this.suit.Equals(c.suit);
        }

        /// <summary>
        /// Hash code = rank followed by suit (both integers)
        /// </summary>
        /// <returns>card's hash code</returns>
        public override int GetHashCode()
        {
            return this.rank.GetHashCode() * 10 + this.suit.GetHashCode();
        }

        /// <summary>
        /// Get a card from its name
        /// </summary>
        /// <param name="name">name as a string</param>
        /// <returns>a matching card object</returns>
        public static Card FromString(string name)
        {
            return (Card)masterList[name].MemberwiseClone();
        }

        /// <summary>
        /// convert a card to a string
        /// </summary>
        /// <returns>name of the card</returns>
        public override string ToString()
        {
            return this.rank.getShortName() + this.suit.getChar();
        }

        /// <summary>
        /// Return a new List<Card> containing all of the cards of a contextual suit
        /// given a list of cards and a particular trump context
        /// </summary>
        /// <param name="cardList">set of cards to filter</param>
        /// <param name="suit">suit to filter by</param>
        /// <param name="trump">current trump</param>
        /// <returns>filtered list of cards</returns>
        public static List<Card> getCardsOfContextualSuit(List<Card> cardList, Suit suit, Trump trump)
        {
            List<Card> returnList = new List<Card>();

            foreach (Card c in cardList)
            {
                if (c.getContextualSuit(trump).Equals(suit))
                {
                    returnList.Add(c);
                }
            }

            return returnList;
        }

        /// <summary>
        /// Return a new List<Card> containing all of the cards of a contextual suit
        /// given a list of cards and a particular trump context
        /// </summary>
        /// <param name="cardList">set of cards to filter</param>
        /// <param name="suit">suit to filter by</param>
        /// <param name="trump">current trump</param>
        /// <returns>filtered list of cards</returns>
        public static List<Card> getCardsOfContextualSuit(List<Card> cardList, Suit suit)
        {
            List<Card> returnList = new List<Card>();

            foreach (Card c in cardList)
            {
                if (c.getSuit().Equals(suit))
                {
                    returnList.Add(c);
                }
            }

            return returnList;
        }
    }
}
