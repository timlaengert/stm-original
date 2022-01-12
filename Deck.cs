using System;
using System.Collections.Generic;
using System.Text;

namespace ShootServer
{
    class Deck : List<Card>
    {
        /// <summary>
        /// number of cards in a full deck.
        /// </summary>
        private int size;

        /// <summary>
        /// Create a new deck and shuffle it.
        /// </summary>
        /// <param name="numDuplicateCards">The number of copies of each card to put in the deck.</param>
        public Deck(int numDuplicateCards)
        {
            // put all neccessary cards in the deck
            foreach (Suit suit in Suit.allSuits)
            {
                foreach (Rank rank in Rank.allRanks)
                {
                    for (int i = 0; i < numDuplicateCards; i++)
                    {
                        this.Add(new Card(rank, suit));
                    }
                }
            }
            size = Count;

            shuffle();
        }

        /// <summary>
        /// Shuffle the deck
        /// </summary>
        private void shuffle()
        {
            Random random = new Random();
            for (int i = 0; i < Count; i++)
            {
                int j = random.Next(Count - i) + i;
                Card tmp = this[i];
                this[i] = this[j];
                this[j] = tmp;
            }
        }

        /// <summary>
        /// get the total number of cards in the deck
        /// </summary>
        /// <returns>number of cards</returns>
        public int cardsInDeck()
        {
            return this.size;
        }

        /// <summary>
        /// take the top card off the deck and return it.
        /// </summary>
        /// <returns>a single random card.</returns>
        public Card getTopCard()
        {
            Card topCard = this[0];
            this.RemoveAt(0);
            return topCard;
        }
    }
}
