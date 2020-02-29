using System;
using System.Collections.Generic;

namespace Interpreter
{
    /// <summary>
    /// class <c>Parser</c> performs the syntax analysis for source code.
    /// Parser also constructs the AST.
    /// TOP-DOWN parsing by using LL(1).
    /// </summary>
    class Parser
    {

        private Scanner scanner;        // scanner object
        private Token inputToken;       // current token in input
        private List<Node> statements;  // abstract syntax tree
        private bool errorsDetected;    // flag telling if errors were detected during parsing
        private string lastError;       // last error which was printed --> prevents duplicate prints

        /// <summary>
        /// constructor <c>Parser</c> creates new Parser-object.
        /// </summary>
        /// <param name="tokenScanner">scanner-object</param>
        public Parser(Scanner tokenScanner)
        {
            scanner = tokenScanner;
            statements = new List<Node>();
            errorsDetected = false;
        }

        /// <summary>
        /// method <c>NoErrorsDetected</c> returns the result of parsing.
        /// </summary>
        /// <returns></returns>
        public bool NoErrorsDetected()
        {
            return !errorsDetected;
        }

        /// <summary>
        /// method <c>Parse</c> starts syntax analysis for scanned content.
        /// Returns the AST if parsing was succesfull. If erros were encountered,
        /// then null is returned.
        /// </summary>
        public List<Node> Parse()
        {
            ProcedureProgram();
            return statements;
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
            if (inputToken.GetTokenType() == TokenType.ERROR)
            {
                if (lastError == null || lastError != inputToken.GetTokenValue())
                {
                    Console.WriteLine(inputToken.GetTokenValue());
                    lastError = inputToken.GetTokenValue();
                }
            }
            else if (inputToken.GetTokenType() == TokenType.EOF)
            {
                if(lastError == null || lastError != eofError)
                {
                    Console.WriteLine(eofError);
                    lastError = eofError;
                }
            }
            else
            {
                if(lastError == null || lastError != defaultError)
                {
                    Console.WriteLine(defaultError);
                    lastError = defaultError;
                }
            }
            // try to move to the end of invalid statement (or the end of file)
            inputToken = scanner.ScanNextToken();
            while (inputToken.GetTokenType() != TokenType.STATEMENT_END && inputToken.GetTokenType() != TokenType.EOF)
            {
                inputToken = scanner.ScanNextToken();
            }
            // update status of parsing --> set error flag to true
            errorsDetected = true;
        }

        /// <summary>
        /// method <c>Match</c> consumes token from input stream if it matches the expected.
        /// Returns the matched token value.
        /// </summary>
        private string Match(TokenType expected)
        {
            if (inputToken.GetTokenType() == expected)
            {
                string value = inputToken.GetTokenValue();
                inputToken = scanner.ScanNextToken();
                return value;
            }
            else
            {
                HandleError();
                return null;
            }
        }

        /// <summary>
        /// method <c>ProcedureProgram</c> is the statring routine for the parsing.
        /// </summary>
        private void ProcedureProgram()
        {
            inputToken = scanner.ScanNextToken();
            while (inputToken.GetTokenType() != TokenType.EOF)
            {
                // while not the end of the file --> process statement
                Node node = ProcedureStatement();
                // if valid statement --> add statement to AST
                if (node != null) statements.Add(node);
            }
        }

