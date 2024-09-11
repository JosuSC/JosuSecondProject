using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml.Linq;
using UnityEngine;
using System.Xml;

namespace Skyrim_Interpreter
{
    internal class Parser
    {
        private List<Token> tokens;
        private int currentPosition;
        private List<string> Errors;
        private ASTnodeTree MyTree;
        static Token comprobar;

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
            this.currentPosition = 0;
            this.Errors = new List<string>();
            comprobar = null;
            MyTree = new ASTnodeTree(); 
        }

        #region Metodos auxilar
        //metodos auxuliares 
        //----------------------------------------------------------------------------------------------------------------
        //Return the actual token
        private Token Peek() 
        {
            return tokens[currentPosition]; 
        }
        
        //Comprueba si el token actual coincide con el token dado
        private bool Check(Token_Type type) 
        {
            if (IsAtEnd()) return false;
            return Peek().Type == type;
        }
        private bool CheckValue(string value) 
        {
            if (IsAtEnd()) return false;
            return Peek().Value == value;   
        }

        public void Avisar(string esperado,string actual) 
        {
            Console.WriteLine($"Se esperaba un{esperado} y en su lugar tenemos {actual}");
        }

        //Verifica si ya se llego al final de la lista de tokens
        private bool IsAtEnd() 
        {
            return Peek().Type == Token_Type.EOF;
        }
        
        //Avanza al siguiente token
        private Token Advance()
        {
            if (!IsAtEnd()) currentPosition++;
            return Previous();
        }
        //Devuelve el token anterior
        private Token Previous()
        {
            return tokens[currentPosition - 1];
        }

        private Token PeekNext() 
        {
            if (IsAtEnd()) return tokens[currentPosition];

            return tokens[currentPosition++];
            
        }

        // consume si el token actual coincide con el tipo dado
        private Token Consume(Token_Type type, string message)
        {
            if (Check(type)) return Advance();
           
            Console.WriteLine($"Se esperaba un token del tipo {type}");
            return null;
        }

        private bool Match(params Token_Type[] types)
        {
            foreach (Token_Type type in types)
            {
                if (Peek().Type == type)
                {
                    PeekNext();
                    return true;
                }
            }
            return false;
        }

        //-----------------------------------------------------------------------------------------------------------------------------

        #endregion

