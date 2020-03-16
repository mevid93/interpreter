﻿using System;
using System.Collections.Generic;

namespace Interpreter
{
    /// <summary>
    /// Class <c>Parser</c> contains functionality to perform the syntax analysis for source code.
    /// It also constructs the abstract syntax tree (AST).
    /// TOP-DOWN parsing by using LL(1).
    /// </summary>
    class Parser
    {

        private Scanner scanner;        // scanner object
        private Token inputToken;       // current token in input
        private List<Node> statements;  // abstract syntax tree
        private bool errorsDetected;    // flag telling if errors were detected during parsing
        private string lastError;       // last error which was printed --> used to prevent duplicate prints

        /// <summary>
        /// Constructor <c>Parser</c> creates new Parser-object.
        /// </summary>
        /// <param name="tokenScanner">scanner-object</param>
        public Parser(Scanner tokenScanner)
        {
            scanner = tokenScanner;
            statements = new List<Node>();
        }

        /// <summary>
        /// Method <c>NoErrorsDetected</c> returns the result of parsing.
        /// </summary>
        /// <returns>true if no errors were detected during parsing</returns>
        public bool NoErrorsDetected()
        {
            return !errorsDetected;
        }

        /// <summary>
        /// Method <c>Parse</c> starts the syntax analysis and building of AST.
        /// Returns the AST if parsing was succesfull. 
        /// If erros were encountered, then null is returned.
        /// </summary>
        /// <returns>AST</returns>
        public List<Node> Parse()
        {
            ProcedureProgram();
            return statements;
        }

        /// <summary>
        /// Method <c>HandleError</c> handles error situtations. 
        /// When parser encounters errors, then the rest of the statement is skipped and the parser
        /// continues from the next statement. 
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
        /// Method <c>Match</c> consumes token from input stream if it matches the expected.
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
        /// Method <c>ProcedureProgram</c> starts the parsing and adds valid statements to AST.
        /// </summary>
        private void ProcedureProgram()
        {
            inputToken = scanner.ScanNextToken();
            while (inputToken.GetTokenType() != TokenType.EOF)
            {
                Node node = ProcedureStatement();
                if (node != null) statements.Add(node);
            }
        }

