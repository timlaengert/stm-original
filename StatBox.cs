using System;
using System.Collections.Generic;
using System.Text;

namespace ShootServer
{
    class StatBox
    {
        private int handsPlayed = 0;
        public int HandsPlayed
        {
            get { return handsPlayed; }
            set { handsPlayed = value; }
        }

        private int pointsFor = 0;
        public int PointsFor
        {
            get { return pointsFor; }
            set { pointsFor = value; }
        }

        private int pointsAgainst = 0;
        public int PointsAgainst
        {
            get { return pointsAgainst; }
            set { pointsAgainst = value; }
        }

        private int callsFor = 0;
        public int CallsFor
        {
            get { return callsFor; }
            set { callsFor = value; }
        }

        private int callsAgainst = 0;
        public int CallsAgainst
        {
            get { return callsAgainst; }
            set { callsAgainst = value; }
        }

        private int euchresFor = 0;
        public int EuchresFor
        {
            get { return euchresFor; }
            set { euchresFor = value; }
        }

        private int euchresAgainst = 0;
        public int EuchresAgainst
        {
            get { return euchresAgainst; }
            set { euchresAgainst = value; }
        }

        private int trickDifferential = 0;
        public int TrickDifferential
        {
            get { return trickDifferential; }
            set { trickDifferential = value; }
        }
    }
}