        public void Parse()
        {
            KeywordNode();
        }
        private ASTnode KeywordNode()
        {
           //comprobamos si el nodo es de tipo effect o card
            if ( (Peek().Value == "effect" || Peek().Value == "card"))
            {
                Token keyword = Peek();
                if (keyword.Value == "effect") {MyTree.children.Add (EffectNode()); }
                else if (keyword.Value == "card") { MyTree.children.Add(CardNode()); }
            }
            Console.WriteLine(Peek()) ;
                while (Peek().Type != Token_Type.EOF) 
                {
                  Console.WriteLine(Peek());
                    if (Peek().Value != "}" && Peek().Value != ";")
                    {
                        if (Peek().Value == "card" || Peek().Value == "effect")
                        {
                            return KeywordNode();
                        }
                        else
                        {
                            throw new Exception("Se debe definir una carta o efecto");
                        }
                    }
                    //vamos a meter todas las cartas creadas en la lista de cartas de el jugador 1
                    Advance();
                }
            return MyTree;
        }
        #region Effect
        //creamos el nodo effect
        private EffectASTNode EffectNode()
        {
            EffectASTNode effect = new EffectASTNode();
            Advance();
            if (!Check(Token_Type.DELIMITIER)) { Console.WriteLine($"Se esperaba un delimitador y tenemos un {Peek().Value}"); return null; }
            Advance();
            //nombre de efecto
            if (Check(Token_Type.IDENTIFIER) && Peek().Value == "Name")
            {
                Advance();
                comprobar = Consume(Token_Type.COLON,$"Se esperaba un : y tenemos un{Peek().Value}");
                if (comprobar == null) { Console.WriteLine($"Se esperaba : despues del name y tenemos {Peek().Value}"); }
                comprobar = Consume(Token_Type.STRING, $"Se esperaba el nombre del effecto  tenemos {Peek().Value}");
                if (comprobar == null) { Console.WriteLine($"No esta bien el nombre de la carta ya que tenemos un {Peek().Value}"); return null; }
                effect.Name = Previous().Value.Replace("\"","");                          
             comprobar =  Consume(Token_Type.COMMA,$"se esperaba una comma despues del name y tenemos un {Peek().Value}");
                if (comprobar == null) { Console.WriteLine("null  en 144 ya que se esperaba una comma despues del name del effecto y tenemos un {Peek().Value}"); return null;}
            }
            else { Console.WriteLine($"Se esperaba obtener el nombre del efecto y tenemos {Peek().Value}"); return null; }   
            if (Peek().Value == "Params") 
            {
                Advance();
               Params parametros = new Params();
               parametros = ParamsNode(parametros);
                if (parametros == null) { Console.WriteLine("null en 152 , los parametros estuvieron nulos"); return null; }
                effect.Params= parametros;  
            }

            if (CheckValue("Action"))
            {
                Advance();
                ActionASTNode action = new ActionASTNode();
                action = ActionNode(action);
                effect.Action = action;
                Advance();
            }
            else { Console.WriteLine($"Estes effecto no tiene accion en su lugar tiene{Peek().Value}"); return null; }
            comprobar = Consume(Token_Type.DELIMITIER,$"se esperaba un delitador y tenemos un {Peek().Value}");
            if (comprobar == null) { Console.WriteLine("null en 173"); return null; }
            //meter este effect en el dictionario
            effect.Evaluar();
            return effect;  
        }
        //los parametros de effect,en el caso que tenga
        private Params ParamsNode(Params parametros)
        {
           comprobar = Consume(Token_Type.COLON,$"Se esperaban dos puntos y tenemos {Peek().Value}");
            if (comprobar == null) { Console.WriteLine("null en 176"); return null; }
            comprobar = Consume(Token_Type.DELIMITIER,$"Se Esperaba un delimitador y tenemos{Peek().Value}");
            if (comprobar == null) { Console.WriteLine("null en 178"); return null; }
            while (Peek().Value != "}") 
            {
                Console.WriteLine(Peek());
                parametros.param.Add(CreateNode());
                if(Check(Token_Type.COMMA)) Advance();
            }
            Advance();
            return parametros;
        }
        //el action de effect
        private ActionASTNode ActionNode(ActionASTNode action)
        {
            comprobar = Consume(Token_Type.COLON,$"se esperaba un : y tenemos un {Peek().Value}");
            if (comprobar == null) { Console.WriteLine("null en 192"); return null; }
            if (Check(Token_Type.LEFT_PAREN))
            {
                Advance();
                if (Check(Token_Type.IDENTIFIER)) action.Target = Peek().Value;
                if (!GameContext.IsContainsAssignment(action.Target)) { Console.WriteLine("Esa lista de cartas no existe"); /*return null; */}
                Advance();
                comprobar = Consume(Token_Type.COMMA, $"Se esperaba una , y tenemos un {Peek().Value}");
                if (comprobar == null) { Console.WriteLine($"se esperaba una comma entre los targets y el context en el action del effect actual"); return null; }
                if (Check(Token_Type.IDENTIFIER)) action.context = Peek().Value;
                Advance();
                if (!CheckValue(")")) { Console.WriteLine($"Se esperaba un ) y tenemos {Peek().Value}"); }
            }
            else { Console.WriteLine($"null en 203 ya que se esperaba un ( y tenemos un {Peek().Value}"); return null; }
            Advance();
           comprobar = Consume(Token_Type.LAMBDA,$"Se esperaba un => y en su lugar tenemos a un {Peek().Value}");
            if (comprobar == null) { Console.WriteLine("null en 206"); return null; }
                if (CheckValue("{"))
                {
                    Advance();
                    while (Peek().Value != "}")         //hasta aqui estas bien
                    {
                        action.actions.Add(CreateNode());
                        if (Check(Token_Type.SEMICOLON)) Advance();
                    }
                  Advance();
                }
                else { Console.WriteLine($"Se esperaba un delimitador despues del lambda y tenemos en su lugar {Peek().Value}"); return null; }
            return action;
        }
        #endregion
        #region Card
        private ASTnode CardNode() 
        {
            CardASTNode card = new CardASTNode();
            Advance();
            comprobar = Consume(Token_Type.DELIMITIER,$"se esperaba un delimitador despues de card y tenemos en su lugar{Peek().Value}");
            if (comprobar == null) { Console.WriteLine("null en 253"); return null;}
            card = Meter("Type", card);
            if (card == null) { Console.WriteLine("null en 257 ya que la carta no tiene Type"); return null; };
            card = Meter("Name", card);
            if (card == null) { Console.WriteLine("null en 255 ya que la  carta no tiene nombre"); return null; };
            card = Meter("Faction", card);
            if (card == null) { Console.WriteLine("null en 259 ya que la cata no tiene Faction"); return null; };
                                                                                        //hasta aqui esta perfecto
            if (Check(Token_Type.IDENTIFIER) && Peek().Value == "Power")
            {
                Advance();
                while (Peek().Type != Token_Type.NUMBER) { Advance(); }
                card.Power = Convert.ToInt32(Peek().Value);
                Advance();
                comprobar = Consume(Token_Type.COMMA, $"se esperaba una , y en su lugar tenemos {Peek().Value}");
                if (comprobar == null) { return null; }
            }
            else { Console.WriteLine("null en 270 ya que la carta no tiene Power"); return null; }
            //bien
            if (Check(Token_Type.IDENTIFIER) && Peek().Value == "Range")
            {
                Advance();
                card = MeterenRange(card);
                if (card == null) return null;
                comprobar = Consume(Token_Type.COMMA, $"se esperaba una comma y tenemos un {Peek().Value}");
                if (comprobar == null) return null;
            }
            else { Console.WriteLine("La carta no tiene Range"); return null;}
            //espermos el OnActivation
            if (Check(Token_Type.IDENTIFIER) && Peek().Value == "OnActivation")
            {
                Advance();
                comprobar = Consume(Token_Type.COLON, $"se esperaba : despues de OnActivation y tenemos un {Peek().Value}");
                comprobar = Consume(Token_Type.DELIMITIER, $"Se esperaba un [ y tenemos un {Peek().Value}");
                if (comprobar == null) return null;
                card = OnActivationList(card);
            }
            else { Console.WriteLine("La carta es tiene OnActivation"); }
            if (!CheckValue("}")){ Console.WriteLine($"Falta un delimitador de cerrada y tenemos es su lugar tenemos {Peek().Value}"); return null; }
            Advance();
           card.Evaluar();
            return card;
        }
        private CardASTNode OnActivationList(CardASTNode card) 
        {
            EffectCardNode effectCard = new EffectCardNode();
            //seguimos buscando hasta no encontar ] 
            while (Peek().Value != "]") 
            {
                //si llegamos al final rompemos el bucle;
                if (Peek().Type == Token_Type.EOF) { Console.WriteLine("Se llego al final y no se logro completar el OnActivation"); break; }
                //si encontramos una {
                if (Peek().Value == "{")
                {
                    Advance();
                    //buscamos hasta no encontar la otra llave
                    while (Peek().Value != "}")
                    {
                        if (Peek().Type == Token_Type.EOF) { Console.WriteLine("Se llego al final y no se logro completar el effect"); break; }
                        if (Peek().Value == "Effect")
                        {
                            Advance();
                            effectCard = EffectForCard();
                            card.OnActivation.Add(effectCard);
                        }
                        if (Peek().Value != "}")
                        {
                            Advance();
                        }
                    }
                    Advance();
                    if (Peek().Value != "]") { comprobar = Consume(Token_Type.COMMA, $"Se esperaba una , y tenemos {Peek().Value}"); if (comprobar == null) { return null; } }
                }
                else { Console.WriteLine("Se esperaba un {");   return null;}  // esta bien                             //hasta aqui esta bien
            }
            Advance();  
            return card;
        }
        private EffectCardNode EffectForCard() 
        {
            EffectCardNode effcard = new EffectCardNode();
            //comprobamos que hayan : despues de la palabra Effect
            comprobar = Consume(Token_Type.COLON,$"se esperaba un : y tenemos un {Peek().Value}");
            if (comprobar == null) return null;
            // si despues de los dos puntos hay un string "..."(Effect : "Damage") lo devolvemos
            if (Peek().Type == Token_Type.STRING)
            {
                effcard.Name = Peek().Value.Replace("\"","");
                if (!GameContext.IsContainsEffcet(effcard.Name)) { Console.WriteLine("Ese effecto no existe en nuestro contexto"); }
                Advance();
                return effcard; 
            }
            //revisamos si hay un { , y hasta no encontrar } seguimos buscando y guardando propiedades en el effect 
            comprobar = Consume(Token_Type.DELIMITIER,$"se esperaba un delimiter y tenemos un {Peek().Value}");
            if (comprobar == null) return null;
            while (Peek().Value != "}") 
            {
                if (Peek().Value == "Name")
                {
                    Advance();
                    if (!Check(Token_Type.COLON))
                    {
                        Console.WriteLine($"Se esperaba dos puntos y tenemos un {Peek().Value}");
                        return null;
                    }
                    Advance();
                    if (!Check(Token_Type.STRING)) { Console.WriteLine($"Se esperaba tener el nombre de la carta y en su lugaer tenemos {Peek().Value}"); }
                    effcard.Name = Peek().Value.Replace("\"","");
                    if (!GameContext.IsContainsEffcet(effcard.Name)) { Console.WriteLine("No existe ese efecto en nuestro contexto"); }
                    Advance();
                    comprobar = Consume(Token_Type.COMMA, $"se esperaba , y tenemos un {Peek().Value}");
                }
                else { Console.WriteLine($"No tenemos nombre para definir el effecto que tiene la carta y tenemos {Peek().Value}"); }
                if (Peek().Type == Token_Type.IDENTIFIER)
                {
                    while (!CheckValue("}"))
                    {
                        if (Check(Token_Type.EOF)) { Console.WriteLine("Llegamos al final y no termiamos de poner los parametros"); return null; }
                        effcard.Parameters.Add(CreateNode()); 
                        comprobar = Consume(Token_Type.COMMA, $"Se esperaba un , y en su lugar tenemos un {Peek().Type}");
                    }
                }
            }
            Advance();
            //encontramos el selector
            if (CheckValue("Selector"))
            {
                Advance();
                // comprobamos los dos puntos
                comprobar = Consume(Token_Type.COLON, $"se esperaba : y tenemos un {Peek().Value}");
                if (comprobar == null) return null;
                comprobar = Consume(Token_Type.DELIMITIER, $"se esperaba delimitadior y tenemos un {Peek().Value}");
                if (comprobar == null) return null;
                while (!CheckValue("}"))
                {
                    effcard.Selector = SelectorForCard();
                }
            }
            Advance();
            //metemos en el efecto de su nombre el selector para obtener la funte y su predicado
            return effcard;
        }
        private SelectorCardNode SelectorForCard() 
        {
            SelectorCardNode selector = new SelectorCardNode();
            if (CheckValue("Source"))
            {
               Advance();   
                comprobar = Consume(Token_Type.COLON, $"se esperaba : y tenemos {Peek().Value}");
                if (comprobar == null) return null;
                if (Check(Token_Type.STRING))
                {
                    selector.Source = Peek().Value.Replace("\"","");
                    if (!GameContext.IsContainsAssignment(selector.Source)) { Console.WriteLine("No existe ese campo en nuestro contexto"); }
                    Advance();
                    comprobar = Consume(Token_Type.COMMA, $"Se esperaba una , y tenemos un {Peek().Value}");
                    if (comprobar == null) { return null; }
                }
                else { Console.WriteLine("Se esperaba la especificacion del source"); return null; }
            }
            if (CheckValue("Single"))
            {
                Advance();
                comprobar = Consume(Token_Type.COLON, "se esperaba un :");
                if (comprobar == null) return null;
                if (Check(Token_Type.BOOLEAN)) selector.Single = Convert.ToBoolean(Peek().Value);
                Advance();
                comprobar = Consume(Token_Type.COMMA, $"se esperaba , y tenemos {Peek().Value}");
            }
            if (CheckValue("Predicate"))
            {
                Advance();
                comprobar = Consume(Token_Type.COLON,"se esperaba : despuesde Predicate");
                Console.WriteLine(Peek());
                selector.Predicate = CreateNode();
            }
            else { Console.WriteLine($"Es necesario que el selector tenga Predicate y no tiene, en su lugar tiene {Peek().Value}");}
            return selector;
        }
        private CardASTNode MeterenRange(CardASTNode card) 
        {
            comprobar = Consume(Token_Type.COLON,$"se esperaba :despues de Range y tenemos {Peek().Value}");
            comprobar = Consume(Token_Type.DELIMITIER,$"se esperaba un delimitador y tenemos {Peek().Value}");
            if (comprobar == null) { return null; }
            while (Peek().Type != Token_Type.DELIMITIER) 
            {
                if (Peek().Type == Token_Type.STRING) { card.Range.Add(Peek().Value.Replace("\"", "")); }
                Advance();
                if (Peek().Type != Token_Type.DELIMITIER) { comprobar = Consume(Token_Type.COMMA, "se esperaba una comma");}
            }
            Advance();
            return card;
        }
        private CardASTNode Meter(string value,CardASTNode card) 
        {
            bool t = false, n = false , f = false;
            if (value == "Name") { n = true; }
            else if (value == "Type") { t = true; }
            else if (value == "Faction") { f = true; }
            comprobar = Consume(Token_Type.IDENTIFIER, "se esperaba el type");
            if (comprobar == null) { return null; }
            while (Peek().Type != Token_Type.STRING) { Advance(); }
            if (t) { card.Type = Peek().Value.Replace("\"", ""); }
            else if (n) { card.Name = Peek().Value.Replace("\"", ""); }
            else if (f) { card.Faction = Peek().Value.Replace("\"", ""); }
            Advance();
            comprobar = Consume(Token_Type.COMMA,"se esperaba una ,");
            if (comprobar == null) { return null; }
            return card;
        }
        #endregion
        #region Make nodos
        //creamos nodos
        private ASTnode CreateNode() {return LogicalNode();}
        //&& y ||
        private ASTnode LogicalNode()
        {
            ASTnode node = EqualASTNode();
            while (Match(Token_Type.AND, Token_Type.LOGIC))
            {
                Token boolean = Previous();
                ASTnode right = CreateNode();
                if (boolean.Type == Token_Type.AND) node = new AndASTNode(node, right);
                else { node = new OrASTNode(node, right); }
            }
           return node;
        }
        // == y !=
        private ASTnode EqualASTNode()
        {
            ASTnode node = ComparasionASTNode();
            while (Match(Token_Type.EQUAL,Token_Type.NOT_EQUAL) )
            {
                Token equality = Previous();
                ASTnode right = CreateNode();
                if (equality.Value == "==") { node = new EqualASTNode(node, right); }
                else { node = new NotEqualASTNode(node, right); }
            }
            return node;
        }
        // < ,> , <= , >=
        private ASTnode ComparasionASTNode() 
        {
            ASTnode node = ConcatenationASTNode();
            while (Match(Token_Type.GREATER,Token_Type.GREATER_EQUAL,Token_Type.LESS,Token_Type.LESS_EQUAL))
            {
                Token comparasion = Previous();
                ASTnode right = CreateNode();
                if (comparasion.Value == ">") { node = new ComparationASTNode(node, Token_Type.GREATER, right); }
                else if (comparasion.Value == ">=") { node = new ComparationASTNode(node, Token_Type.GREATER_EQUAL, right); }
                else if (comparasion.Value == "<") { node = new ComparationASTNode(node, Token_Type.LESS, right); }
                else if (comparasion.Value == "<=") { node = new ComparationASTNode(node, Token_Type.LESS_EQUAL, right); }
            }
            return node;        
        }
        //@@
        private ASTnode ConcatenationASTNode() 
        {
            ASTnode node = TermASTNode();
            while (Match(Token_Type.CONCAT))
            {
                Token concat = Previous();
                ASTnode right = CreateNode();
                node = new ConcatenationASTNode(node, right);
            }
            return node;
        }
        // + , -
        private ASTnode TermASTNode() 
        {
            ASTnode node = FactorNode();
            while (Match(Token_Type.PLUS , Token_Type.MINUS)) 
            {
                Token term = Previous();
                ASTnode right = CreateNode();
                if (term.Value == "+") { node = new PlusAST(node, right); }
                else
                {
                    node = new MinusASTNode(node, right);   
                }
            }
            return node;
        }
        // *,/,%
        private ASTnode FactorNode() 
        {
            ASTnode node = PowerNode();
            while (Match(Token_Type.DIVIDE,Token_Type.MULTIPLY,Token_Type.MODULUS)) 
            {
                Token fact = Previous();    
                ASTnode right = CreateNode();
                if (fact.Value == "*") { node = new FactorASTNode(node,Token_Type.MULTIPLY,right); }
               else if (fact.Value == "/") { node = new FactorASTNode(node,Token_Type.DIVIDE,right); }
               else  if (fact.Value == "%") { node = new FactorASTNode(node,Token_Type.MODULUS,right); }
            }
            return node;    
        }
        //^
        private ASTnode PowerNode() 
        {
            ASTnode node = AssignationNode();
            while (Match(Token_Type.POWER)) 
            {
                Token power = Previous();
                ASTnode right = CreateNode();
                node = new PowerASTNode(node,right);
            }
            return node;
        }
        //=
        private ASTnode AssignationNode() 
        {
          ASTnode node = AssingWithValue();
            while (Match(Token_Type.ASSIGN) && Previous().Value == "=")
            {
                Token assign= Previous();
                ASTnode right = CreateNode();
                if (node is not IdentifierASTNode id) throw new Exception("No tenemos variable");
                GameContext.InputKeyAssign(id,right);
                node = new AssignASTNode(node,right);
            }
            return node;
        }
        // -= ,+=,*=,/=,%=
        private ASTnode AssingWithValue()
        {
            ASTnode node = ColonNode();
            while (CheckValue("-=") || CheckValue("+=") || CheckValue("*=") || CheckValue("/=") || CheckValue("%="))
            {
                string asigwithvalue = Peek().Value;
                Advance();
                ASTnode rigth = CreateNode();
                node = new AssgnWithValueASTNode(node, asigwithvalue, rigth);
            }
            return node;
        }
        // :
        private ASTnode ColonNode() 
        {
            ASTnode node = AccessNode();
            while (Match(Token_Type.COLON)) 
            {
                Token col = Previous();
                ASTnode right = CreateNode();
                node = new ColonASTNode(node,right);
            }
            return node;
        }  
        //.
        private ASTnode AccessNode() 
        {
            ASTnode node = LambdaASTnode();
            while (Match(Token_Type.ACCESS))
            {
                Token access = Previous();
                ASTnode right = LambdaASTnode();
                node = new AccessASTNode(node,right);           
            }
            return node;
        }
        private ASTnode LambdaASTnode() 
        {
            ASTnode node = UnaryNode();
            while (Match(Token_Type.LAMBDA)) 
            {
                Token lamb = Previous();
                ASTnode right = CreateNode();
                node = new LambdaASTNode(node,right);
            }
            return node;    
        }
        //! , ++, --
        private ASTnode UnaryNode() 
        {
            while (Match(Token_Type.NOT)) 
            {
                Token not= Previous();
                ASTnode node = CreateNode();
                return new UnaryASTNode(not.Value,node);
            }
            ASTnode nod = LiteralNode();
            while (Match(Token_Type.UNARY)) 
            {
               Token unary = Previous();    
               nod = new UnaryASTNode(unary.Value,nod);
            }
            return nod;
        }
        //string ,number,boolean
        private ASTnode LiteralNode() 
        {
            while (Match(Token_Type.NUMBER,Token_Type.BOOLEAN,Token_Type.STRING))
            {
                return new LiteralASTNode(Previous().Type, Previous().Value);
            }
            while (Match(Token_Type.IDENTIFIER)) 
            {
                IdentifierASTNode ident =  new IdentifierASTNode(Previous().Type, Previous().Value);
                if (Peek().Type == Token_Type.LEFT_PAREN) { ident.Parameters = CreateNode();}
                return ident;
            }
            if (Match(Token_Type.LEFT_PAREN))
            {
                if (CheckValue(")")) { Advance(); return null; }
                ASTnode insidetheparent = CreateNode();
                Consume(Token_Type.RIGHT_PAREN,$"Se esperaba un parentisis derecho y tenemos {Peek().Value}");
                Console.WriteLine(Peek());
                return new GroupingASTNode(insidetheparent);    
            }
            Console.WriteLine(Peek());
            return PrimaryNode();
        }

