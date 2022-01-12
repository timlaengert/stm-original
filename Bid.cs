using System;
using System.Collections.Generic;
using System.Text;

namespace ShootClient
{
    sealed class Bid
    {
        private static readonly int PASS_NUM = 0;
        private static readonly int SHOOT_NUM = 9;

        private int number;
        private Trump trump;

        private int shootNumber;

        public Player bidder = null;

        private Bid(int number, Trump trump, int shootNumber)
        {
            this.number = number;
            this.trump = trump;
            this.shootNumber = shootNumber;
        }
 
        /// <summary>
        /// Makes a bid that is neither a pass nor a shoot (ie. a normal bid)
        /// </summary>
        /// <param name="number">The number of the bid (ie. "6 hearts"-> number = 6)</param>
        /// <param name="trump">The trump suit associated with the bid</param>
        /// <returns>a new normal Bid with given number and trump</returns>
        public static Bid makeNormalBid(int number, Trump trump)
        {
            return new Bid(number, trump, 0);
        }

        /// <summary>
        /// Construct a pass bid
        /// </summary>
        /// <returns>a new pass bid</returns>
        public static Bid makePassBid()
        {
            return new Bid(PASS_NUM, null, 0);
        }

        /// <summary>
        /// Makes a shoot bid
        /// </summary>
        /// <param name="shootNumber">Single shoot = 1, Double shoot = 2, etc...</param>
        /// <param name="trump">trump suit to shoot in</param>
        /// <returns>a new Shoot bid with given shoot number and trump</returns>
        public static Bid makeShootBid(int shootNumber, Trump trump)
        {
            return new Bid(SHOOT_NUM, trump, shootNumber);
        }

        /// <summary>
        /// is this bid a pass?
        /// </summary>
        /// <returns>true if pass, false otherwise</returns>
        public bool isPass()
        {
            return number == PASS_NUM;
        }

        /// <summary>
        /// is this bid a shoot?
        /// </summary>
        /// <returns>true if shoot, false otherwise</returns>
        public bool isShoot()
        {
            return number == SHOOT_NUM;
        }

        /// <summary>
        /// is this a normal bid?
        /// </summary>
        /// <returns>true if this bid is neither a shoot or a pass, false otherwise</returns>
        public bool isNormalBid()
        {
            return number > PASS_NUM && number < SHOOT_NUM;
        }

        /// <summary>
        /// Gets the number associated with a bid.
        /// </summary>
        /// <returns>the number associated with the bid (ie. "I bid 5 hearts"-> number = 5)</returns>
        public int getNumber()
        {
            return number;
        }

        /// <summary>
        /// If the bid was a shoot, returns what kind of shoot it was.
        /// A single shoot will return 1, a double shoot will return 2, etc...
        /// </summary>
        /// <returns>the shoot number</returns>
        public int getShootNumber()
        {
            return shootNumber;
        }

        /// <summary>
        /// Gets the trump associated with this bid.
        /// </summary>
        /// <returns>trump associated with bid</returns>
        public Trump getTrump()
        {
            return trump;
        }

        /// <summary>
        /// compare two bids.
        /// </summary>
        /// <param name="otherBid">the other bid to check against</param>
        /// <returns>whether or not this bid is greater than another bid (ie, would beat another bid for the highest bid)</returns>
        public bool isBetterThan(Bid otherBid)
        {
            if (otherBid == null) throw new Exception("Illegal null argument passed to Bid.isBetterThan().");
            //if other bid is a pass and this isn't, return true
            if (otherBid.isPass() & !this.isPass()) return true;
            //if this a shoot, but the other isn't, return true
            if (!otherBid.isShoot() & this.isShoot()) return true;
            //if both bids are passes, return false
            if (otherBid.isPass() && this.isPass()) return false;
            //if both bids are regular bids, check number
            if (otherBid.isNormalBid() && this.isNormalBid())
            {
                return this.getNumber() > otherBid.getNumber();
            }
            //if both bids are shoots, check shoot number
            if (otherBid.isShoot() && this.isShoot())
            {
                return this.getShootNumber() > otherBid.getShootNumber();
            }
            //all other cases, other bid is better, so return false
            return false;
        }

        /// <summary>
        /// take a bid in string form and construct a Bid object.
        /// </summary>
        /// <param name="bid">a bid in string form. Ex: "0" (pass), "3Clubs", "9High" (shoot)</param>
        /// <returns>corresponding bid object</returns>
        public static Bid FromString(string bid)
        {
            int tricks = int.Parse(bid.Substring(0, 1));
            int shootNum = int.Parse(bid.Substring(1, 1));
            string t = bid.Substring(2);
            Trump trump;

            if (t == string.Empty || tricks == PASS_NUM)
            {
                return Bid.makePassBid();
            }

            trump = Trump.FromString(t);

            if (tricks == SHOOT_NUM)
            {
                return Bid.makeShootBid(shootNum, trump);
            }
            else
            {
                return Bid.makeNormalBid(tricks, trump);
            }
        }

        /// <summary>
        /// convert a bid to a string
        /// </summary>
        /// <returns>bid in string form</returns>
        public override string ToString()
        {
            if (isPass())
            {
                return PASS_NUM.ToString() + shootNumber.ToString();
            }
            else
            {
                return number.ToString() + shootNumber.ToString() + trump.ToString();
            }
        }

        /// <summary>
        /// Get bid in a human-readable format.
        /// </summary>
        /// <returns></returns>
        public string ToDisplayString()
        {
            string tricks = isPass() ? string.Empty : number.ToString();
            string trump = isPass() ? string.Empty : this.trump.ToString();

            if (isPass()) return "Pass";
            else if (isShoot()) return "Shoot in " + trump;
            else if (number == 1 && trump.Contains("s")) return tricks + " " + trump.Substring(0, trump.Length - 1);
            else return tricks + " " + trump;
        }
    }
}
