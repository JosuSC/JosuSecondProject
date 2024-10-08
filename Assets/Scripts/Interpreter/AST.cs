﻿using Skyrim_Interpreter;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
//using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Tilemaps;

namespace Skyrim_Interpreter
{
    public abstract class ASTnode 
    {
        public abstract object Evaluar(Context context, Targets targets);
        public abstract object Evaluar();
    }
    public class ASTnodeTree : ASTnode
    {
      public List<ASTnode> children;
        public ASTnodeTree()
        {
            children= new List<ASTnode>();
        }
        public override object Evaluar(Context context, Targets targets)
        {
            var results = new List<object>();
            foreach (var child in children)
            {
                try
                {
                    var result = child.Evaluar(context, targets);
                    results.Add(result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error evaluating child node {child}: {ex.Message}");
                    throw;
                }
            }
            return results;
        }
        public override object Evaluar()
        {
            throw new NotImplementedException();
        }
    }
    public class PlusAST : ASTnode
    {
        public Token_Type type = Token_Type.PLUS;
        public ASTnode LeftChild { get; set; }
        public ASTnode RightChild { get; set; }

        public PlusAST(ASTnode left, ASTnode right)
        {
            LeftChild = left;
            RightChild = right;
        }

        public override object Evaluar() 
        {
            UnityEngine.Debug.Log("Entramos a evaluar un suma");
            UnityEngine.Debug.Log("eavluamos su hijo izq");
            var left = LeftChild.Evaluar();
            UnityEngine.Debug.Log("evaluamos su hd");
            var right = RightChild.Evaluar();
            return Ayudante.EvaluateBinary(left,this.type,right);
        }

        public override object Evaluar(Context context, Targets targets){ return Evaluar();}
    }
    public class IdentifierASTNode : ASTnode
    {
        public Token_Type type { get; set; }
        public string value { get; set; }
        public bool Have { get; set; }
        public ASTnode Parameters { get; set;}
        public IdentifierASTNode(Token_Type type, string value) 
        {
            this.type = type;   
            this.value = value;
            Have = false;
        }
        public override object Evaluar() 
        {
            UnityEngine.Debug.Log("Comenzamos a enavluar un identificador ");
            UnityEngine.Debug.Log($"evaluamos un identificador de {value}");
            if (GameContext.Assignment.ContainsKey(value))
            {
                UnityEngine.Debug.Log("esta en gamecontext");
                return GameContext.Assignment[value];
            }
            else if (Context.Asignments.ContainsKey(value)) 
            {
                UnityEngine.Debug.Log("esta en context del juego");
                return Context.Asignments[value];
            }
            else if (GameContext.Parameters.ContainsKey(value)) 
            {
                UnityEngine.Debug.Log("esta en parametros");
                return GameContext.Parameters[value];
            }
            return value ;
        }
        public override object Evaluar(Context context, Targets targets)
        {
            return Evaluar();
        }
    }
    public class MinusASTNode : ASTnode
    {
        Token_Type type = Token_Type.MINUS;

        public ASTnode LeftChild { get; set; }
        public ASTnode RightChild { get; set; }

        public MinusASTNode(ASTnode left, ASTnode right)
        {
            LeftChild = left;
            RightChild = right;
        }

        public override object Evaluar()
        {
            UnityEngine.Debug.Log("evaluar -");
            UnityEngine.Debug.Log("izq");
            var left = LeftChild.Evaluar();
            UnityEngine.Debug.Log("derecho");
            var right = RightChild.Evaluar();
            return Ayudante.EvaluateBinary(left, this.type, right);
        }
        public override object Evaluar(Context context, Targets targets) { return Evaluar(); }
        
    } 
    public class PowerASTNode : ASTnode
    {
        Token_Type type = Token_Type.POWER;
        public ASTnode Number { get; set; }
        public ASTnode Pow { get; set; }

        public PowerASTNode(ASTnode number, ASTnode pow)
        {
            this.Number = number;
            this.Pow = pow;
        }

        public override object Evaluar()
        {

            var left = Number.Evaluar();
            var right = Pow.Evaluar();
            return Ayudante.EvaluateBinary(left, this.type, right);
        }
        public override object Evaluar(Context context, Targets targets) { return Evaluar();}
    }
    public class AndASTNode : ASTnode
    {
        public ASTnode left { get; set; }
        Token_Type type = Token_Type.AND;

