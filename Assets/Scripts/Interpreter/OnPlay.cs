﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skyrim_Interpreter
{
    public class OnPlay
    {
        public static void Play(Card card,Context context) 
        {
            // recorremos el OnActivation de card
            foreach (var item in card.OnActivation)
            {
                Effect myeffect = new Effect();
                //comprobamos que los elementos de este sean de tipo effectcardnode y si no los on entonces lanzamos exception 
                if (item is EffectCardNode ecn)
                {
                    //actualizar los parametros
                    foreach (var things in ecn.Parameters)
                    {
                        if (things is ColonASTNode colon) { GameContext.InputKeyParameter(colon.left,colon.right); }
                        else { throw new InvalidOperationException("Estan mal asignados los parametros ya que estos se asignan mediante :"); }
                    }
                    //obtenemos el efecto que vamos a realizar
                    myeffect = GameContext.EffectAssignmet[ecn.Name];
                    Targets objective = null;
                    UnityEngine.Debug.Log($"Encontro el effecto y es {myeffect.Name}");
                    if (ecn.Selector != null)
                    {
                        SelectorCardNode myselcetor = ecn.Selector;
                        UnityEngine.Debug.Log($"El source que vamos a buscar es {myselcetor.Source}");
                        if (myselcetor.Source == null) throw new InvalidOperationException("Necesitamos que en el selector ");
                        UnityEngine.Debug.Log(myselcetor.Source);
                        if (myselcetor.Predicate is not LambdaASTNode lambda) throw new InvalidOperationException("Estamos presentando problemas con el predicate del selector");
                        List<string> args = new List<string>() { "hand1", "hand2", "deck1", "deck2", "field1", "field2", "graveyard1", "graveyard2", "board" };
                        if (!args.Contains(myselcetor.Source)) throw new Exception("El source no existe en nuestro contexto");
                        List<Card> source = Context.Asignments[myselcetor.Source];
                        if (source == null) throw new Exception("El source no existe en nuestro contexto");
                        UnityEngine.Debug.Log("Vamos a buscar el source");
                        objective = DrawCards(source, lambda, context);
                        //ya tenemos el objetivo del effecto
                    }
                    if (myeffect.Name == "ReturnToDeck") 
                    { 
                        List<Card> list = Context.Asignments["board"];
                        List<Card> newlist = new List<Card>();
                        for (int i = 0; i < list.Count; i++)
                        {
                            newlist.Add(list[i]);
                        }
                        Targets t = new Targets();
                        t .targets = newlist;
                        objective = t;
                    }
                    myeffect.Targets = objective;
                    if(objective != null) UnityEngine.Debug.Log($"el count del source es de {myeffect.Targets.targets.Count}");
                    //llamamos al effecto
                    MakeEffectcs.DoIt(myeffect,context);
                }
                else 
                {
                    throw new InvalidOperationException("En el OnActivation solo puede haber effectos");
                }
            }
        }
        private static Targets DrawCards(List<Card> source ,LambdaASTNode lambda,Context context) 
        {
            UnityEngine.Debug.Log("Comenzamos");
            UnityEngine.Debug.Log($"El count del source es de {source.Count}");
            Targets output= new Targets();
            object value = null;
            AccessASTNode a = null;
            ASTnode r = null;
           
            Token_Type t = Token_Type.EOF; ;
            if(!CheckLeftChild(lambda.Left)) throw new InvalidOperationException("El hijo izquierdo de lambda debe ser un identificador dentro de parentisis");
            if (!IsBooleanOperation(lambda.Right)) throw new InvalidOperationException("El hijo derecho del lambda debe ser un operador logico");
            ASTnode rightchild = lambda.Right;
            if (rightchild is ComparationASTNode comparation)
            {
                if (!(comparation.left is AccessASTNode acces)) { throw new InvalidOperationException("El hijo izquierdo del operador logico es debe ser del tipo acces"); }
                a = acces;
                t = comparation.type;
                r = comparation.right;
                UnityEngine.Debug.Log("Encontro en el hijo derecho del lambda una comparacion");
            }
            else if (rightchild is EqualASTNode equal)
            {
                if (!(equal.Left is AccessASTNode acces)) { throw new InvalidOperationException("El hijo izquierdo del operador logico es debe ser del tipo acces"); }
                a = acces;
                t = equal.type;
                r = equal.Right;
                UnityEngine.Debug.Log("Encontro en el hijo derecho del lambda un ==");
            }
            else if (rightchild is NotEqualASTNode notequal)
            {
                if (!(notequal.Left is AccessASTNode acces)) { throw new InvalidOperationException("El hijo izquierdo del operador logico es debe ser del tipo acces"); }
                a = acces;
                t = notequal.type;
                r = notequal.Right;
                UnityEngine.Debug.Log("Encontro en el hijo derecho del lambda un !=");
            }
            else 
            {
                throw new InvalidOperationException("El hijo derecho del operador lambda esta mal");
            }
            foreach (Card item in source) 
            {
                UnityEngine.Debug.Log($"vamos a comprobar si la carta{ item} cumple con los requisitos");    
                value = IsAccess(a, item);
                UnityEngine.Debug.Log($"El valor buscado de la carta es {value}");
                object v = r.Evaluar();
                UnityEngine.Debug.Log($"El valor de r es de {v}");
                if (Ayudante.EvaluateBinary(value, t, r.Evaluar()) is true) 
                {
                    UnityEngine.Debug.Log("Encontramos un valor");
                    output.Add(item);
                }
            }
            return output;
        }
        private static bool CheckLeftChild(ASTnode node) 
        {
            if (node is GroupingASTNode group) 
            {
                if (group.groupnode is IdentifierASTNode) 
                {
                    return true;
                }
            }
             return false;
        }
        private static bool IsBooleanOperation(ASTnode node) => node is EqualASTNode || node is NotEqualASTNode || node is ComparationASTNode;
        private static object IsAccess(AccessASTNode node, Card card)
        {
            UnityEngine.Debug.Log("Entramos a IsAccess");
            if ( node.left is IdentifierASTNode && node.right is IdentifierASTNode i)
            {
                UnityEngine.Debug.Log($"Quieres acceder al atributo de {i.value}");

                return i.value switch
                {
                    "name" => card.name,
                    "faction" => card.faction,
                    "Range" => card.Range,
                    "type" => card.type,
                    "power" => card.power,
                    _ => throw new InvalidOperationException("No estás accediendo a ningún atributo de la carta")
                };
            }
            throw new InvalidOperationException("El hijo izquierdo del operador lógico debe ser del tipo accesos");
        }
    }
}