        //keyweords que no son ,ni effect , ni card
        public ASTnode PrimaryNode() 
        {
            if (Match(Token_Type.KEYWORD))
            {
                Token node = Previous();
                 if (node.Value == "while") { return WhileNode(); }
                else if (node.Value == "for") { return ForNode(); }
            }
            if (Match(Token_Type.COMMA))
            {
                return new CommaASTNode();
            }
            if (Match(Token_Type.SEMICOLON)) 
            {
                Console.WriteLine("Se encontro un punto y coma ");
            }
            Console.WriteLine(Peek());
          return KeywordNode();
        }
        public ASTnode WhileNode() 
        {
            Console.WriteLine(Peek());
            ConditionalASTNode condition = new ConditionalASTNode();  
            BlockASTNode block = new BlockASTNode();
           comprobar =   Consume(Token_Type.LEFT_PAREN,$"se esperaba un y tenemos un {Peek().Value}");
            if (comprobar == null) { return null; }
            while (Peek().Type != Token_Type.RIGHT_PAREN) { Console.WriteLine(Peek()); condition.condicion = CreateNode(); Console.WriteLine(Peek()); }
            Advance();  
            Console.WriteLine(Peek());
           comprobar = Consume(Token_Type.DELIMITIER, $"se esperaba un delimitador y tenemos {Peek().Value}");
            if (comprobar == null) { return null; }
            while (Peek().Type != Token_Type.DELIMITIER) { Console.WriteLine(Peek()); block.Block.param.Add(CreateNode()); Console.WriteLine(Peek());if (Check(Token_Type.SEMICOLON)) { Advance();} }
           comprobar = Consume(Token_Type.DELIMITIER, $"se esperaba un delimitador y tenemos un {Peek().Value}");
            if (comprobar == null) { return null; }
            return new WhileASTNode(condition, block);  
        }