        /// <summary>
        /// method <c>ProcedureStatement</c> handles the statement processing.
        /// </summary>
        private Node ProcedureStatement()
        {
            switch (inputToken.GetTokenType())
            {
                // variable declaration or initialization
                case TokenType.KEYWORD_VAR:
                    Match(TokenType.KEYWORD_VAR);
                    string id = Match(TokenType.IDENTIFIER);
                    string symbol = Match(TokenType.SEPARATOR);
                    string type = ProcedureType();
                    Node lhs = new VariableNode(id, type);
                    Node rhs = null;
                    Node node = new ExpressionNode(NodeType.INIT, symbol, lhs, rhs);
                    if (inputToken.GetTokenType() == TokenType.ASSIGNMENT)
                    {
                        Match(TokenType.ASSIGNMENT);
                        rhs = ProcedureExpression();
                        node = new ExpressionNode(NodeType.INIT, symbol, lhs, rhs);
                    }
                    Match(TokenType.STATEMENT_END);
                    return node;

                // variable assignment
                case TokenType.IDENTIFIER:
                    id = Match(TokenType.IDENTIFIER);
                    symbol = Match(TokenType.ASSIGNMENT);
                    lhs = new VariableNode(id, null);
                    rhs = ProcedureExpression();
                    node = new ExpressionNode(NodeType.ASSIGN, symbol, lhs, rhs);
                    Match(TokenType.STATEMENT_END);
                    return node;

                // for loop
                case TokenType.KEYWORD_FOR:
                    symbol = Match(TokenType.KEYWORD_FOR);
                    id = Match(TokenType.IDENTIFIER);
                    Match(TokenType.KEYWORD_IN);
                    Node start = ProcedureExpression();
                    Match(TokenType.RANGE);
                    Node end = ProcedureExpression();
                    Match(TokenType.KEYWORD_DO);
                    Node variable = new VariableNode(id, null);
                    ForloopNode forNode = new ForloopNode(symbol, variable, start, end);
                    while(inputToken.GetTokenType() != TokenType.EOF && inputToken.GetTokenType() != TokenType.KEYWORD_END)
                    {
                        Node stmnt = ProcedureStatement();
                        if (stmnt != null) forNode.AddStatement(stmnt);
                    }
                    Match(TokenType.KEYWORD_END);
                    Match(TokenType.KEYWORD_FOR);
                    Match(TokenType.STATEMENT_END);
                    return forNode;
                
                // reading input from user into variable
                case TokenType.KEYWORD_READ:
                    symbol = Match(TokenType.KEYWORD_READ);
                    id = Match(TokenType.IDENTIFIER);
                    Node parameter = new VariableNode(id, null);
                    node = new FunctionNode(symbol, parameter);
                    Match(TokenType.STATEMENT_END);
                    return node;

                // printing into console for user
                case TokenType.KEYWORD_PRINT:
                    symbol = Match(TokenType.KEYWORD_PRINT);
                    parameter = ProcedureExpression();
                    node = new FunctionNode(symbol, parameter);
                    Match(TokenType.STATEMENT_END);
                    return node;
                
                // assert statement
                case TokenType.KEYWORD_ASSERT:
                    symbol = Match(TokenType.KEYWORD_ASSERT);
                    Match(TokenType.OPEN_PARENTHIS);
                    parameter = ProcedureExpression();
                    node = new FunctionNode(symbol, parameter);
                    Match(TokenType.CLOSE_PARENTHIS);
                    Match(TokenType.STATEMENT_END);
                    return node;

                default:
                    HandleError();
                    return null;
            }
        }

        /// <summary>
        /// method <c>ProcedureType</c> handles the type processing.
        /// </summary>
        private string ProcedureType()
        {
            switch (inputToken.GetTokenType())
            {
                case TokenType.TYPE_INT:
                    return Match(TokenType.TYPE_INT);
                case TokenType.TYPE_STRING:
                    return Match(TokenType.TYPE_STRING);
                case TokenType.TYPE_BOOL:
                    return Match(TokenType.TYPE_BOOL);
                default:
                    HandleError();
                    return null;
            }
        }

        /// <summary>
        /// method <c>ProcedureExpression</c> handles the expression processing.
        /// </summary>
        private Node ProcedureExpression()
        {
            switch (inputToken.GetTokenType())
            {
                case TokenType.IDENTIFIER:
                case TokenType.VAL_INTEGER:
                case TokenType.VAL_STRING:
                case TokenType.OPEN_PARENTHIS:
                case TokenType.NOT:
                    Node expression = ProcedureLogicalAnd();
                    expression = ProcedureLogicalAndTail(expression);
                    return expression;
                default:
                    HandleError();
                    return null;
            }
        }

        /// <summary>
        /// method <c>ProcedureLogicalAnd</c> handles the logical AND processing.
        /// </summary>
        private Node ProcedureLogicalAnd()
        {
            switch (inputToken.GetTokenType())
            {
                case TokenType.IDENTIFIER:
                case TokenType.VAL_INTEGER:
                case TokenType.VAL_STRING:
                case TokenType.OPEN_PARENTHIS:
                case TokenType.NOT:
                    Node expression = ProcedureEquality();
                    expression = ProcedureEqualityTail(expression);
                    return expression;
                default:
                    HandleError();
                    return null;
            }
        }

