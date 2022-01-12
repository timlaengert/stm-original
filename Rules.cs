using System;
using System.Collections.Generic;
using System.Text;

namespace ShootClient
{
    sealed class Rules
    {

        /// <summary>
        /// Find out whether a card is legal.
        /// </summary>
        /// <param name="hand">player's hand</param>
        /// <param name="card">card to test</param>
        /// <param name="leadCard">first card of trick</param>
        /// <param name="trump">current trump</param>
        /// <returns>true if card is allowed</returns>
        public static bool isCardLegal(List<Card> hand, Card card,
                Card leadCard, Trump trump, out string reason)
        {
            Suit leadSuit = null;
            reason = string.Empty;

            if (leadCard != null)
            {
                leadSuit = leadCard.getContextualSuit(trump);
            }
            // cardPlayed is illegal if it is not contained in hand
            if (!hand.Contains(card))
            {
                reason = "Card not in hand.";
                return false;
            }
            // if no card has been lead yet, then any card (as long as it
            // is in the hand) is legal
            if (leadSuit == null)
                return true;
            // if the suit lead is the same as the suit played, the card is always
            // legal
            if (leadSuit == card
                    .getContextualSuit(trump))
            {
                return true;
                // else if the CardList contains no cards of the suit lead, any card
                // is legal
            }
            else if (Card.getCardsOfContextualSuit(hand,
                    leadSuit, trump).Count == 0)
            {
                return true;
                // otherwise the card is illegal
            }
            else
            {
                reason = "Card is the wrong suit.";
                return false;
            }
        }

        /*
         * Returns a new List<Card> containing all legal cards in a hand for a
         * given lead Card and trump
         */
        public static List<Card> getLegalCards(List<Card> hand, Card leadCard,
                Trump trump)
        {
            string reason = string.Empty;

            List<Card> legalCards = new List<Card>();

            foreach (Card c in hand)
            {
                if (Rules.isCardLegal(hand, c, leadCard, trump, out reason))
                {
                    legalCards.Add(c);
                }
            }

            return legalCards;
        }

        /// <summary>
        /// Determines whether or not one card would beat another in a trick
        /// </summary>
        /// <param name="c1">Card to compare to</param>
        /// <param name="c2">Card being played</param>
        /// <param name="trump">Current Trump</param>
        /// <param name="suitLead">Suit that was lead</param>
        /// <returns>true if c2 is strictly better than c1</returns>
        public static bool isCardBetter(Card c1, Card c2, Trump trump,
                Suit suitLead)
        {
            // Case 1: The two cards are the same suit
            // if the two cards are the same suit, simply compare their contextual
            // ranks (with a reverse decision if the
            // trump is LOW
            if (c1.getContextualSuit(trump) == c2.getContextualSuit(trump))
            {
                if (trump == Trump.LOW)
                {
                    return (c2.getContextualRank(trump).getRanking() < c1
                            .getContextualRank(trump).getRanking());
                }
                else
                {
                    return (c2.getContextualRank(trump).getRanking() > c1
                            .getContextualRank(trump).getRanking());
                }
            }
            // Case 2: Trump is a suit and the two cards played are different suits
            if (trump.isSuit())
            {
                // if c1 is a trump, c1 must win (since they do not have the same
                // suit)
                if (c1.getContextualSuit(trump) == trump.getSuit())
                {
                    return false;
                    // if c2 is a trump, c2 must win (since they do not have the
                    // same
                    // suit)
                }
                else if (c2.getContextualSuit(trump) == trump.getSuit())
                {
                    return true;
                    // at this point, we know that the trump is a suit, both cards
                    // are
                    // different suits, and neither is trump
                    // so if c1 is the suit that was lead, it must beat c2
                }
                else if (c1.getContextualSuit(trump) == suitLead)
                {
                    return false;
                    // if c2 is of the suit lead, then it must beat c1
                }
                else if (c2.getContextualSuit(trump) == suitLead)
                {
                    return true;
                    // otherwise, neither cards follow suit, say c1 wins
                }
                else
                {
                    return false;
                }
                // Case 3: Trump is not a suit and the two cards played are
                // different suits
            }
            else
            {
                // whichever card followed suit must win (since they are different
                // suits, and trump is not a suit)
                // so if c1 is the suit that was lead, it must beat c2
                if (c1.getContextualSuit(trump) == suitLead)
                {
                    return false;
                    // if c2 is of the suit lead, then it must beat c1
                }
                else if (c2.getContextualSuit(trump) == suitLead)
                {
                    return true;
                    // otherwise, neither cards follow suit, say c1 wins
                }
                else
                {
                    return false;
                }
            }
        }

        /**
         * Determine the Leech limit for a particular GameSettings. The leech limit
         * is defined to be the point at which if the other team only took half of
         * the tricks to make their contract, the team that didn't make the contract
         * would still win. In order to avoid this, a team cannot get any points
         * once their score is equal to or exceeds the leech limit for the game.
         * 
         * @param settings
         *            the settings of the game
         * @return the score at which a team can only receive points if they make
         *         the contract
         */
        public static int getLeechLimit(GameSettings settings)
        {
            //int deckSize = settings.getDeckSize();
            //int numPlayers = settings.getNumPlayersPerTeam() * 2;

            //return deckSize - (deckSize / numPlayers) / 2;

            return 47;
        }

        public static bool isBidLegal(GameSettings settings, Bid newBid, Bid lastHighestBid)
        {
            if (lastHighestBid == null)
            {
                if (newBid.isShoot() && newBid.getShootNumber() != 1)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            // first check newBid is a legitimate bid on its own
            if (newBid.isNormalBid())
            {
                // return false if the bid is greater than the number of cards in
                // someone's hand
                if (newBid.getNumber() > getHandSize(settings))
                    return false;
            }

            // if the bid is a pass, it's always legal
            if (newBid.isPass())
                return true;
            // if the new bid is not better than the old bid, it isn't legal (since
            // we already took care of passes)
            if (!newBid.isBetterThan(lastHighestBid))
                return false;
            // make sure shoot numbers occur in right order (ie someone didn't go
            // straight to double shooting or someone didn't go from single shooting
            // to triple shooting)
            if (newBid.isShoot() && !lastHighestBid.isShoot())
            {
                if (newBid.getShootNumber() == 1) return true;
                else return false;
            }
            else if (newBid.isShoot() && lastHighestBid.isShoot())
            {
                return (newBid.getShootNumber() == lastHighestBid.getShootNumber() + 1);
            }
            //all other cases the new bid is legal
            return true;

        }

        public static int getHandSize(GameSettings settings)
        {
            return settings.getDeckSize()
                    / (settings.getNumPlayersPerTeam() * 2);
        }
    }
}
