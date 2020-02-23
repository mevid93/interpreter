using System.Linq;
using System;

namespace Interpreter
{
    /// <summary>
    /// class <c>Parser</c> performs the syntax analysis for source code.
    /// Parser also constructs the AST.
    /// TOP-DOWN parsing by using LL(1).
    /// </summary>
    class Parser
    {

        private Scanner scanner;    // scanner object
        private Token inputToken;   // current token in input

        /// <summary>
        /// constructor <c>Parser</c> creates new Parser-object.
        /// </summary>
        /// <param name="tokenScanner">scanner-object</param>
        public Parser(Scanner tokenScanner)
        {
            scanner = tokenScanner;
        }

        /// <summary>
        /// method <c>Parse</c> starts syntax analysis for scanned content.
        /// Returns the AST if parsing was succesfull. If erros were encountered,
        /// then null is returned.
        /// </summary>
        public void Parse()
        {
            ProcedureProgram();
        }

        /// <summary>
        /// method <c>HandleError</c> handles error situtations. If errors
        /// are encountered, then the rest of the statement is skipped and the parses
        /// continues from the next statement. If EOF, then the parser stops.
        /// </summary>
        private void HandleError()
        {
            // define different error types
            string defaultError = $"SyntaxError::Row {inputToken.GetRow()}::Column {inputToken.GetColumn()}::Invalid syntax!";
            string eofError = $"SyntaxError::Row {inputToken.GetRow()}::Column {inputToken.GetColumn()}::Unexpected end of file!";
            // print error to user
            if (inputToken.GetTokenType() == TokenType.ERROR) Console.WriteLine(inputToken.GetTokenValue());
            else if (inputToken.GetTokenType() == TokenType.EOF) Console.WriteLine(eofError);
            else Console.WriteLine(defaultError);
            // try to move forward and get the next end of statement token
            inputToken = scanner.ScanNextToken();
        }

        /// <summary>
        /// method <c>Match</c> consumes token from input stream if it matches the expected.
        /// </summary>
        private void Match(TokenType expected)
        {
            if (inputToken.GetTokenType() == expected) inputToken = scanner.ScanNextToken();
            else HandleError();
        }

        /// <summary>
        /// method <c>ProcedureProgram</c> is the statring routine for the parsing.
        /// </summary>
        private void ProcedureProgram()
        {
            inputToken = scanner.ScanNextToken();
            TokenType[] validTokenTypes = {
                TokenType.KEYWORD_VAR,
                TokenType.KEYWORD_PRINT,
                TokenType.KEYWORD_ASSERT,
                TokenType.EOF
            };
            if (validTokenTypes.Contains(inputToken.GetTokenType()))
            {
                ProcedureStatementList();
                Match(TokenType.EOF);
            }
            else
            {
                HandleError();
            }
        }

        /// <summary>
        /// method <c>ProcedureStatementList</c> handles the statementlist processing.
        /// </summary>
        private void ProcedureStatementList()
        {
            switch (inputToken.GetTokenType())
            {
                case TokenType.KEYWORD_VAR:
                case TokenType.IDENTIFIER:
                case TokenType.KEYWORD_FOR:
                case TokenType.KEYWORD_READ:
                case TokenType.KEYWORD_PRINT:
                case TokenType.KEYWORD_ASSERT:
                    ProcedureStatement();
                    ProcedureStatementList();
                    break;
                case TokenType.EOF:
                case TokenType.ERROR:
                case TokenType.KEYWORD_END:
                    return;
                default:
                    HandleError();
                    break;
            }
        }

        /// <summary>
        /// method <c>ProcedureStatement</c> handles the statement processing.
        /// </summary>
        private void ProcedureStatement()
        {
            switch (inputToken.GetTokenType())
            {
                case TokenType.KEYWORD_VAR:
                    Match(TokenType.KEYWORD_VAR);
                    Match(TokenType.IDENTIFIER);
                    Match(TokenType.SEPARATOR);
                    ProcedureType();
                    if(inputToken.GetTokenType() == TokenType.ASSIGNMENT)
                    {
                        Match(TokenType.ASSIGNMENT);
                        ProcedureExpression();
                    }
                    Match(TokenType.STATEMENT_END);
                    break;
                case TokenType.IDENTIFIER:
                    Match(TokenType.IDENTIFIER);
                    Match(TokenType.ASSIGNMENT);
                    ProcedureExpression();
                    Match(TokenType.STATEMENT_END);
                    break;
                case TokenType.KEYWORD_FOR:
                    Match(TokenType.KEYWORD_FOR);
                    Match(TokenType.IDENTIFIER);
                    Match(TokenType.KEYWORD_IN);
                    ProcedureExpression();
                    Match(TokenType.RANGE);
                    ProcedureExpression();
                    Match(TokenType.KEYWORD_DO);
                    ProcedureStatementList();
                    Match(TokenType.KEYWORD_END);
                    Match(TokenType.KEYWORD_FOR);
                    Match(TokenType.STATEMENT_END);
                    break;
                case TokenType.KEYWORD_READ:
                    Match(TokenType.KEYWORD_READ);
                    Match(TokenType.IDENTIFIER);
                    Match(TokenType.STATEMENT_END);
                    break;
                case TokenType.KEYWORD_PRINT:
                    Match(TokenType.KEYWORD_PRINT);
                    ProcedureExpression();
                    Match(TokenType.STATEMENT_END);
                    break;
                case TokenType.KEYWORD_ASSERT:
                    Match(TokenType.KEYWORD_ASSERT);
                    Match(TokenType.OPEN_PARENTHIS);
                    ProcedureExpression();
                    Match(TokenType.CLOSE_PARENTHIS);
                    Match(TokenType.STATEMENT_END);
                    break;
                default:
                    HandleError();
                    break;
            }
        }