        public ASTnode right { get; set; }
        public AndASTNode(ASTnode left, ASTnode rigth)
        {
            this.left = left;


            this.right = rigth;
        }
        public override object Evaluar()
        {
            UnityEngine.Debug.Log("eavlaur and");
            UnityEngine.Debug.Log("izq");
            var left = this.left.Evaluar();
            UnityEngine.Debug.Log("derech");
            var right = this.right.Evaluar();
            return Ayudante.EvaluateBinary(left, this.type, right);
        }
        public override object Evaluar(Context context, Targets targets) { return Evaluar(); }
        
    }
    public class OrASTNode : ASTnode
    {
        public ASTnode left { get; set; }
        Token_Type type = Token_Type.OR;

        public ASTnode right { get; set; }

        public OrASTNode(ASTnode left, ASTnode right)
        {

            this.left = left;
            this.right = right;
        }
        public override object Evaluar()
        {
            UnityEngine.Debug.Log("Evaluar un or");
            UnityEngine.Debug.Log("izquierdo");
            var left = this.left.Evaluar();
            UnityEngine.Debug.Log("derecho");
            var right = this.right.Evaluar();
            return Ayudante.EvaluateBinary(left, this.type, right);
        }
        public override object Evaluar(Context context, Targets targets) { return Evaluar(); }
    }
    public class NotEqualASTNode : ASTnode
    {
        public Token_Type type = Token_Type.NOT_EQUAL;
        public ASTnode Left { get; private set; }
        public ASTnode Right { get; private set; }

        public NotEqualASTNode(ASTnode left, ASTnode right)
        {

            Left = left;
            Right = right;
        }
        public override object Evaluar() 
        {
            UnityEngine.Debug.Log("Evaluar un notequ");
            UnityEngine.Debug.Log("izquierdo");
            var left = this.Left.Evaluar( );
            UnityEngine.Debug.Log("derecho");
            var right = this.Right.Evaluar( );
            return Ayudante.EvaluateBinary(left,this.type,right);
        }
        public override object Evaluar(Context context, Targets targets) { return Evaluar(); }
    }

    public class EqualASTNode : ASTnode
    {
       public  Token_Type type = Token_Type.EQUAL;
        public ASTnode Left { get; private set; }
        public ASTnode Right { get; private set; }

        public EqualASTNode(ASTnode left, ASTnode right)
        {
            Left = left;
            Right = right;
        }
        public override object Evaluar() 
        {
            UnityEngine.Debug.Log("Evaluar un ");
            UnityEngine.Debug.Log("izquierdo");
            var left = this.Left.Evaluar();
            UnityEngine.Debug.Log("derecho");
            var right = this.Right.Evaluar();
            return Ayudante.EvaluateBinary(left,this.type,right);
        }
        public override object Evaluar(Context context, Targets targets) { return Evaluar();}
    }

    public class AssignASTNode : ASTnode
    {
        Token_Type type = Token_Type.ASSIGN;
        public ASTnode Left { get;  set; }
        public ASTnode Right { get; set; }

        public AssignASTNode(ASTnode left, ASTnode right)
        {
            Left = left;
            Right = right;
        }
        public override object Evaluar(Context context, Targets targets)
        {
            UnityEngine.Debug.Log("Evaluar una asignacion");
            if (Left is IdentifierASTNode ident)
            {
                GameContext.InputKeyAssign(ident,this.Right,context,targets);
            }
            else 
            {
                throw new InvalidOperationException("Solo se puede hacer asignacion a variables");
            }
            return true;
        }
        public override object Evaluar() { return Evaluar();}
    }

