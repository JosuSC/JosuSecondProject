using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skyrim_Interpreter
{

    //let's to definate the types of Token
    public enum Token_Type 
    {
        // Separators
        LEFT_PAREN,     // Represents the left parenthesis "("
        RIGHT_PAREN,    // Represents the right parenthesis ")"
        SEMICOLON,      // Represents a semicolon ";"
        COMMA,          // Represents a comma ","
        DELIMITIER,      // Represente the general separators 

        // Variables
        IDENTIFIER,     // Represents an identifier (variable name)
        NUMBER,         // Represents a numeric value
        FLOAT,           //Represente a float value
        STRING,         // Represents a string value
        BOOLEAN,        // Represents a boolean value

        KEYWORD,
        ACCESS,

        COLON,   
        // Operators
        PLUS,           // Represents the addition operator "+"
        MINUS,          // Represents the subtraction operator "-"
        MULTIPLY,       // Represents the multiplication operator "*"
        DIVIDE,         // Represents the division operator "/"
        MODULUS,        // Represents the modulus operator "%"
        POWER,          // Represents the exponentiation operator "^"
        AND,            // Represents the logical AND operator "&"
        OR,             // Represents the logical OR operator "|"
        NOT,            // Represents the logical NOT operator "!"
        NOT_EQUAL,      // Represents the inequality operator "!="
        EQUAL,          // Represents the equality operator "=="
        ASSIGN,         // Represents the assignment operator "="
        GREATER,        // Represents the greater than operator ">"
        GREATER_EQUAL,  // Represents the greater than or equal to operator ">="
        LESS,           // Represents the less than operator "<"
        LESS_EQUAL,     // Represents the less than or equal to operator "<="
        CONCAT,         // Represents the string concatenation operator "@"
        LAMBDA,         // Represents the lambda function operator "=>"
        ARITHMETIC,
        RELATION,
        LOGIC,
        UNARY,
        EXPRESSION,
        EMPTY,
        DECLARATION,
        IF,
        WHILE,
        FOR,
        PARAMS,
        // End of File
        EOF
    }

    internal class Token
    {
        public Token_Type Type { get; set; } 
        public string Value { get; set; }
        public int ColumnNumber { get; internal set; }
        public int LineNumber { get; internal set; }

        public Token(Token_Type type, string value)
        {
            this.Type = type;
            this.Value = value;
        }
        public override string ToString()
        {
            return $"{Type} : {Value}";
        }
        public bool IsOperator()
        {
            switch (Type)
            {
                case Token_Type.PLUS:
                case Token_Type.MINUS:
                case Token_Type.MULTIPLY:
                case Token_Type.DIVIDE:
                case Token_Type.MODULUS:
                case Token_Type.POWER:
                case Token_Type.AND:
                case Token_Type.OR:
                case Token_Type.NOT:
                    return true;
                default:
                    return false;
            }
        }

        public bool IsNumeric()
        {
            switch (Type)
            {
                case Token_Type.NUMBER:
                case Token_Type.FLOAT:
                    return true;
                default:
                    return false;
            }
        }

        public double GetNumericValue()
        {
            if (IsNumeric())
            {
                return double.Parse(Value);
            }
            else
            {
                throw new InvalidOperationException("Token is not a numeric value");
            }
        }

    }
}
