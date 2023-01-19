using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.InputHistory
{
    // Tracks relevant information to resolve TAS replay outputs
    public struct InputStates
    {
        public int Jump;

        public InputStates(int jump)
        {
            Jump = jump;
        }
    }
}
