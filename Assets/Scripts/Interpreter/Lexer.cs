using System.ComponentModel;
using Skyrim_Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace Skyrim_Interpreter
{
    class Lexer
    {
        //list for tokens 
        public List<Token> tokens { get; private set; }
        //string a analizar
         string   source;
         int position;

        public Lexer(string source)
        {
            tokens = new List<Token>();
            this.source = source;
            position= 0;
        }

        // let's create our regex
         public List<Token> Tokenizer() 
        {
            // ignore white spaces 
           // source = source.Replace(" ", string.Empty);

            //regex 
            string keywords = @"\b(Effect|card|for|while|if|else|return|Params|Action|effect)\b";
            string indetifiers = @"^[a-zA-Z_][a-zA-Z0-9_]*";
            string lambdaoperator = @"=>";
           // string arithmeticsoperators = @"(?:[+\-*/%])";
            string relational = @"(?:==|<=|>=|>|<|!=)";
            string logicoperations = @"(?: &&|\|\||!)";
            string and = @"&&";
            string or = @"||";
            string not = @"!";
            string booleanValues = @"\b(true|false)\b";
            string assignmentoperations = @"(?:=|\+=|-=|\*=|/=|%=|:)";
            string unaryoperations = @"(?:\+\+|--)";
            string number = @"\d+";
            string floats = @"\d+(\.\d+)?";
            string dilimiter = @"[\(\)\{\}\[\]]";
            string colom = @",";
            string semocolom = @";";
            string acccess = @"\.";
            string strings = @"""((^""\\]|\.)*?)""";
            string singlecomment = @"//.*";
            string multiplecomment = @"/\*.*";
            string colonPattern = @":";
            string leftParentPattern = @"\(";
            string rightParentPattern = @"\)";
            string concatenationPattern = @"@@";
            string plusPattern = @"\+";
            string minusPattern = @"\-";
            string multiplyPattern = @"\*";
            string dividePattern = @"\/";
            string modulusPattern = @"%";
            string powerPattern = @"\^";
            string lessPattern = @"<";
            string lessEqualPattern = @"<=";
            string greaterPattern = @">";
            string greaterEqualPattern = @">=";
            string equalPattern = @"==";
            string notEqualPattern = @"!=";
            string stringPattern = @"""([^""]*)""";


            // dictionary for to know what tokens we have  
            Dictionary<string, Token_Type> WeHave = new Dictionary<string, Token_Type>()
            {
                { keywords, Token_Type.KEYWORD },
                {stringPattern,Token_Type.STRING },
                 {booleanValues,Token_Type.BOOLEAN },
                { indetifiers , Token_Type.IDENTIFIER },
                { lambdaoperator , Token_Type.LAMBDA },
                {plusPattern,Token_Type.PLUS },
                { minusPattern,Token_Type.MINUS},
                { multiplyPattern,Token_Type.MULTIPLY},
                { dividePattern,Token_Type.DIVIDE},
                {modulusPattern,Token_Type.MODULUS },
                {powerPattern,Token_Type.POWER},
                //{ arithmeticsoperators , Token_Type.ARITHMETIC },
                {lessPattern,Token_Type.LESS },
                { lessEqualPattern,Token_Type.LESS_EQUAL},
                {greaterPattern,Token_Type.GREATER },
                { greaterEqualPattern,Token_Type.GREATER_EQUAL},
                {equalPattern,Token_Type.EQUAL },
                {notEqualPattern,Token_Type.NOT_EQUAL },
                { relational,Token_Type.RELATION },
                 {not,Token_Type.NOT},
                { logicoperations,Token_Type.LOGIC },
                {colonPattern,Token_Type.COLON},
                { assignmentoperations,Token_Type.ASSIGN },
                { unaryoperations,Token_Type.UNARY  },
                { number,Token_Type.NUMBER },
                { floats,Token_Type.FLOAT},
                {concatenationPattern,Token_Type.CONCAT},
                {leftParentPattern,Token_Type.LEFT_PAREN },
                {rightParentPattern,Token_Type.RIGHT_PAREN },
                { dilimiter,Token_Type.DELIMITIER },
                { colom, Token_Type.COMMA },
                { semocolom,Token_Type.SEMICOLON},
                { acccess,Token_Type.ACCESS },
                { strings , Token_Type.STRING },
                { and,Token_Type.AND }
               
            };

            //we need to scaning the source
            while (position < source.Length ) 
            {
                if (source[position] == ' ') { source = source.Substring(1); continue; }
                string bestmatch = null;
                int bestlength = 0;
                Token_Type besttokentype  = Token_Type.EOF;

                foreach ( var element in WeHave ) 
                {
                    Match match = Regex.Match(source, element.Key);
                    if (match.Success && match.Index == 0 && match.Length > bestlength )
                    {
                        bestmatch = match.Value;
                        besttokentype = element.Value; 
                        bestlength = match.Length;
                    }
                }
                if ( bestmatch == null ) 
                {
                    source = source.Substring(1).Replace(" "," ");
                    continue;
                }
                tokens.Add(new Token(besttokentype, bestmatch));
                source = source.Substring(bestlength).Replace(" "," ");
                if (besttokentype == Token_Type.UNARY && !string.IsNullOrEmpty(source))
                {
                    var next = Regex.Match(source, unaryoperations);
                    if (next.Success && next.Index == 0)
                    {
                        tokens[tokens.Count - 1].Value += next.Value;
                        source = source.Replace(" ", " ");
                    }
                }
            }
            tokens.Add(new Token(Token_Type.EOF,""));
            return tokens;
         }

        //obtener la linea
        private int GetLineNumber(string source, int position)
        {
            int lineNumber = 1;
            int columnNumber = 1;

            for (int i = 0; i < position; i++)
            {
                if (source[i] == '\n')
                {
                    lineNumber++;
                    columnNumber = 1;
                }
                else
                {
                    columnNumber++;
                }
            }

            return lineNumber;
        }


        //obtener la columna
        private int GetColumnNumber(string source, int position)
        {
            int columnNumber = 1;

            for (int i = 0; i < position; i++)
            {
                if (source[i] == '\n')
                {
                    columnNumber = 1;
                }
                else
                {
                    columnNumber++;
                }
            }
            return columnNumber;
        }
    }
}