    public class AssgnWithValueASTNode : ASTnode
    {
       public  string value { get; set; }  
        public  ASTnode left { get; set; }
        public ASTnode right { get; set; }
        public AssgnWithValueASTNode( ASTnode left, string value, ASTnode right)
        {
            this.value = value;
            this.left = left;
            this.right = right;
        }
        public override object Evaluar(Context context, Targets targets)
        {
            UnityEngine.Debug.Log("Evaluar un assignacion ");
            if (left is IdentifierASTNode ident && GameContext.IsContainsAssignment(ident.value))
            {
                GameContext.InputAssignmentwithValue(ident, this.right, this.value, context, targets);
            }
            else if (left is AccessASTNode acc ) 
            {
                if(acc.left is IdentifierASTNode i && i.value == "target") 
                {
                    Card c = (Card)GameContext.Assignment["target"];
                    UnityEngine.Debug.Log($"El operador eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeesssssssssss {value}");
                    if (this.value != "-=" && this.value != "+=") throw new Exception("Esta mal el operador");
                    var h = right.Evaluar(context,targets);
                    if (h is not int) { throw new Exception("Debe ser int"); }
                    int y = (int)h;
                    if (acc.right is not IdentifierASTNode l || l.value != "power") throw new Exception("No estas accediendo a power");
                    Ayudante.ActualizarPowerOfCard(c,  this.value, y);
                }
            }
            else
            {
                throw new InvalidOperationException("No existe la variable en nuestro contexto");
            }
            return true;    
        }
        public override object Evaluar() { throw new NotImplementedException(); }
    }
    public class UnaryASTNode : ASTnode
    {
        public string value { get; set; }
        public ASTnode Son { get; set; }
        public UnaryASTNode(string value, ASTnode son)
        {
            this.value = value; 
            Son = son;
        }
        public override object Evaluar(Context context, Targets targets)
        {
            UnityEngine.Debug.Log("Evaluar un unary");
            if (Son is not IdentifierASTNode sn) {throw new Exception("No estamos trabajando con una variable");}
            object soo = null;
                if (!GameContext.IsContainsAssignment(sn.value)) { throw new Exception("No existe en nuestro contexto esa variable"); }
                 soo = GameContext.Assignment[sn.value];
            return Ayudante.EvaluateUnary(this.value, soo,sn.value);
        }
        public override object Evaluar() { return null; }
    }
    public class ColonASTNode : ASTnode 
    {
        public Token_Type type = Token_Type.COLON;
        public ASTnode left { get; set; }
        public ASTnode right { get; set; }
        public ColonASTNode( ASTnode left, ASTnode right)
        {
            this.left = left;
            this.right = right;
        }
        public override object Evaluar(){return null;}
        public override object Evaluar(Context context, Targets targets) { UnityEngine.Debug.Log("Evaluar un colon el cual no se deberia evaluar"); return null;}
    }
    public class Params : ASTnode
    {
        public List<ASTnode> param { get; set; }
        public Params() => param = new List<ASTnode>();
        public override object Evaluar(){ return null;}
        public override object Evaluar(Context context, Targets targets) 
        {
            UnityEngine.Debug.Log("Evaluar un parametros");
            foreach (var item in param)
            {
                item.Evaluar(context, targets);
            }
            return true;
        }
    }
    public class ConditionalASTNode : ASTnode
    {
        public ASTnode condicion { get; set; }
        public override object Evaluar()
        {
            var condtion = this.condicion.Evaluar();
            if (condtion is bool) { return (bool)condtion; }
            else { throw new InvalidOperationException("Invalid type for condition evalue"); }
        }
        public override object Evaluar(Context context, Targets targets) { return Evaluar();}
    }
    public class BlockASTNode : ASTnode
    {
        public Params Block { get; set;}
        public BlockASTNode()
        {
            Block= new Params();    
        }
        public override object Evaluar(Context context, Targets targets)
        {
            UnityEngine.Debug.Log("Evaluar un block de codigo de algun while o for o algo");
            Block.Evaluar(context,targets);
            return true;
        }
        public override object Evaluar() { return false; }
    }
    public class CommaASTNode : ASTnode 
    {
        public Token_Type type = Token_Type.COMMA;
        public string value = ",";
        public override object Evaluar() { return value; }
        public override object Evaluar(Context context, Targets targets) { throw new NotImplementedException(); }
    }

