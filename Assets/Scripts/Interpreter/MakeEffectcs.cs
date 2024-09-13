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
            UnityEngine.Debug.Log($"Se mando a hacer el effecto {effect.Name}");
            effect.Action.Evaluar(context,effect.Targets);
            UnityEngine.Debug.Log("Se ejecuto el efectooooooooooooooooooooooooooo");
        }

    }
}