        /// <summary>
        /// method <c>ProcedureType</c> handles the type processing.
        /// </summary>
        private void ProcedureType()
        {
            if (inputToken.GetTokenType() == TokenType.TYPE_INT) Match(TokenType.TYPE_INT);
            else if (inputToken.GetTokenType() == TokenType.TYPE_STRING) Match(TokenType.TYPE_STRING);
            else if (inputToken.GetTokenType() == TokenType.TYPE_BOOL) Match(TokenType.TYPE_BOOL);
            else HandleError();
        }

        /* ###############################################################################################
         * presedence and order or evaluation is explained in the following page:
         * https://docs.microsoft.com/en-us/cpp/c-language/precedence-and-order-of-evaluation?view=vs-2019
         * ###############################################################################################
         */

        /// <summary>
        /// method <c>ProcedureExpression</c> handles the expression processing.
        /// </summary>
        private void ProcedureExpression()
        {
            Console.WriteLine("Expression");
            switch (inputToken.GetTokenType())
            {
                case TokenType.IDENTIFIER:
                case TokenType.VAL_INTEGER:
                case TokenType.VAL_STRING:
                case TokenType.OPEN_PARENTHIS:
                case TokenType.NOT:
                    ProcedureLogicalAnd();
                    ProcedureLogicalAndTail();
                    break;
                default:
                    HandleError();
                    break;
            }
            Console.WriteLine("Exit Expression");
        }

        /// <summary>
        /// method <c>ProcedureLogicalAnd</c> handles the logical AND processing.
        /// </summary>
        private void ProcedureLogicalAnd()
        {
            Console.WriteLine("LogicalAnd");
            switch (inputToken.GetTokenType())
            {
                case TokenType.IDENTIFIER:
                case TokenType.VAL_INTEGER:
                case TokenType.VAL_STRING:
                case TokenType.OPEN_PARENTHIS:
                case TokenType.NOT:
                    ProcedureEquality();
                    ProcedureEqualityTail();
                    break;
                default:
                    HandleError();
                    break;
            }
            Console.WriteLine("Exit LogicalAnd");
        }

        /// <summary>
        /// method <c>ProcedureLogicalAndTail</c> handles the end processing of logical AND.
        /// </summary>
        private void ProcedureLogicalAndTail()
        {
            Console.WriteLine("LogicalAndTail");
            switch (inputToken.GetTokenType())
            {
                case TokenType.AND:
                    Match(TokenType.AND);
                    ProcedureEquality();
                    ProcedureEqualityTail();
                    break;
                case TokenType.VAL_INTEGER:
                case TokenType.VAL_STRING:
                case TokenType.OPEN_PARENTHIS:
                case TokenType.NOT:
                case TokenType.KEYWORD_ASSERT:
                case TokenType.STATEMENT_END:
                case TokenType.RANGE:
                case TokenType.KEYWORD_DO:
                case TokenType.KEYWORD_END:
                case TokenType.CLOSE_PARENTHIS:
                    Console.WriteLine("Exit LogicalTail");
                    return;
                default:
                    HandleError();
                    break;
            }
            Console.WriteLine("Exit LogicalAndTail");
        }

        /// <summary>
        /// method <c>ProcedureEquality</c> handles the processing of equality comparison.
        /// </summary>
        private void ProcedureEquality()
        {
            Console.WriteLine("Equality");
            switch (inputToken.GetTokenType())
            {
                case TokenType.IDENTIFIER:
                case TokenType.VAL_INTEGER:
                case TokenType.VAL_STRING:
                case TokenType.OPEN_PARENTHIS:
                case TokenType.NOT:
                    ProcedureComparison();
                    ProcedureComparisonTail();
                    break;
                default:
                    HandleError();
                    break;
            }
            Console.WriteLine("Exit Equality");
        }

