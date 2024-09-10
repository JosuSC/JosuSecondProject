using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skyrim_Interpreter
{
    public class MakeEffectcs
    {
        public static void DoIt(Effect effect ,Context context) 
        {
            effect.Action.Evaluar(context,effect.Targets);
        }

    }
}