        /// <summary>
        /// method <c>ProcedureLogicalAndTail</c> handles the end processing of logical AND.
        /// </summary>
        private Node ProcedureLogicalAndTail(Node lhs)
        {
            switch (inputToken.GetTokenType())
            {
                case TokenType.AND:
                    string symbol = Match(TokenType.AND);
                    Node rhs = ProcedureEquality();
                    rhs = ProcedureEqualityTail(rhs);
                    Node node = new ExpressionNode(NodeType.LOGICAL_AND, symbol, lhs, rhs);
                    return node;
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
                    return lhs;
                default:
                    HandleError();
                    return null;
            }
        }

        /// <summary>
        /// method <c>ProcedureEquality</c> handles the processing of equality comparison.
        /// </summary>
        private Node ProcedureEquality()
        {
            switch (inputToken.GetTokenType())
            {
                case TokenType.IDENTIFIER:
                case TokenType.VAL_INTEGER:
                case TokenType.VAL_STRING:
                case TokenType.OPEN_PARENTHIS:
                case TokenType.NOT:
                    Node expression = ProcedureComparison();
                    expression = ProcedureComparisonTail(expression);
                    return expression;
                default:
                    HandleError();
                    return null;
            }
        }

        /// <summary>
        /// method <c>ProduceEqualityTail</c> handles the end processing of equality comparison.
        /// </summary>
        private Node ProcedureEqualityTail(Node lhs)
        {
            switch (inputToken.GetTokenType())
            {
                case TokenType.EQUALS:
                    string symbol = Match(TokenType.EQUALS);
                    Node rhs = ProcedureComparison();
                    rhs = ProcedureComparisonTail(rhs);
                    Node node = new ExpressionNode(NodeType.EQUALITY, symbol, lhs, rhs);
                    return node;
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
                    return lhs;
                default:
                    HandleError();
                    return null;
            }
        }

        /// <summary>
        /// method <c>ProcedureComparison</c> handles the processing of comparison operations (less than).
        /// </summary>
        private Node ProcedureComparison()
        {
            switch (inputToken.GetTokenType())
            {
                case TokenType.IDENTIFIER:
                case TokenType.VAL_INTEGER:
                case TokenType.VAL_STRING:
                case TokenType.OPEN_PARENTHIS:
                case TokenType.NOT:
                    Node expression = ProcedureTerm();
                    expression = ProcedureTermTail(expression);
                    return expression;
                default:
                    HandleError();
                    return null;
            }
        }

        /// <summary>
        /// method <c>ProcedureComparisonTail</c> handles the end processing of comparison operations.
        /// </summary>
        private Node ProcedureComparisonTail(Node lhs)
        {
            switch (inputToken.GetTokenType())
            {
                case TokenType.LESS_THAN:
                    string symbol = Match(TokenType.LESS_THAN);
                    Node rhs = ProcedureTerm();
                    rhs = ProcedureTermTail(rhs);
                    Node node = new ExpressionNode(NodeType.LESS_THAN, symbol, lhs, rhs);
                    return node;
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
                    return lhs;
                default:
                    HandleError();
                    return null;
            }
        }

        /// <summary>
        /// method <c>ProduceTerm</c> handles the processing of addivite (+, -) operations.
        /// </summary>
        private Node ProcedureTerm()
        {
            switch (inputToken.GetTokenType())
            {
                case TokenType.IDENTIFIER:
                case TokenType.VAL_INTEGER:
                case TokenType.VAL_STRING:
                case TokenType.OPEN_PARENTHIS:
                case TokenType.NOT:
                    Node expression = ProcedureFactor();
                    expression = ProcedureFactorTail(expression);
                    return expression;
                default:
                    HandleError();
                    return null;
            }
        }