        /// <summary>
        /// method <c>ProduceEqualityTail</c> handles the end processing of equality comparison.
        /// </summary>
        private void ProcedureEqualityTail()
        {
            Console.WriteLine("Equality Tail");
            switch (inputToken.GetTokenType())
            {
                case TokenType.EQUALS:
                    Match(TokenType.EQUALS);
                    ProcedureComparison();
                    ProcedureComparisonTail();
                    break;
                case TokenType.VAL_INTEGER:
                case TokenType.VAL_STRING:
                case TokenType.OPEN_PARENTHIS:
                case TokenType.NOT:
                case TokenType.KEYWORD_ASSERT:
                case TokenType.STATEMENT_END:
                case TokenType.RANGE:
                case TokenType.KEYWORD_DO:
                case TokenType.KEYWORD_END:
                case TokenType.CLOSE_PARENTHIS:
                case TokenType.AND:
                    Console.WriteLine("Exit Equality Tail");
                    return;
                default:
                    HandleError();
                    break;
            }
            Console.WriteLine("Exit Equality Tail");
        }

        /// <summary>
        /// method <c>ProcedureComparison</c> handles the processing of comparison operations (less than).
        /// </summary>
        private void ProcedureComparison()
        {
            Console.WriteLine("Comparison");
            switch (inputToken.GetTokenType())
            {
                case TokenType.IDENTIFIER:
                case TokenType.VAL_INTEGER:
                case TokenType.VAL_STRING:
                case TokenType.OPEN_PARENTHIS:
                case TokenType.NOT:
                    ProcedureTerm();
                    ProcedureTermTail();
                    break;
                default:
                    HandleError();
                    break;
            }
            Console.WriteLine("Exit Comparison");
        }

        /// <summary>
        /// method <c>ProcedureComparisonTail</c> handles the end processing of comparison operations.
        /// </summary>
        private void ProcedureComparisonTail()
        {
            Console.WriteLine("ComparisonTail");
            switch (inputToken.GetTokenType())
            {
                case TokenType.LESS_THAN:
                    Match(TokenType.LESS_THAN);
                    ProcedureTerm();
                    ProcedureTermTail();
                    break;
                case TokenType.CLOSE_PARENTHIS:
                case TokenType.IDENTIFIER:
                case TokenType.KEYWORD_READ:
                case TokenType.KEYWORD_PRINT:
                case TokenType.EOF:
                case TokenType.STATEMENT_END:
                case TokenType.KEYWORD_ASSERT:
                case TokenType.RANGE:
                case TokenType.KEYWORD_DO:
                case TokenType.KEYWORD_END:
                case TokenType.EQUALS:
                case TokenType.AND:
                    Console.WriteLine("Exit ComparisonTail");
                    return;
                default:
                    HandleError();
                    break;
            }
            Console.WriteLine("Exit ComparisonTail");
        }

        /// <summary>
        /// method <c>ProduceTerm</c> handles the processing of addivite (+, -) operations.
        /// </summary>
        private void ProcedureTerm()
        {
            Console.WriteLine("Term");
            switch (inputToken.GetTokenType())
            {
                case TokenType.IDENTIFIER:
                case TokenType.VAL_INTEGER:
                case TokenType.VAL_STRING:
                case TokenType.OPEN_PARENTHIS:
                case TokenType.NOT:
                    ProcedureFactor();
                    ProcedureFactorTail();
                    break;
                default:
                    HandleError();
                    break;
            }
            Console.WriteLine("Exit Term");
        }

        /// <summary>
        /// method <c>ProduceTermTail</c> handles the end processing of additive (+, -) operations.
        /// </summary>
        private void ProcedureTermTail()
        {
            Console.WriteLine("TermTail");
            switch (inputToken.GetTokenType())
            {
                case TokenType.ADD:
                    Match(TokenType.ADD);
                    ProcedureFactor();
                    ProcedureFactorTail();
                    break;
                case TokenType.MINUS:
                    Match(TokenType.MINUS);
                    ProcedureFactor();
                    ProcedureFactorTail();
                    break;
                case TokenType.CLOSE_PARENTHIS:
                case TokenType.IDENTIFIER:
                case TokenType.KEYWORD_READ:
                case TokenType.KEYWORD_PRINT:
                case TokenType.EOF:
                case TokenType.STATEMENT_END:
                case TokenType.KEYWORD_ASSERT:
                case TokenType.RANGE:
                case TokenType.KEYWORD_DO:
                case TokenType.KEYWORD_END:
                case TokenType.EQUALS:
                case TokenType.LESS_THAN:
                case TokenType.AND:
                    Console.WriteLine("Exit TermTail");
                    return;
                default:
                    HandleError();
                    break;
            }
            Console.WriteLine("Exit TermTail");
        }

