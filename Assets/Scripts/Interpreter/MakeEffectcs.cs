using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Skyrim_Interpreter
{
    public class MakeEffectcs
    {
        public static void DoIt(Effect effect ,Context context) 
        {
            UnityEngine.Debug.Log($"Se mando a hacer el effecto {effect.Name}");
            effect.Action.Evaluar(context,effect.Targets);
            List<Card> c = Context.Asignments["board"];
            UnityEngine.Debug.Log($"La cantidad de cartas que hay actualmente en el board son de {c.Count}");
            UnityEngine.Debug.Log("Se ejecuto el efectooooooooooooooooooooooooooo");
            if (effect.Name == "Draw") 
            {
                GameManager game = GameManager.Instancia;
                game.ActualizarVisualizacionCartas(Context.Asignments["hand1"],game.Hand1);
                game.ActualizarVisualizacionCartas(Context.Asignments["hand2"], game.Hand1);
            }
        }

    }
}