    public class AccessASTNode : ASTnode
    {
        public ASTnode left { get; set; }
        public ASTnode right { get; set; }
        public AccessASTNode(ASTnode left, ASTnode right)
        {
            this.left = left;
            this.right = right;
        }
        //ejemplos a evaluar context.hand.power.
        public override object Evaluar(Context context, Targets targets)
        {
            UnityEngine.Debug.Log("Evaluar un access");
            List<object> list = new List<object>();
            if (this.right is IdentifierASTNode i && i.Parameters != null) 
            {
                UnityEngine.Debug.Log($"Tenemos como hijo derecho a {i.value} y su parametros es {i.Parameters} ");

                var ty = i.Parameters.Evaluar(context,targets);

                UnityEngine.Debug.Log($"El parametro es {ty} y el del tipo {ty.GetType()}");
                if (ty is string )
                {
                    if (GameContext.IsContainsAssignment(ty.ToString()))
                    {
                        list.Add(GameContext.Assignment[ty.ToString()]);
                    }
                    else if (Context.Asignments.ContainsKey(ty.ToString())) 
                    {
                        list.Add(Context.Asignments[ty.ToString()]);
                    }
                    throw new Exception("No existe ese objeto en nuestro context");
                }
                else if (ty is int) 
                {
                    list.Add(ty);
                }
                list.Add(ty);
            }
            if (left is IdentifierASTNode identifier && right is IdentifierASTNode identifier2)
            {
               return Ayudante.ReturnList(identifier.value, identifier2.value, context, list.ToArray());
            }
            else if (left is AccessASTNode t && right is IdentifierASTNode ide)
            {
                UnityEngine.Debug.Log("el hijo izquierdo es un acces");
                object a = t.Evaluar(context, targets);
                UnityEngine.Debug.Log($" el object es de tipo{a.GetType()}");
                return Ayudante.ReturnChangeAux(a, ide.value, list.ToArray());
            }
                throw new InvalidOperationException("Error en los access");  
        }
         public override object Evaluar() { throw new NotImplementedException(); }
    }
    public class ActionASTNode : ASTnode
    {
        public string Target { get; set; }
        public string context { get; set; }
        public List<ASTnode> actions { get; set; }
        
        public LambdaForAction Lambda {get; set;}
        public ActionASTNode()
        {
             
            actions= new List<ASTnode>();   
        }

