using System;
using System.Collections.Generic;
using System.Text;

namespace ShootServer
{
    class CardCounter
    {
        private List<Card> cardsLeft = new List<Card>();
        private List<Card> cardsPlayed = new List<Card>();
        private Dictionary<Suit, Card> bestCardsLeft = new Dictionary<Suit, Card>();
        private Dictionary<Suit, int> numBestCardsLeft = new Dictionary<Suit, int>();
        private Dictionary<Suit, int> numTotalCardsLeft = new Dictionary<Suit, int>();
        private Trump trump;
        private Suit suitLeadInCurrentTrick;
        private int leadPositionInCurrentTrick;
        public Card bestInCurrentTrick;
        public Client winnerOfCurrentTrick;

        private const bool DEBUG_MODE = false;

        public void initializeRound()
        {
            cardsLeft.Clear();
            cardsPlayed.Clear();
            bestCardsLeft.Clear();
            numBestCardsLeft.Clear();
            numTotalCardsLeft.Clear();
            trump = null;

            foreach (Suit suit in Suit.allSuits)
            {
                foreach (Rank rank in Rank.allRanks)
                {
                    cardsLeft.Add(new Card(rank, suit));
                    cardsLeft.Add(new Card(rank, suit));
                }
            }
        }

        public void initializeTrick()
        {
            suitLeadInCurrentTrick = null;
            leadPositionInCurrentTrick = -1;
            bestInCurrentTrick = null;
            winnerOfCurrentTrick = null;
        }

        public bool playsLast(Client player)
        {
            return (player.position + 1) % (player.game.settings.getNumPlayersPerTeam() * 2) == leadPositionInCurrentTrick;
        }

        public void playCard(Card card)
        {
            Suit cSuit = card.getContextualSuit(trump);
            ContextualRank cRank = card.getContextualRank(trump);
            Card newCard = null;
            Rank newRank = null;
            Suit newSuit = null;
            bool cardOK = false;
            int numCardsLeft;
            Client player = card.player;

            if (leadPositionInCurrentTrick == -1)
            {
                leadPositionInCurrentTrick = player.position;
                suitLeadInCurrentTrick = card.getContextualSuit(trump);
            }

            if (bestInCurrentTrick == null || Rules.isCardBetter(bestInCurrentTrick, card, trump, bestInCurrentTrick.getContextualSuit(trump)))
            {
                bestInCurrentTrick = card;
                winnerOfCurrentTrick = player;
            }

            cardsLeft.Remove(card);
            if (DEBUG_MODE)
            {
                System.Console.WriteLine(card.getRank().getShortName() + " of " + card.getSuit().ToString() + " removed from Cards Left");
            }
            cardsPlayed.Add(card);
            if (DEBUG_MODE)
            {
                System.Console.WriteLine(card.getRank().getShortName() + " of " + card.getSuit().ToString() + " added to Cards Played");
            }
            numCardsLeft = numTotalCardsLeft[cSuit];
            numTotalCardsLeft.Remove(cSuit);
            numTotalCardsLeft.Add(cSuit, numCardsLeft - 1);
            if (numTotalCardsLeft[cSuit] == 0)
            {
                bestCardsLeft.Remove(cSuit);
                numBestCardsLeft.Remove(cSuit);
                numBestCardsLeft.Add(cSuit, 0);
                return;
            }

            if (bestCardsLeft[cSuit].Equals(card))
            {
                numCardsLeft = numBestCardsLeft[cSuit];
                numBestCardsLeft.Remove(cSuit);
                numBestCardsLeft.Add(cSuit, numCardsLeft - 1);
                if (DEBUG_MODE)
                {
                    System.Console.WriteLine(card.getRank().getShortName() + " of " + card.getSuit().ToString() + " removed from Best Cards Left");
                }

                if (numBestCardsLeft[cSuit] == 0)
                {
                    while (!cardOK)
                    {
                        if (trump.Equals(Trump.LOW) && !cRank.Equals(ContextualRank.ACE))
                        { // trump is low
                            newRank = Rank.allRanks[cRank.getRanking() + 1 - 9];
                            newSuit = cSuit;
                        }
                        else if ((trump.Equals(Trump.HIGH) || !cSuit.Equals(trump.getSuit())) && !cRank.Equals(ContextualRank.NINE))
                        { // trump is high or card isn't trump
                            if (trump.isSuit() && trump.getSuit().Equals(cSuit.getSameColourSuit()) && cRank.Equals(ContextualRank.QUEEN))
                            {
                                newRank = Rank.TEN;
                            }
                            else
                            {
                                newRank = Rank.allRanks[cRank.getRanking() - 1 - 9];
                            }
                            newSuit = cSuit;
                        }
                        else if (cSuit.Equals(trump.getSuit()) && !cRank.Equals(ContextualRank.NINE))
                        { // card played is trump
                            if (cRank.Equals(ContextualRank.RIGHT))
                            {
                                newRank = Rank.JACK;
                                newSuit = cSuit.getSameColourSuit();
                            }
                            else if (cRank.Equals(ContextualRank.LEFT))
                            {
                                newRank = Rank.ACE;
                                newSuit = cSuit;
                            }
                            else if (cRank.Equals(ContextualRank.ACE))
                            {
                                newRank = Rank.KING;
                                newSuit = cSuit;
                            }
                            else if (cRank.Equals(ContextualRank.KING))
                            {
                                newRank = Rank.QUEEN;
                                newSuit = cSuit;
                            }
                            else if (cRank.Equals(ContextualRank.QUEEN))
                            {
                                newRank = Rank.TEN;
                                newSuit = cSuit;
                            }
                            else if (cRank.Equals(ContextualRank.TEN))
                            {
                                newRank = Rank.NINE;
                                newSuit = cSuit;
                            }
                            else
                            {
                                return;
                            }
                        }
                        else
                        {
                            return;
                        }
                        newCard = new Card(newRank, newSuit);
                        if (!cardsLeft.Contains(newCard))
                        {
                            cRank = newCard.getContextualRank(trump);
                        }
                        else
                        {
                            cardOK = true;
                        }
                    }
                    bestCardsLeft.Remove(cSuit);
                    bestCardsLeft.Add(cSuit, newCard);
                    numBestCardsLeft.Remove(cSuit);
                    numBestCardsLeft.Add(cSuit, 2);
                    if (DEBUG_MODE)
                    {
                        System.Console.WriteLine(newCard.getRank().getShortName() + " of " + newCard.getSuit().ToString() + " added to Best Cards Left (x2)");
                    }
                }
            }
        }