        /// <summary>
        /// Method <c>ProcedureStatement</c> handles the statement processing.
        /// </summary>
        private Node ProcedureStatement()
        {
            switch (inputToken.GetTokenType())
            {
                // variable declaration or initialization
                case TokenType.KEYWORD_VAR:
                    Match(TokenType.KEYWORD_VAR);
                    int row = inputToken.GetRow();
                    int col = inputToken.GetColumn();
                    string id = Match(TokenType.IDENTIFIER);
                    int row2 = inputToken.GetRow();
                    int col2 = inputToken.GetColumn();
                    string symbol = Match(TokenType.SEPARATOR);
                    string type = ProcedureType();
                    Node lhs = new VariableNode(row, col, id, type);
                    Node rhs = null;
                    Node node = new ExpressionNode(row2, col2, NodeType.INIT, symbol, lhs, rhs);
                    if (inputToken.GetTokenType() == TokenType.ASSIGNMENT)
                    {
                        row = inputToken.GetRow();
                        col = inputToken.GetColumn();
                        Match(TokenType.ASSIGNMENT);
                        rhs = ProcedureExpression();
                        node = new ExpressionNode(row, col, NodeType.INIT, symbol, lhs, rhs);
                    }
                    Match(TokenType.STATEMENT_END);
                    return node;

                // variable assignment
                case TokenType.IDENTIFIER:
                    row = inputToken.GetRow();
                    col = inputToken.GetColumn();
                    id = Match(TokenType.IDENTIFIER);
                    row2 = inputToken.GetRow();
                    col2 = inputToken.GetColumn();
                    symbol = Match(TokenType.ASSIGNMENT);
                    lhs = new VariableNode(row, col, id, null);
                    rhs = ProcedureExpression();
                    node = new ExpressionNode(row2, col2, NodeType.ASSIGN, symbol, lhs, rhs);
                    Match(TokenType.STATEMENT_END);
                    return node;

                // for loop
                case TokenType.KEYWORD_FOR:
                    row2 = inputToken.GetRow();
                    col2 = inputToken.GetColumn();
                    symbol = Match(TokenType.KEYWORD_FOR);
                    row = inputToken.GetRow();
                    col = inputToken.GetColumn();
                    id = Match(TokenType.IDENTIFIER);
                    Match(TokenType.KEYWORD_IN);
                    Node start = ProcedureExpression();
                    Match(TokenType.RANGE);
                    Node end = ProcedureExpression();
                    Match(TokenType.KEYWORD_DO);
                    Node variable = new VariableNode(row, col, id, null);
                    ForloopNode forNode = new ForloopNode(row2, col2, variable, start, end);
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
                    row2 = inputToken.GetRow();
                    col2 = inputToken.GetColumn();
                    symbol = Match(TokenType.KEYWORD_READ);
                    row = inputToken.GetRow();
                    col = inputToken.GetColumn();
                    id = Match(TokenType.IDENTIFIER);
                    Node parameter = new VariableNode(row, col, id, null);
                    node = new FunctionNode(row2, col2, symbol, parameter);
                    Match(TokenType.STATEMENT_END);
                    return node;

                // printing into console for user
                case TokenType.KEYWORD_PRINT:
                    row = inputToken.GetRow();
                    col = inputToken.GetColumn();
                    symbol = Match(TokenType.KEYWORD_PRINT);
                    parameter = ProcedureExpression();
                    node = new FunctionNode(row, col, symbol, parameter);
                    Match(TokenType.STATEMENT_END);
                    return node;
                
                // assert statement
                case TokenType.KEYWORD_ASSERT:
                    row = inputToken.GetRow();
                    col = inputToken.GetColumn();
                    symbol = Match(TokenType.KEYWORD_ASSERT);
                    Match(TokenType.OPEN_PARENTHIS);
                    parameter = ProcedureExpression();
                    node = new FunctionNode(row, col, symbol, parameter);
                    Match(TokenType.CLOSE_PARENTHIS);
                    Match(TokenType.STATEMENT_END);
                    return node;

                default:
                    HandleError();
                    return null;
            }
        }

