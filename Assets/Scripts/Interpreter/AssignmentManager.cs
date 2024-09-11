using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Skyrim_Interpreter
{
    public  class GameContext
    {
        public static Dictionary<string, object> Assignment { get; set; } = new Dictionary<string, object>();
        public static Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
        public static Dictionary<string, Effect> EffectAssignmet { get; set; } = new Dictionary<string, Effect>();
        public static Dictionary<string, Card> Cards { get; set; } = new Dictionary<string, Card>();
        public static bool Search(Dictionary<string,object> dictionary,string key) 
        {
         if(dictionary.ContainsKey(key))return true;
            return false;   
        }

        public static void ActualizarValor(string n,object o) 
        {
            if (Assignment.ContainsKey(n)) 
            {
                Assignment[n] = o;
                return;
            }
            Assignment.Add(n, o);   
        }

        public static void InputKeyParameter(ASTnode left ,ASTnode right) 
        {
            //si el left es un idebtificador
            if (left is IdentifierASTNode ident)
            {
              //si no se encuentra ese identificador en el dictionary
                if (!Search(Parameters, ident.value))
                {
                    //si el derecho es un identifier 
                    if (right is IdentifierASTNode ide)
                    {
                        if (ide.value == "Number") Parameters.Add(ident.value , 0);
                        else if (ide.value == "String") Parameters.Add(ident.value,"");
                        else if (ide.value == "Boolean") Parameters.Add(ident.value, false);
                    }
                    else
                    {
                        object value = right.Evaluar();
                        if (value is not int && value is not bool && value is not string) throw new Exception("Parametro no valido");
                        Parameters.Add(ident.value, value);
                    }
                }
                else 
                {
                    //commprobamos si el actual y el que ya estaba son del mismo tipo
                    object v1 = Parameters[ident.value];
                    object v2 = right.Evaluar();
                    if (v1.GetType() == v2.GetType()) { Parameters[ident.value] = v2;}
                    else { throw new InvalidOperationException("Deben ser los mismo tipos"); }
                }
            }
            else 
            {
                throw new InvalidOperationException("No se puede asignar un valor a algo que no sea una variable");
            }
        }

        public static void MiContext(string cont,Context conteext) 
        {
            if (Assignment.ContainsKey(cont)) 
            {
                Assignment[cont] = conteext;
                return;
            }
            Assignment.Add(cont, conteext); 
        }
        public static void InputKeyAssign(IdentifierASTNode left,ASTnode right,Context cont,Targets target) 
        {
                var value = right.Evaluar(cont,target);
                Type obj = value.GetType();
                Console.WriteLine(obj);
                if (!Search(Assignment, left.value))
                {
                    Assignment.Add(left.value,value);
                }
                else
                {
                    Assignment[left.value] = value;   
                } 
        }
        public static void InputAssignmentwithValue(ASTnode left,ASTnode right,string oparator, Context cont, Targets target) 
        {
            if (left is IdentifierASTNode ident)
            {
                object v1 = right.Evaluar(cont,target);

                if (!Search(Assignment, ident.value))
                {
                    throw new InvalidOperationException("no podemos realizar una oparacion de asignacion con operacion aritmetica si no tenemos la variable");
                }
                else 
                {
                    object v2 = Assignment[ident.value];
                    if (v1.GetType() == v2.GetType())
                    {
                        if(oparator == "+=") { Assignment[ident.value] = (int)v2 + (int)v1; }
                        if (oparator == "-=") { Assignment[ident.value] = (int)v2 - (int)v1; }
                    }
                    else 
                    {
                        throw new InvalidOperationException("No son del mismo tipo los value");
                    }
                }
            }
            else 
            {
                throw new InvalidOperationException("Solo se pude asignar a una variable");  
            }
        }
        public static void InputEffcet(string name,Effect effect) 
        {
            //comprobamos si existe el efecto
            if (EffectAssignmet.ContainsKey(name))
            {
               throw new InvalidOperationException("Ese efecto ya existe");
            }
            else 
            {
                EffectAssignmet.Add(name, effect);  
            }
        }
        public static bool IsContainsEffcet(string name) 
        {
            if (EffectAssignmet.ContainsKey(name)) return true;
            return false;   
        }
        public static bool IsContainsAssignment(string name) 
        {
            if (Assignment.ContainsKey(name)) return true;
            return false;
        }
    }
}
