
namespace Interpreter
{

    /// <summary>
    /// Enum <c>TokenType</c> indicates the type of token.
    /// </summary>
    public enum TokenType
    {
        ADD,                    // + symbol (sum operation for integers and strings)
        AND,                    // & symbol (logical AND operation)
        ASSIGNMENT,             // := symbol (assingment operation)
        CLOSE_PARENTHIS,        // ) symbol 
        DIVIDE,                 // / symbol (division operation for integers)
        EOF,                    // end-of-file indicator
        EQUALS,                 // = symbol (comparison operator between integers, strings and booleans
        ERROR,                  // error token for returning error information for parser
        IDENTIFIER,             // token for identifier symbols in source code
        KEYWORD_ASSERT,         // assert symbol (function keyword)
        KEYWORD_DO,             // do symbol (keyword)
        KEYWORD_FOR,            // for symbol (keyword)
        KEYWORD_END,            // end symbol (keyword)
        KEYWORD_IN,             // in symbol (keyword)
        KEYWORD_PRINT,          // print symbol (keyword)
        KEYWORD_READ,           // read symbol (keyrword)
        KEYWORD_VAR,            // var symbol (keyword)
        LESS_THAN,              // < symbol (less than operation)
        MINUS,                  // - symbol (minus operatio from integers)
        MULTIPLY,               // * symbol (multiplication operation for integers)
        NOT,                    // ! symbol (logical not operation)
        OPEN_PARENTHIS,         // ) symbol
        RANGE,                  // .. symbol (for loop range)
        STATEMENT_END,          // ; symbol
        SEPARATOR,              // : symbol
        TYPE_BOOL,              // bool symbol
        TYPE_INT,               // int symbol
        TYPE_STRING,            // string symbol
        VAL_BOOL,               // THIS IS NOT USED (Mini-PL does not have source symbols for True and False
        VAL_INTEGER,            // token for integer values
        VAL_STRING              // token for string values
    }

    /// <summary>
    /// Class <c>Token</c> represents a token scanned from source code.
    /// </summary>
    class Token
    {
        private readonly TokenType type;    // type of token
        private readonly string value;      // value of token
        private readonly int line;          // line where token exists in source code
        private readonly int column;        // column where token starts in source code

        /// <summary>
        /// Constructor <c>Token</c> creates new Token-object.
        /// <param name="tokenValue">source code symbol</param>
        /// <param name="tokenType">Type of token</param>
        /// <param name="row">row in source code</param>
        /// <param name="col">column in source code</param>
        /// </summary>
        public Token(string tokenValue, TokenType tokenType, int row, int col)
        {
            type = tokenType;
            value = tokenValue;
            line = row;
            column = col;
        }

        /// <summary>
        /// Method <c>GetTokenType</c> returns the type of token.
        /// </summary>
        /// <returns>token type</returns>
        public TokenType GetTokenType()
        {
            return type;
        }

        /// <summary>
        /// Method <c>GetTokenValue</c> returns value of token. 
        /// This is the symbol in source code.
        /// </summary>
        /// <returns>token value</returns>
        public string GetTokenValue()
        {
            return value;
        }

        /// <summary>
        /// Method <c>GetRow</c> returns the row number where token is located in source code.
        /// </summary>
        /// <returns>row number</returns>
        public int GetRow()
        {
            return line;
        }

        /// <summary>
        /// Method <c>GetColumn</c> returns the column number where token is located in source code.
        /// </summary>
        /// <returns>column number</returns>
        public int GetColumn()
        {
            return column;
        }

        /// <summary>
        /// Method <c>ToString</c> returns string representation of Token-object.
        /// </summary>
        /// <returns>token in string format</returns>
        override
        public string ToString()
        {
            return $"{type}, {value}, Row: {line}, Col: {column}";
        }

        /// <summary>
        /// Method <c>FindTokenType</c> returns the type of the token for given string.
        /// If string cannot be interpreted as a valid token, then ERROR token is returned.
        /// </summary>
        public static TokenType FindTokenType(string value)
        {
            switch (value)
            {
                case "(":
                    return TokenType.OPEN_PARENTHIS;
                case ")":
                    return TokenType.CLOSE_PARENTHIS;
                case "+":
                    return TokenType.ADD;
                case "-":
                    return TokenType.MINUS;
                case "*":
                    return TokenType.MULTIPLY;
                case "<":
                    return TokenType.LESS_THAN;
                case "&":
                    return TokenType.AND;
                case "!":
                    return TokenType.NOT;
                case ";":
                    return TokenType.STATEMENT_END;
                case "=":
                    return TokenType.EQUALS;
                case ":=":
                    return TokenType.ASSIGNMENT;
                case "var":
                    return TokenType.KEYWORD_VAR;
                case "for":
                    return TokenType.KEYWORD_FOR;
                case "end":
                    return TokenType.KEYWORD_END;
                case "in":
                    return TokenType.KEYWORD_IN;
                case "do":
                    return TokenType.KEYWORD_DO;
                case "read":
                    return TokenType.KEYWORD_READ;
                case "print":
                    return TokenType.KEYWORD_PRINT;
                case "assert":
                    return TokenType.KEYWORD_ASSERT;
                case "int":
                    return TokenType.TYPE_INT;
                case "string":
                    return TokenType.TYPE_STRING;
                case "bool":
                    return TokenType.TYPE_BOOL;
                default:
                    return TokenType.ERROR;
            }
        }

    }
}