        /// <summary>
        /// method <c>ProcedureFactor</c> handles the processing of multiplicative (*, /) operations.
        /// </summary>
        private void ProcedureFactor()
        {
            Console.WriteLine("Factor");
            switch (inputToken.GetTokenType())
            {
                case TokenType.IDENTIFIER:
                case TokenType.VAL_INTEGER:
                case TokenType.VAL_STRING:
                case TokenType.OPEN_PARENTHIS:
                case TokenType.NOT:
                    ProcedureUnary();
                    ProcedureUnaryTail();
                    break;
                default:
                    HandleError();
                    break;
            }
            Console.WriteLine("Exit Factor");
        }

        /// <summary>
        /// method <c>ProcedureFactorTail</c> handles the end processing of multiplicative operations.
        /// </summary>
        private void ProcedureFactorTail()
        {
            Console.WriteLine("FactorTail");
            switch (inputToken.GetTokenType())
            {
                case TokenType.DIVIDE:
                case TokenType.MULTIPLY:
                    Match(TokenType.MULTIPLY);
                    ProcedureFactor();
                    ProcedureFactorTail();
                    break;
                case TokenType.ADD:
                case TokenType.MINUS:
                case TokenType.IDENTIFIER:
                case TokenType.KEYWORD_READ:
                case TokenType.KEYWORD_PRINT:
                case TokenType.EOF:
                case TokenType.CLOSE_PARENTHIS:
                case TokenType.STATEMENT_END:
                case TokenType.KEYWORD_ASSERT:
                case TokenType.RANGE:
                case TokenType.KEYWORD_DO:
                case TokenType.KEYWORD_END:
                case TokenType.EQUALS:
                case TokenType.LESS_THAN:
                case TokenType.AND:
                    Console.WriteLine("Exit FactorTail");
                    return;
                default:
                    HandleError();
                    break;
            }
            Console.WriteLine("Exit FactorTail");
        }

        /// <summary>
        /// method <c>ProcedureUnary</c> handles the processing of unary opearations.
        /// </summary>
        private void ProcedureUnary()
        {
            Console.WriteLine("Unary");
            switch (inputToken.GetTokenType())
            {
                case TokenType.IDENTIFIER:
                case TokenType.VAL_INTEGER:
                case TokenType.VAL_STRING:
                case TokenType.OPEN_PARENTHIS:
                    ProcedurePrimary();
                    break;
                case TokenType.NOT:
                    Match(TokenType.NOT);
                    break;
                default:
                    HandleError();
                    break;
            }
            Console.WriteLine("Exit Unary");
        }

        /// <summary>
        /// method <c>ProcedureUnaryTail</c> handles the end processing of unary operations.
        /// </summary>
        private void ProcedureUnaryTail()
        {
            Console.WriteLine("UnaryTail");
            switch (inputToken.GetTokenType())
            {
                case TokenType.IDENTIFIER:
                case TokenType.VAL_INTEGER:
                case TokenType.VAL_STRING:
                case TokenType.OPEN_PARENTHIS:
                case TokenType.NOT:
                    ProcedureUnary();
                    ProcedureUnaryTail();
                    break;
                case TokenType.ADD:
                case TokenType.MINUS:
                case TokenType.CLOSE_PARENTHIS:
                case TokenType.MULTIPLY:
                case TokenType.DIVIDE:
                case TokenType.LESS_THAN:
                case TokenType.EOF:
                case TokenType.STATEMENT_END:
                case TokenType.KEYWORD_ASSERT:
                case TokenType.RANGE:
                case TokenType.KEYWORD_DO:
                case TokenType.KEYWORD_END:
                case TokenType.EQUALS:
                case TokenType.AND:
                    Console.WriteLine("Exit UnaryTail");
                    return;
                default:
                    HandleError();
                    break;
            }
            Console.WriteLine("Exit UnaryTail");
        }

        /// <summary>
        /// method <c>ProcedurePrimary</c> handles the preccing of primary elements.
        /// </summary>
        public void ProcedurePrimary()
        {
            Console.WriteLine("Primary");
            switch (inputToken.GetTokenType())
            {
                case TokenType.IDENTIFIER:
                    Match(TokenType.IDENTIFIER);
                    break;
                case TokenType.VAL_INTEGER:
                    Match(TokenType.VAL_INTEGER);
                    break;
                case TokenType.VAL_STRING:
                    Match(TokenType.VAL_STRING);
                    break;
                case TokenType.OPEN_PARENTHIS:
                    Match(TokenType.OPEN_PARENTHIS);
                    ProcedureExpression();
                    Match(TokenType.CLOSE_PARENTHIS);
                    break;
                default:
                    HandleError();
                    break;
            }
            Console.WriteLine("Exit Primary");
        }
    }
}