        public ASTnode ForNode() 
        { 
            BlockASTNode block = new BlockASTNode();
            IdentifierASTNode colcetion = null;
            if (Peek().Type == Token_Type.IDENTIFIER)
            {
               Advance();
                if (Peek().Type == Token_Type.IDENTIFIER && Peek().Value == "in")
                {
                    Advance();
                    if (Peek().Type == Token_Type.IDENTIFIER)
                    {
                        colcetion = new IdentifierASTNode(Peek().Type,Peek().Value);
                        Advance();
                        Consume(Token_Type.DELIMITIER, $"se esperaba un delimitador pero tenemos un {Peek().Value}");
                        while (Peek().Type != Token_Type.DELIMITIER) 
                        {
                            block.Block.param.Add(CreateNode()); 
                            if(Peek().Type == Token_Type.SEMICOLON) Advance();
                        }
                        Advance();
                        Consume(Token_Type.SEMICOLON,$"Se esperaba una ; y en su lugar tenemos{Peek()}");
                    }
                    else { throw new Exception($"Se esperaba un identifier y tenemos un {Peek().Value}"); }
                }
                else{ throw new Exception($"Se esperaba un identifier y tenemos un {Peek().Value}");}
            }
            else{throw new Exception($"Se esperaba un identifier y tenemos un {Peek().Value}");}

            return new ForASTNode(block, colcetion);
        }
        #endregion
    }
}