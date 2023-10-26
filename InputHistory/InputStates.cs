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
        public int Dash;
        public int Demo;

        public InputStates(int jump, int dash, int demo)
        {
            Jump = jump;
            Dash = dash;
            Demo = demo;
        }
    }
}