        public void adjustForTrump(Trump newTrump)
        {
            Rank rank;
            trump = newTrump;

            foreach (Suit suit in Suit.allSuits)
            {
                if (trump.isSuit() && trump.getSuit().Equals(suit))
                {
                    rank = Rank.JACK;
                    numTotalCardsLeft.Add(suit, 14);
                }
                else if (trump.isSuit() && trump.getSuit().Equals(suit.getSameColourSuit()))
                {
                    rank = Rank.ACE;
                    numTotalCardsLeft.Add(suit, 10);
                }
                else if (trump.Equals(Trump.LOW))
                {
                    rank = Rank.NINE;
                    numTotalCardsLeft.Add(suit, 12);
                }
                else
                {
                    rank = Rank.ACE;
                    numTotalCardsLeft.Add(suit, 12);
                }

                bestCardsLeft.Add(suit, new Card(rank, suit));
                numBestCardsLeft.Add(suit, 2);
                if (DEBUG_MODE)
                {
                    System.Console.WriteLine(rank.getShortName() + " of " + suit.ToString() + " added to Best Cards Left (x2)");
                }
            }
        }

        public bool isHighest(Card card)
        {
            if (bestCardsLeft[card.getContextualSuit(trump)].Equals(card))
            {
                return true;
            }
            return false;
        }

        public bool isHighestExcluding(Card card, Dictionary<Suit, List<Card>> excluded)
        { // sloppy
            Suit suit = card.getContextualSuit(trump);
            List<Card> adjustedCardsLeft = new List<Card>(cardsLeft);

            foreach (List<Card> cardsInSuit in excluded.Values)
            {
                foreach (Card toRemove in cardsInSuit)
                {
                    adjustedCardsLeft.Remove(toRemove);
                }
            }
            foreach (Card candidate in adjustedCardsLeft)
            {
                if (candidate.getContextualSuit(trump).Equals(suit) && Rules.isCardBetter(card, candidate, trump, suitLeadInCurrentTrick))
                {
                    return false;
                }
            }
            return true;
        }

        public bool isLone(Card card)
        {
            if (!cardsPlayed.Contains(card))
            {
                return false;
            }
            return true;
        }

        public int cardsRemaining(Suit suit)
        { // could probably be optimised
            int result = 0;

            foreach (Card card in cardsLeft)
            {
                if (card.getSuit().Equals(suit))
                {
                    result++;
                }
            }

            return result;
        }
    }
}