        /// <summary>
        /// Method <c>ProcedureType</c> handles the type processing.
        /// Returns the string representation of the type that was processed.
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
        /// Method <c>ProcedureExpression</c> handles the expression processing.
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
        /// Method <c>ProcedureLogicalAnd</c> handles the logical AND processing.
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
        /// Method <c>ProcedureLogicalAndTail</c> handles the end processing of logical AND.
        /// </summary>
        private Node ProcedureLogicalAndTail(Node lhs)
        {
            switch (inputToken.GetTokenType())
            {
                case TokenType.AND:
                    int row = inputToken.GetRow();
                    int col = inputToken.GetColumn();
                    string symbol = Match(TokenType.AND);
                    Node rhs = ProcedureEquality();
                    rhs = ProcedureEqualityTail(rhs);
                    Node node = new ExpressionNode(row, col, NodeType.LOGICAL_AND, symbol, lhs, rhs);
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
        /// Method <c>ProcedureEquality</c> handles the processing of equality comparison.
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
        /// Method <c>ProduceEqualityTail</c> handles the end processing of equality comparison.
        /// </summary>
        private Node ProcedureEqualityTail(Node lhs)
        {
            switch (inputToken.GetTokenType())
            {
                case TokenType.EQUALS:
                    int row = inputToken.GetRow();
                    int col = inputToken.GetColumn();
                    string symbol = Match(TokenType.EQUALS);
                    Node rhs = ProcedureComparison();
                    rhs = ProcedureComparisonTail(rhs);
                    Node node = new ExpressionNode(row, col, NodeType.EQUALITY, symbol, lhs, rhs);
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
        /// Method <c>ProcedureComparison</c> handles the processing of less than comparison.
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
        /// Method <c>ProcedureComparisonTail</c> handles the end processing of less than comparison.
        /// </summary>
        private Node ProcedureComparisonTail(Node lhs)
        {
            switch (inputToken.GetTokenType())
            {
                case TokenType.LESS_THAN:
                    int row = inputToken.GetRow();
                    int col = inputToken.GetColumn();
                    string symbol = Match(TokenType.LESS_THAN);
                    Node rhs = ProcedureTerm();
                    rhs = ProcedureTermTail(rhs);
                    Node node = new ExpressionNode(row, col, NodeType.LESS_THAN, symbol, lhs, rhs);
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
        /// Method <c>ProduceTerm</c> handles the processing of addivite (+, -) operations.
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
        /// Method <c>ProduceTermTail</c> handles the end processing of additive (+, -) operations.
        /// </summary>
        private Node ProcedureTermTail(Node lhs)
        {
            switch (inputToken.GetTokenType())
            {
                case TokenType.ADD:
                    int row = inputToken.GetRow();
                    int col = inputToken.GetColumn();
                    string symbol = Match(TokenType.ADD);
                    Node rhs = ProcedureFactor();
                    rhs = ProcedureFactorTail(rhs);
                    Node node = new ExpressionNode(row, col, NodeType.ADD, symbol, lhs, rhs);
                    return node;
                case TokenType.MINUS:
                    row = inputToken.GetRow();
                    col = inputToken.GetColumn();
                    symbol = Match(TokenType.MINUS);
                    rhs = ProcedureFactor();
                    rhs = ProcedureFactorTail(rhs);
                    node = new ExpressionNode(row, col, NodeType.MINUS, symbol, lhs, rhs);
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
        /// Method <c>ProcedureFactor</c> handles the processing of multiplicative (*, /) operations.
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
        /// Method <c>ProcedureFactorTail</c> handles the end processing of multiplicative operations.
        /// </summary>
        private Node ProcedureFactorTail(Node lhs)
        {
            switch (inputToken.GetTokenType())
            {
                case TokenType.DIVIDE:
                    int row = inputToken.GetRow();
                    int col = inputToken.GetColumn();
                    string symbol = Match(TokenType.DIVIDE);
                    Node rhs = ProcedureFactor();
                    rhs = ProcedureFactorTail(rhs);
                    Node node = new ExpressionNode(row, col, NodeType.DIVIDE, symbol, lhs, rhs);
                    return node;
                case TokenType.MULTIPLY:
                    row = inputToken.GetRow();
                    col = inputToken.GetColumn();
                    symbol = Match(TokenType.MULTIPLY);
                    rhs = ProcedureFactor();
                    rhs = ProcedureFactorTail(rhs);
                    node = new ExpressionNode(row, col, NodeType.MULTIPLY, symbol, lhs, rhs);
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
        /// Method <c>ProcedureUnary</c> handles the processing of unary opearations.
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
        /// Method <c>ProcedureUnaryTail</c> handles the end processing of unary operations.
        /// </summary>
        private Node ProcedureUnaryTail(Node lhs)
        {
            switch (inputToken.GetTokenType())
            {
                case TokenType.NOT:
                    int row = inputToken.GetRow();
                    int col = inputToken.GetColumn();
                    string symbol = Match(TokenType.NOT);
                    Node child = ProcedurePrimary();
                    NotNode node = new NotNode(row, col, child);
                    return node;
                case TokenType.IDENTIFIER:
                case TokenType.VAL_INTEGER:
                case TokenType.VAL_STRING:
                case TokenType.OPEN_PARENTHIS:
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
        /// Method <c>ProcedurePrimary</c> handles the processing of primary elements.
        /// </summary>
        private Node ProcedurePrimary()
        {
            switch (inputToken.GetTokenType())
            {
                case TokenType.IDENTIFIER:
                    int row = inputToken.GetRow();
                    int col = inputToken.GetColumn();
                    string symbol = Match(TokenType.IDENTIFIER);
                    return new VariableNode(row, col, symbol, null);
                case TokenType.VAL_INTEGER:
                    row = inputToken.GetRow();
                    col = inputToken.GetColumn();
                    symbol = Match(TokenType.VAL_INTEGER);
                    return new IntegerNode(row, col, symbol);
                case TokenType.VAL_STRING:
                    row = inputToken.GetRow();
                    col = inputToken.GetColumn();
                    symbol = Match(TokenType.VAL_STRING);
                    return new StringNode(row, col, symbol);
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