        public override object Evaluar(Context context, Targets targets)
        {
            UnityEngine.Debug.Log("Evaluar un action");
            for (int i = 0; i < actions.Count; i++)
            {
                actions[i].Evaluar(context, targets);   
            }
            UnityEngine.Debug.Log("terminamos de evaluar el action ");
            return true;
        }
        public override object Evaluar() { throw new NotImplementedException(); }
    }
    //*********************************************************************
    public class EffectASTNode : ASTnode    
    {
        public string Name { get; set; }
        public Params Params { get; set; }
        public ActionASTNode Action { get; set; }
        public List<ASTnode> children { get; set; }
        public EffectASTNode()
        {
            children = new List<ASTnode>();
        }
        public override object Evaluar()
        {
           Effect neweffect = new Effect();
            if (this.Name != null) { neweffect.Name = Name; }
            else { throw new InvalidOperationException("The name for effect is null"); }
            if (Params != null) 
            { 
                neweffect.Parameters = this.Params.param;
                Ayudante.MakeParameters(this.Params.param);
            }
            neweffect.Action = Action;
            GameContext.EffectAssignmet.Add(neweffect.Name,neweffect);
            return true; 
        }
        public override object Evaluar(Context context, Targets targets) { return Evaluar(); }
    }
    public class ComparationASTNode : ASTnode
    {
        public Token_Type type { get; set; }
         public  ASTnode left { get; set; }
         public  ASTnode right { get; set; }
        public ComparationASTNode(ASTnode left, Token_Type type, ASTnode right)
        {
            this.left = left;
            this.right = right;
            this.type = type;
        }
        public override object Evaluar()
        {
            UnityEngine.Debug.Log($"Evaluar un comparacion con {type}");
            UnityEngine.Debug.Log("izq");
            var left = this.left.Evaluar();
            UnityEngine.Debug.Log("derech ");
            var right = this.right.Evaluar();
            return Ayudante.EvaluateBinary(left,this.type,right);
        }
        public override object Evaluar(Context context, Targets targets) { return Evaluar(); }
    }
    public class ConcatenationASTNode : ASTnode
    {
        public Token_Type type = Token_Type.CONCAT;
        public ASTnode left { get; set; }
        public ASTnode right { get; set; }
        public ConcatenationASTNode(ASTnode left, ASTnode right)
        {
            this.left = left;
            this.right = right;
        }
        public override object Evaluar()
        {
            UnityEngine.Debug.Log("concatenacion ");
            var left = this.left.Evaluar();
           var right = this.right.Evaluar();
            return Ayudante.EvaluateBinary(left,this.type,right); 
        }
        public override object Evaluar(Context context, Targets targets) { return Evaluar(); }
    }
    public class FactorASTNode : ASTnode
    {
       public  Token_Type type { get; set; }
       public  ASTnode leftchild { get; set; }
        public ASTnode rightchild { get; set; }
        public FactorASTNode(ASTnode leftchild, Token_Type type, ASTnode rightchild)
        {
            this.type = type;
            this.leftchild = leftchild;
            this.rightchild = rightchild;
        }
        public override object Evaluar()
        {
            UnityEngine.Debug.Log($"Evaluar un factor con {type}");
            var left = leftchild.Evaluar(); 
            var right = rightchild.Evaluar();
            return Ayudante.EvaluateBinary(left,this.type,right);
        }
        public override object Evaluar(Context context,Targets targets) { return Evaluar(); }
    }
    public class LiteralASTNode : ASTnode   
    {
        public Token_Type Type { get; set; }
        public string value { get; set; }
        public LiteralASTNode(Token_Type type,string value)
        {
            this.Type = type;
            this.value = value; 
        }
        public override object Evaluar() 
        {
            UnityEngine.Debug.Log("Evaluar un literal");
            if (Type == Token_Type.NUMBER)
            {
                UnityEngine.Debug.Log("Es un entero ");
                return int.Parse(value);
            }
            else if (Type == Token_Type.STRING)
            {
                UnityEngine.Debug.Log("Es un string ");
                return value;
            }
            else if (Type == Token_Type.BOOLEAN)
            {
                UnityEngine.Debug.Log("Es un bolleano");
                return bool.Parse(value);
            }
             return null;
        }
        public override object Evaluar(Context context, Targets targets) { return Evaluar(); }
    }
    public class GroupingASTNode : ASTnode
    {
        public ASTnode groupnode { get; }
        public GroupingASTNode(ASTnode groupnode)
        {
            this.groupnode = groupnode; 
        }
        public override object Evaluar() {return null;}
        public override object Evaluar(Context context, Targets targets) 
        {
            UnityEngine.Debug.Log("Evaluar un group");
            return groupnode.Evaluar(context,targets);
        }   
    }
    public class WhileASTNode  : ASTnode
    {
        public ASTnode condition { get; set; }
        public BlockASTNode block { get; set;}
        public WhileASTNode(ASTnode condition,BlockASTNode block)
        {
            this.condition = condition; 
            this.block = block; 
        }
        public override object Evaluar(Context context,Targets targets) 
        {
            UnityEngine.Debug.Log("Evaluar un while");
            
            bool continueLoop = true;
            while (continueLoop)
            {
                var conditionResult = condition.Evaluar(context, targets);
                if (conditionResult is bool)
                {
                    continueLoop = (bool)conditionResult;
                }
                else
                {
                    throw new ArgumentException("Condition must evaluate to a boolean value");
                }
                if (continueLoop)
                {
                    UnityEngine.Debug.Log("la condicion es verdadera ");
                    block.Evaluar(context, targets);
                }
                UnityEngine.Debug.Log("Terminamos el blucle while ");
            }
            return true;
        }
        public override object Evaluar(){ throw new NotImplementedException();}
    }
    public class ForASTNode : ASTnode
    {
        public IdentifierASTNode colection { get; set; }
        public BlockASTNode block { get; set; }
        public ForASTNode(BlockASTNode block,IdentifierASTNode colection)
        {
            this.block = block; 
            this.colection = colection; 
        }
        public override object Evaluar(Context context, Targets targets)
        {
            UnityEngine.Debug.Log("Evaluar un  For");
            //Targets colec = (Targets)GameContext.Assignment[colection.value];
            foreach (Card item in targets.targets)
            {
              GameContext.ActualizarValor("target",item);      
                UnityEngine.Debug.Log("Seguimos evaluando el cuerpo del for");
              block.Evaluar(context, targets);
            }
            UnityEngine.Debug.Log("Se termino de evaluar el for ");
            return true;
        }
        public override object Evaluar(){ throw new NotImplementedException();}
    }
   //*******************************************************************************
    public class CardASTNode  : ASTnode
    {
        public string Name { get; set; }    
        public string Type { get; set; }
        public string Faction { get; set;}
        public int Power { get; set;}
        public List<string> Range { get; set;}
        public List<ASTnode> OnActivation { get; set; }
        public CardASTNode()
        {
                Range = new List<string>();    
            OnActivation = new List<ASTnode>(); 
        }
        public override object Evaluar()
        {
            Card newcard = new Card();
            if (Name != null) 
            {
                newcard.name = Name; 
            }
            if (Faction != null) { newcard.faction = Faction; }
            if (Type != null) { newcard.type = Type; }
            if (Range.Count != 0)
            {
                if (Ayudante.CheckRange(this.Range)) { newcard.Range = this.Range.ToArray(); }
                else { throw new InvalidOperationException("Invalid types for range of card evaluation"); }
            }
            else { throw new InvalidOperationException("The range for the card is empty"); }
            if (Power != null)
            {
                if ((newcard.type == "Clima" || newcard.type == "Aumento") && this.Power != 0) { throw new InvalidOperationException("Ivalid power for this card "); }
                newcard.power = Power;
            }
            else { throw new InvalidOperationException("The power card is empty"); }
            if (this.OnActivation.Count != 0)
            {
                newcard.OnActivation = OnActivation;
            }
            else 
            {
                throw new InvalidOperationException("This card don't have any Effcet");
            }
            newcard.IsCreated = true;
            newcard.EffectType = CardEffects.Created;
            GameContext.Cards.Add(this.Name,newcard);
            return true; 
        }
        public override object Evaluar(Context context ,Targets targets)
        {
            return Evaluar();
        }
    }
    public class EffectCardNode : ASTnode
    {
        public string Name { get; set; }    
        public List<ASTnode> Parameters { get; set; }

