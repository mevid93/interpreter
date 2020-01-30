
namespace Interpreter
{

    /// <summary>
    /// enum <c>TokenType</c> which tells the type of token.
    /// </summary>
    public enum TokenType
    {
        ADD,
        AND,
        ASSIGNMENT,
        CLOSE_PARENTHIS,
        DIVIDE,
        EQUALS,
        ERROR,
        IDENTIFIER,
        KEYWORD_ASSERT,
        KEYWORD_DO,
        KEYWORD_FOR,
        KEYWORD_END,
        KEYWORD_IN,
        KEYWORD_PRINT,
        KEYWORD_READ,
        KEYWORD_VAR,
        LESS_THAN,
        MINUS,
        MULTIPLY,
        NOT,
        OPEN_PARENTHIS,
        RANGE,
        STATEMENT_END,
        SEPARATOR,
        TYPE_BOOL,
        TYPE_INT,
        TYPE_STRING,
        VAL_BOOL,
        VAL_INTEGER,
        VAL_STRING
    }

    /// <summary>
    /// class <c>Token</c> that represents single scanned token.
    /// </summary>
    class Token
    {
        private TokenType type; // type of token
        private string value;   // value of token

        /// <summary>
        /// constructor <c>Token</c> for creating Token object.
        /// </summary>
        public Token(string tokenValue, TokenType tokenType)
        {
            type = tokenType;
            value = tokenValue;
        }

        /// <summary>
        /// method <c>GetTokenType</c> returns type of token.
        /// </summary>
        /// <returns>token type</returns>
        public TokenType GetTokenType()
        {
            return type;
        }

        /// <summary>
        /// method <c>GetTokenValue</c> returns value of token.
        /// </summary>
        /// <returns>token value</returns>
        public string GetTokenValue()
        {
            return value;
        }

        /// <summary>
        /// method <c>FindTokenType</c> returns type of the token for given string.
        /// if string is not a token, then ERROR is returned.
        /// </summary>
        public static TokenType FindTokenType(string value)
        {
            switch (value) {
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
            }

            // Failed...
            return TokenType.ERROR;
        }

    }
}
