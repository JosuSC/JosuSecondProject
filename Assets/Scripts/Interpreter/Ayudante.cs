using System.Runtime.InteropServices.ComTypes;
using System.IO.Enumeration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.AccessControl;
using System.Xml.Linq;
using System.Reflection;

namespace Skyrim_Interpreter
{
    public static class Ayudante
    {
      
        public static bool IsNumber(params object[] myobjects)
        {
            foreach (var obejects in myobjects)
            {
                if (obejects is not double) return false;
            }
            return true;
        }

        public static bool IsBoolean(params object[] myobjects)
        {
            foreach (var objects in myobjects)
            {
                if (objects is not Boolean)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsString(params object[] myobejects)
        {
            foreach (var objects in myobejects)
            {
                if (objects is not string) return false;
            }
            return true;
        }

        public static bool IsEqual(object a, object b)
        {
            if (a == null && b == null) return true;

            return a == null ? false : a.Equals(b);
        }
        public static object EvaluateBinary(object left, Token_Type type, object right)
        {
            switch (type)
            {
                case Token_Type.PLUS:
                    if (IsNumber(left, right)) return (double)left + (double)right;
                    throw new InvalidOperationException("Invalid types for plus evaluation");
                case Token_Type.MINUS:
                    if (IsNumber(left, right)) return (double)left - (double)right;
                    throw new InvalidOperationException("Invalid types for minus evaluation");
                case Token_Type.MULTIPLY:
                    if (IsNumber(left, right)) return (double)left * (double)right;
                    throw new InvalidOperationException("Invalid types for multlipy evaluation");
                case Token_Type.DIVIDE:
                    if (IsNumber(left, right)) return (double)left / (double)right;
                    throw new InvalidOperationException("Invalid types for divide evaluation");
                case Token_Type.MODULUS:
                    if (IsNumber(left, right)) return (double)left % (double)right;
                    throw new InvalidOperationException("Invalid types for modulus evaluation");
                case Token_Type.POWER:
                    if (IsNumber(left, right))
                    {
                        double salida = 1;
                        for (int i = 0; i < (double)right; i++)
                        {
                            salida *= (double)left;
                        }
                        return salida;
                    }
                    throw new InvalidOperationException("Invalid types for power evaluation");

                case Token_Type.GREATER:
                    if (IsNumber(left, right)) return (int)left > (int)right;
                    throw new InvalidOperationException("Invalid types for greater evaluation");
                case Token_Type.LESS:
                    if (IsNumber(left, right)) return (int)left < (int)right;
                    UnityEngine.Debug.Log($"tenemos como hijo izquierdo a{left.GetType()} y derecho{right.GetType()} para aplicarle el {type}");
                    throw new InvalidOperationException("Invalid types for less evaluation");
                case Token_Type.LESS_EQUAL:
                    if (IsNumber(left, right)) return (int)left <= (int)right;
                    throw new InvalidOperationException("Invalid types for less_equal evaluation");
                case Token_Type.GREATER_EQUAL:
                    if (IsNumber(left, right)) return (int)left >= (int)right;
                    throw new InvalidOperationException("Invalid types for greater_equal evaluation");

                case Token_Type.AND:
                    if (IsBoolean(left, right)) return (bool)left && (bool)right;
                    throw new InvalidOperationException("Invalid types for And evaluation");
                case Token_Type.OR:
                    if (IsBoolean(left, right)) return (bool)left || (bool)right;
                    throw new InvalidOperationException("Invalid types for Or evaluation");

                case Token_Type.EQUAL:
                    return IsEqual(left, right);
                case Token_Type.NOT_EQUAL:
                    return !IsEqual(left, right);

                case Token_Type.CONCAT:
                    return left.ToString() + right.ToString();

                default: return null;
            }
        }
        public static object EvaluateUnary(string value, object son)
        {
            switch (value)
            {
                case "!":
                    if (IsBoolean(son)) { GameContext.Assignment[son.ToString()] = !(bool)son; return !(bool)son; }
                    break;
                case "++":
                    if (IsNumber(son)) { GameContext.Assignment[son.ToString()] = (int)son + 1;  return (int)son + 1; }
                    break;
                case "--":
                    if (IsNumber(son)) { GameContext.Assignment[son.ToString()] = (int)son - 1; return (int)son - 1; }
                    break;
                default: return null;
            }
            return null;
        }
        public static bool CheckRange(List<string> range)
        {
            foreach (var node in range) 
            {
                Console.WriteLine(node);         
                if (node != "m" && node != "r" && node != "s") { return false; }
            }
            return true;
        }
        public static bool CheckSource(string source) 
        {
            List<string> list =new List<string> {"Hand1","Hand2","Deck1","Deck2","A1","A2","D1","D2","G1","G2","Graveyard1","Graveyard2","Clima21", "Clima22", "Clima23", "Clima11", "Clima12", "Clima13" };
            foreach (var item in list) 
            {
                if(source == item) return true; 
            }
            return false;
        }
        public static object ReturnList(string i1,string i2,Context context,params object[] parametros) 
        {
            UnityEngine.Debug.Log($"tenemos ccomo primer string{i1} y como segundo {i2}");

            if (GameContext.IsContainsAssignment(i1))
            {
                object obj = GameContext.Assignment[i1];
                if (obj is Context cont)
                {
                    UnityEngine.Debug.Log($"Entroooooooooooooooooo");
                    Type tipo = cont.GetType();
                    if (parametros.Length == 0)
                    {
                        UnityEngine.Debug.Log("Entro a propiedad");
                        PropertyInfo property = tipo.GetProperty(i2);
                        if (property is not null)
                        {
                            UnityEngine.Debug.Log("es una propiedad");
                            return property.GetValue(cont);
                        }
                        UnityEngine.Debug.Log("No es un propiedad");
                    }
                    UnityEngine.Debug.Log("comprobemos si es un metodo");
                    MethodInfo method = tipo.GetMethod(i2);
                    if (method is not null)
                    {
                        UnityEngine.Debug.Log("Es un metodo");
                        // Invocar el método y devolver el resultado
                        return method.Invoke(cont, parametros);
                    }
                    else { throw new InvalidOperationException("El metodo no existe en nuestro contexto"); }
                }
                else
                {
                   Type type = obj.GetType();
                    if (parametros.Length == 0)
                    {
                        PropertyInfo property = type.GetProperty(i2);
                        if (property is not null)
                        {
                            return property.GetValue(obj);
                        }
                    }
                    MethodInfo method = type.GetMethod(i2);
                    if (method is not null)
                    {
                        // Invocar el método y devolver el resultado
                        return method.Invoke(obj, parametros);
                    }
                    else { throw new InvalidOperationException("El metodo no existe en nuestro contexto"); }
                }
            }
            else 
            {
                throw new Exception("El primer elemento del access no pertenece a nuestro contexto");
            }
        }
        public static object ReturnChangeAux(object obj, string method, object[] parameters)
        {
            UnityEngine.Debug.Log($"entramos a retunchangeaux de tenemos el objeto {obj},el metodo {method}");
            if (parameters.Length != 0) UnityEngine.Debug.Log($"Tenemos parametros y son en total {parameters.Length}");

            //si el objeto no hereda de ICard eso esta mal
            if (obj is not ICard) { throw new Exception("No estamos trabajando sobre ningun coleccion de cartas disponible"); }
            UnityEngine.Debug.Log("El obj es un Icard");
            Type tipo = obj.GetType();
            MethodInfo metodo = tipo.GetMethod(method);
            if (metodo == null)
            {
                UnityEngine.Debug.Log("el method es un metodo de Icard");

                throw new InvalidOperationException($"El método '{method}' no existe en la clase {tipo.Name}.");
            }
            UnityEngine.Debug.Log("vamos a invocar el metodo");
            return metodo.Invoke(obj, parameters);    
        }
        public static Card FindCard(Targets targets,CardASTNode card)
        {
            foreach (var target in targets.targets) 
            {
                if (target.name == card.Name && target.faction == card.Faction && target.power == card.Power) return target;
            }
            return null; 
        }
        
        public static void MakeParameters(List<ASTnode> param) 
        {
            foreach (var item in param)
            {
                if (item is ColonASTNode colon)
                {
                    GameContext.InputKeyParameter(colon.left, colon.right);
                }
                else throw new InvalidOperationException("Estan mal definidos los parametros ,para definirlos es necesario utilizar dos puntos");
            }
        }
    }
} 