        public SelectorCardNode Selector { get; set; }
        public EffectCardNode()
        {
            Parameters = new List<ASTnode>();   
        }
        public override object Evaluar(Context context, Targets targets)
        {
           throw new NotImplementedException(); 
        }

        public override object Evaluar()
        {
            throw new NotImplementedException();
        }
    }
    public class SelectorCardNode : ASTnode
    {
        public string Source { get; set;}
        public bool Single { get; set;}

        public ASTnode Predicate { get; set; }
        public SelectorCardNode()
        {
            Single = false;
        }

        public override object Evaluar(Context context, Targets targets)
        {
            throw new NotImplementedException();
        }
        public override object Evaluar()
        {
            throw new NotImplementedException();
        }
    }
    public class LambdaForAction  :ASTnode
    {
        public List<ASTnode> left { get; set; }
        public List<ASTnode> right { get; set; }

        public LambdaForAction(List<ASTnode> left, List<ASTnode> right)
        {
            this.left = left;
            this.right = right;
        }
        public override object Evaluar(Context context, Targets targets){throw new NotImplementedException();}

        public override object Evaluar() {throw new NotImplementedException();}
    }
    public class LambdaASTNode : ASTnode 
    {
       public  Token_Type type = Token_Type.LAMBDA;
        public ASTnode Left { get; set; }
        public ASTnode Right { get; set; }
        public LambdaASTNode(ASTnode left,ASTnode right)
        {
                Left = left;    
                Right = right;
        }
        public override object Evaluar(Context context, Targets targets)
        {
            UnityEngine.Debug.Log("Evaluar un lambda");
            UnityEngine.Debug.Log("izq");
            var left = Left.Evaluar(context, targets);
            UnityEngine.Debug.Log("derech ");
            var right = Right.Evaluar(context, targets);

            if (left is not string st) throw new InvalidOperationException("No estas pasando un objeto");
            if (!GameContext.IsContainsAssignment(st)) throw new Exception("No existe ese objeto en nuestro contexto");
            if (right is true) return GameContext.Assignment[st];
            else { return false; }
        }
        public override object Evaluar()
        {
            return null;
        }

    }
    public class SemicolomASTNode : ASTnode 
    {
        public string value = ";";
            public override object Evaluar(Context context, Targets targets)
        {
            throw new NotImplementedException();
        }
        public override object Evaluar()
        {
            throw new NotImplementedException();
        }
    }

}