        /// <summary>
        /// method <c>ProduceTermTail</c> handles the end processing of additive (+, -) operations.
        /// </summary>
        private Node ProcedureTermTail(Node lhs)
        {
            switch (inputToken.GetTokenType())
            {
                case TokenType.ADD:
                    string symbol = Match(TokenType.ADD);
                    Node rhs = ProcedureFactor();
                    rhs = ProcedureFactorTail(rhs);
                    Node node = new ExpressionNode(NodeType.ADD, symbol, lhs, rhs);
                    return node;
                case TokenType.MINUS:
                    symbol = Match(TokenType.MINUS);
                    rhs = ProcedureFactor();
                    rhs = ProcedureFactorTail(rhs);
                    node = new ExpressionNode(NodeType.MINUS, symbol, lhs, rhs);
                    return node;
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
                    return lhs;
                default:
                    HandleError();
                    return null;
            }
        }

        /// <summary>
        /// method <c>ProcedureFactor</c> handles the processing of multiplicative (*, /) operations.
        /// </summary>
        private Node ProcedureFactor()
        {
            switch (inputToken.GetTokenType())
            {
                case TokenType.IDENTIFIER:
                case TokenType.VAL_INTEGER:
                case TokenType.VAL_STRING:
                case TokenType.OPEN_PARENTHIS:
                case TokenType.NOT:
                    Node expression = ProcedureUnary();
                    expression = ProcedureUnaryTail(expression);
                    return expression;
                default:
                    HandleError();
                    return null;
            }
        }

        /// <summary>
        /// method <c>ProcedureFactorTail</c> handles the end processing of multiplicative operations.
        /// </summary>
        private Node ProcedureFactorTail(Node lhs)
        {
            switch (inputToken.GetTokenType())
            {
                case TokenType.DIVIDE:
                case TokenType.MULTIPLY:
                    string symbol = Match(TokenType.MULTIPLY);
                    Node rhs = ProcedureFactor();
                    rhs = ProcedureFactorTail(rhs);
                    Node node = new ExpressionNode(NodeType.MULTIPLY, symbol, lhs, rhs);
                    return node;
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
                    return lhs;
                default:
                    HandleError();
                    return null;
            }
        }

        /// <summary>
        /// method <c>ProcedureUnary</c> handles the processing of unary opearations.
        /// </summary>
        private Node ProcedureUnary()
        {
            switch (inputToken.GetTokenType())
            {
                case TokenType.IDENTIFIER:
                case TokenType.VAL_INTEGER:
                case TokenType.VAL_STRING:
                case TokenType.OPEN_PARENTHIS:
                    Node expression = ProcedurePrimary();
                    return expression;
                case TokenType.NOT:
                    return null;
                default:
                    HandleError();
                    return null;
            }
        }

        /// <summary>
        /// method <c>ProcedureUnaryTail</c> handles the end processing of unary operations.
        /// </summary>
        private Node ProcedureUnaryTail(Node lhs)
        {
            switch (inputToken.GetTokenType())
            {
                case TokenType.IDENTIFIER:
                case TokenType.VAL_INTEGER:
                case TokenType.VAL_STRING:
                case TokenType.OPEN_PARENTHIS:
                    Node expression = ProcedureUnary();
                    expression = ProcedureUnaryTail(expression);
                    return expression;
                case TokenType.NOT:
                    string symbol = Match(TokenType.NOT);
                    Node child = ProcedureUnaryTail(lhs);
                    NotNode node = new NotNode(symbol, child);
                    return node;
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
                    return lhs;
                default:
                    HandleError();
                    return null;
            }
        }

        /// <summary>
        /// method <c>ProcedurePrimary</c> handles the processing of primary elements.
        /// </summary>
        private Node ProcedurePrimary()
        {
            switch (inputToken.GetTokenType())
            {
                case TokenType.IDENTIFIER:
                    string symbol = Match(TokenType.IDENTIFIER);
                    return new VariableNode(symbol, null);
                case TokenType.VAL_INTEGER:
                    symbol = Match(TokenType.VAL_INTEGER);
                    return new IntegerNode(symbol);
                case TokenType.VAL_STRING:
                    symbol = Match(TokenType.VAL_STRING);
                    return new StringNode(symbol);
                case TokenType.OPEN_PARENTHIS:
                    Match(TokenType.OPEN_PARENTHIS);
                    Node expression = ProcedureExpression();
                    Match(TokenType.CLOSE_PARENTHIS);
                    return expression;
                default:
                    HandleError();
                    return null;
            }
        }
    }
}
