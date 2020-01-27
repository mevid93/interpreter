
namespace Interpreter
{

    /// <summary>
    /// enum <c>TokenType</c> which tells the type of token.
    /// </summary>
    public enum TokenType
    {
        AND,
        BOOL,
        CLOSE_PARENTHIS,
        DIVIDE,
        EQUALS,
        IDENTIFIER,
        KEYWORD,
        LESS_THAN,
        MINUS,
        MULTIPLY,
        NOT,
        INTEGER,
        OPEN_PARENTHIS,
        STATEMENT_END,
        STRING,
        SEPARATOR,
        RANGE,
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

    }
}
