using System;
using System.Collections.Generic;
using System.Text;

namespace ShootServer
{
    class ConservativeAIProfile : EvisceratorProfile
    {
        public ConservativeAIProfile()
        {
            AGGRESSION_FACTOR = 1.1;
        }
    }
}
