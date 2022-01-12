using System;
using System.Collections.Generic;
using System.Text;

namespace ShootClient
{
    /*
     * This class is to be used in conjuction with
     * CardList.sort( CardComparator c ) in order to sort
     * a CardList into a visually apppealing form
     * (shouldn't be used to evaluate tricks)
     */
    class CardComparator : IComparer<Card>
    {
        private bool contextual;

        private Trump trump;

        /*
         * Default Constructor
         * Will sort cards by physical card (ie. no trump
         * consideration)
         */
        public CardComparator()
        {
            trump = null;
            contextual = false;
        }

        /*
         * Creates a comparator which will group cards by their
         * contextual suit and order them by their contextual rank
         */
        public CardComparator(Trump trump)
        {
            this.trump = trump;
            contextual = true;
        }

        public int Compare(Card card1, Card card2)
        {
            //if we don't have a trump context, simply
            //compare the two physical cards
            if (contextual == false)
            {
                if (card1.Equals(card2))
                {
                    return 0;
                }
                else if (card1.getSuit() == card2.getSuit())
                {
                    return (card1.getRank().getRanking() < card2.getRank().getRanking() ? 1 : -1);
                }
                else
                {
                    return ((card1.getSuit().getId() > card2.getSuit().getId()) ? 1 : -1);
                }
                //if we do have a trump context, compare the
                //contextual suit and rank of the two cards
            }
            else
            {
                if (card1.Equals(card2))
                {
                    return 0;
                }
                else if (card1.getContextualSuit(trump) == card2.getContextualSuit(trump))
                {
                    return (card1.getContextualRank(trump).getRanking() < card2.getContextualRank(trump).getRanking() ? 1 : -1);
                }
                else
                {
                    return ((card1.getContextualSuit(trump).getId() > card2.getContextualSuit(trump).getId()) ? 1 : -1);
                }
            }
        }

    }
}
