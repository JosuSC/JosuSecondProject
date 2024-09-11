using System;
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
                    if (ecn.Selector == null) { throw new InvalidOperationException("No podemos enviarles los targets al effecto si no tenemos un selector definido"); }
                    SelectorCardNode myselcetor = ecn.Selector;
                    if (myselcetor.Source == null) throw new InvalidOperationException("Necesitamos que en el selector ");
                    if (myselcetor.Predicate is not LambdaASTNode lambda) throw new InvalidOperationException("Estamos presentando problemas con el predicate del selector");
                    List<Card> source = context.GetListByName(myselcetor.Source);
                    if (source == null) throw new Exception("El source no existe en nuestro contexto");
                    Targets objective = DrawCards(source,lambda,context);
                    //ya tenemos el objetivo del effecto
                    myeffect.Targets = objective;
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
            }
            else if (rightchild is EqualASTNode equal)
            {
                if (!(equal.Left is AccessASTNode acces)) { throw new InvalidOperationException("El hijo izquierdo del operador logico es debe ser del tipo acces"); }
                a = acces;
                t = equal.type;
                r = equal.Right;
            }
            else if (rightchild is NotEqualASTNode notequal)
            {
                if (!(notequal.Left is AccessASTNode acces)) { throw new InvalidOperationException("El hijo izquierdo del operador logico es debe ser del tipo acces"); }
                a = acces;
                t = notequal.type;
                r = notequal.Right;
            }
            else 
            {
                throw new InvalidOperationException("El hijo derecho del operador lambda esta mal");
            }
            foreach (var item in source) 
            {
                value = IsAccess(a, item);
                if (Ayudante.EvaluateBinary(value, t, r.Evaluar()) is true) 
                {
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

        private static bool IsAnyOfTheCard(string any) 
        {
            List<string> things =new List<string> {"Name","Faction","Power","Type","Range"};
            if(things.Contains(any)) return true;   
            return false;
        }
        private static bool IsBooleanOperation(ASTnode node) => node is EqualASTNode || node is NotEqualASTNode || node is ComparationASTNode;
        private static object IsAccess(AccessASTNode node, Card card)
        {
            if ( node.left is IdentifierASTNode && node.right is IdentifierASTNode i)
            {
                return i.value switch
                {
                    "Name" => card.name,
                    "Faction" => card.faction,
                    "Range" => card.Range,
                    "Type" => card.type,
                    "Power" => card.power,
                    _ => throw new InvalidOperationException("No estás accediendo a ningún atributo de la carta")
                };
            }
            throw new InvalidOperationException("El hijo izquierdo del operador lógico debe ser del tipo accesos");
        }
    }
}
