using System;
using System.Collections.Generic;
using System.Text;

namespace ShootServer
{
    class AggressiveAIProfile : EvisceratorProfile
    {
        public AggressiveAIProfile()
        {
            AGGRESSION_FACTOR = 1.2;
        }
    }
}
