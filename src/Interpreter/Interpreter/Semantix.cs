using System;
using System.Collections.Generic;

namespace Interpreter
{
    /// <summary>
    /// class <c>Semnatix</c> holds functionality to do semantic analysis for
    /// intermediate representation of source code. In other words, it takes
    /// AST as input, checks semantic constraints and reports any error it finds.
    /// </summary>
    class Semantix
    {
        private List<Node> ast;                 // AST representation of source code
        private bool errorsDetected;            // flag telling about status of semantic analysis
        private SymbolTable symbolTable;        // stack like scoped scoped symbol table    

        /// <summary>
        /// constructor <c>Semantix</c> creates new Semantix-object.
        /// </summary>
        /// <param name="ast"></param>
        public Semantix(List<Node> ast)
        {
            this.ast = ast;
            errorsDetected = false;
            symbolTable = new SymbolTable();
        }

        /// <summary>
        /// method <c>NoErrorsDetected</c> returns the result of semantic analysis.
        /// If no errors were detected, then it returns true, otherwise false.
        /// </summary>
        /// <returns>true if no errors were detected</returns>
        public bool NoErrorsDetected()
        {
            return !errorsDetected;
        }

        /// <summary>
        /// method <c>CheckConstraints</c> checks the semantic constraints of source code.
        /// </summary>
        public void CheckConstraints()
        {
            foreach (Node statement in ast)
            {
                CheckStatement(statement);
            }
        }

        /// <summary>
        /// method <c>CheckStatement</c> performs semantical analysis for single statement.
        /// </summary>
        /// <param name="node"></param>
        private void CheckStatement(Node node)
        {
            switch (node.CheckType())
            {
                case NodeType.INIT:
                    // check that the initialization operation is semantically correct
                    CheckInitOperation(node);
                    break;
                case NodeType.ASSIGN:
                    // check that the assignment operation is semantically correct
                    CheckAssignmentOperation(node);
                    break;
                case NodeType.FOR_LOOP:
                    // check that the for loop is semantically correct
                    CheckForLoopOperation(node);
                    break;
                case NodeType.FUNCTION:
                    // check that the function call is semantically correct
                    CheckFunctionOperation(node);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// method <c>CheckInitOperation</c> checks that the declaration and initialization of 
        /// variable is semantically correct.
        /// </summary>
        private void CheckInitOperation(Node node)
        {
            ExpressionNode ex = (ExpressionNode)node;
            VariableNode lhs = (VariableNode)ex.GetLhs();
            Node rhs = ex.GetRhs();

            string varIdentifier = lhs.GetVariableSymbol();
            string varType = lhs.GetVariableType();

            // variablenode lhs is supposted to be declared/initialized
            // check that variable is not initialized before
            if (symbolTable.IsSymbolInTable(varIdentifier))
            {
                Console.WriteLine($"SemanticError::Row {lhs.GetRow()}::Column {lhs.GetCol()}::Variable {varIdentifier} already defined in this scope!");
                errorsDetected = true;
                return;
            }

            // if rhs expression is null, then the variable gets default value
            if (rhs == null)
            {
                Symbol s = new Symbol(varIdentifier, lhs.GetVariableType(), "value", symbolTable.GetCurrentScope());
                symbolTable.DeclareSymbol(s);
                return;
            }

            // variable has not been declared before, so let's check that the expression matches the variable type
            string type = GetNodeType(rhs);
            if (type != null)
            {
                if (type.Equals(varType))
                {
                    Symbol s = new Symbol(varIdentifier, lhs.GetVariableType(), "value", symbolTable.GetCurrentScope());
                    symbolTable.DeclareSymbol(s);
                }
                else
                {
                    Console.WriteLine($"SemanticError::Row {lhs.GetRow()}::Column {lhs.GetCol()}::Cannot implicitly convert {type} to {varType}!");
                    errorsDetected = true;
                }
            }
        }

        /// <summary>
        /// method <c>CheckAssignmentOperation</c> checks that the assignment operation is semantically correct. 
        /// </summary>
        private void CheckAssignmentOperation(Node node)
        {
            ExpressionNode ex = (ExpressionNode)node;
            VariableNode lhs = (VariableNode)ex.GetLhs();
            Node rhs = ex.GetRhs();

            string varIdentifier = lhs.GetVariableSymbol();

            // check that variable is initialized before
            if (!symbolTable.IsSymbolInTable(varIdentifier))
            {
                Console.WriteLine($"SemanticError::Row {lhs.GetRow()}::Column {lhs.GetCol()}::Variable {varIdentifier} not declared in this scope!");
                errorsDetected = true;
                return;
            }

            // check that expression type matches with variable type
            string type = GetNodeType(rhs);
            string varType = symbolTable.GetSymbolByIdentifier(varIdentifier).GetSymbolType();
            if (type != null && !type.Equals(varType))
            {
                Console.WriteLine($"SemanticError::Row {lhs.GetRow()}::Column {lhs.GetCol()}::Cannot implicitly convert {type} to {varType}!");
                errorsDetected = true;
            }
        }

        /// <summary>
        /// method <c>CheckAssignmentOperation</c> checks that the for loop is semantically correct. 
        /// </summary>
        private void CheckForLoopOperation(Node node)
        {
            ForloopNode forNode = (ForloopNode)node;
            VariableNode varNode = (VariableNode)forNode.GetVariable();
            Node start = forNode.GetStart();
            Node end = forNode.GetEnd();

            string varIdentifier = varNode.GetVariableSymbol();

            // check that variable is initialized before
            if (!symbolTable.IsSymbolInTable(varIdentifier))
            {
                Console.WriteLine($"SemanticError::Row {varNode.GetRow()}::Column {varNode.GetCol()}::Variable {varIdentifier} not declared in this scope!");
                errorsDetected = true;
                return;
            }

            // check that start node and end node types are int
            string typeStart = GetNodeType(start);
            string typeEnd = GetNodeType(end);
            if (typeStart != null && !typeStart.Equals("int"))
            {
                Console.WriteLine($"SemanticError::Row {start.GetRow()}::Column {start.GetCol()}::Cannot implicitly convert {typeStart} to int!");
                errorsDetected = true;
            }
            if (typeEnd != null && !typeEnd.Equals("int"))
            {
                Console.WriteLine($"SemanticError::Row {end.GetRow()}::Column {end.GetCol()}::Cannot implicitly convert {typeEnd} to int!");
                errorsDetected = true;
            }

            // scope changes when entering for loop
            symbolTable.AddScope();

            // check all statements inside for loop
            foreach (Node statement in forNode.GetStatements())
            {
                CheckStatement(statement);
            }

            // scope changes when exiting for loop
            symbolTable.RemoveScope();
        }

        /// <summary>
        /// method <c>CheckFunctionOperation</c> checkc that function call is semantically correct.
        /// </summary>
        private void CheckFunctionOperation(Node node)
        {
            FunctionNode funcNode = (FunctionNode)node;
            Node param = funcNode.GetParameter();

            // multiple different functions...
            if (funcNode.GetFunctionName().Equals("read"))
            {
                // check that the argument is variable and it is declared before
                if (param.CheckType() == NodeType.VARIABLE)
                {
                    GetNodeType(param);
                    return;
                }
                Console.WriteLine($"SemanticError::Row { funcNode.GetRow()}::Column { funcNode.GetCol()}::Argument is not variable!");
            }
            else if (funcNode.GetFunctionName().Equals("print"))
            {
                // enough to check that the argument has a type
                GetNodeType(param);
            }
            else if (funcNode.GetFunctionName().Equals("assert"))
            {
                // check that the argument type is bool
                string type = GetNodeType(param);
                if (type != null && !type.Equals("bool"))
                {
                    Console.WriteLine($"SemanticError::Row { funcNode.GetRow()}::Column { funcNode.GetCol()}::Cannot implicitly convert {type} to bool!");
                }
            }
        }

        /// <summary>
        /// method <c>GetTypeOfExpression</c> evaluates the expression and returns the type of the return value.
        /// If the expression contains errors, then null is returned. Any error that is discovered, will be 
        /// printed to user.
        /// </summary>
        private string GetTypeOfExpression(ExpressionNode node)
        {
            Node lhs = node.GetLhs();
            Node rhs = node.GetRhs();
            NodeType operation = node.CheckType();
            string operationSymbol = node.GetNodeSymbol();
            string lhsType = GetNodeType(lhs);
            string rhsType = GetNodeType(rhs);

            if (lhsType == null || rhsType == null)
            {
                return null;
            }

            string error = $"SemanticError::Row {lhs.GetRow()}::Column {lhs.GetCol()}::Operation {operationSymbol} not supported between types {lhsType} and {rhsType}!";

            switch (operation)
            {
                case NodeType.LOGICAL_AND:
                    // lhs and rhs must be booleans
                    if (lhsType.Equals("bool") && rhsType.Equals("bool")) return "bool";
                    Console.WriteLine(error);
                    errorsDetected = true;
                    return null;
                case NodeType.EQUALITY:
                case NodeType.LESS_THAN:
                    // lhs and rhs must be integers
                    if (lhsType.Equals("string") && rhsType.Equals("string")) return "bool";
                    if (lhsType.Equals("bool") && rhsType.Equals("bool")) return "bool";
                    if (lhsType.Equals("int") && rhsType.Equals("int")) return "bool";
                    Console.WriteLine(error);
                    errorsDetected = true;
                    return null;
                case NodeType.MINUS:
                case NodeType.DIVIDE:
                case NodeType.MULTIPLY:
                    // lhs and rhs must be integers
                    if (lhsType.Equals("int") && rhsType.Equals("int")) return "int";
                    Console.WriteLine(error);
                    errorsDetected = true;
                    return null;
                case NodeType.ADD:
                    // lhs and rhs must be integers or strings
                    if (lhsType.Equals("int") && rhsType.Equals("int")) return "int";
                    if (lhsType.Equals("string") && rhsType.Equals("string")) return "string";
                    Console.WriteLine(error);
                    errorsDetected = true;
                    return null;
                default:
                    break;
            }

            return null;
        }

        /// <summary>
        /// method <c>GetNodeType</c> returns the type of single node ("int", "string", "bool").
        /// If errors are detected, returns null.
        /// </summary>
        /// <param name="node"></param>
        private string GetNodeType(Node node)
        {
            if (node == null) return null;

            switch (node.CheckType())
            {
                case NodeType.INTEGER:
                    return "int";
                case NodeType.STRING:
                    return "string";
                case NodeType.VARIABLE:
                    VariableNode varNode = (VariableNode)node;
                    if (symbolTable.IsSymbolInTable(varNode.GetVariableSymbol()))
                    {
                        return varNode.GetVariableType();
                    }
                    Console.WriteLine($"SemanticError::Row {node.GetRow()}::Column {node.GetCol()}::Variable {varNode.GetVariableSymbol()} not defined in this scope!");
                    return null;
                case NodeType.ADD:
                case NodeType.DIVIDE:
                case NodeType.MINUS:
                case NodeType.MULTIPLY:
                case NodeType.LESS_THAN:
                case NodeType.LOGICAL_AND:
                case NodeType.EQUALITY:
                    ExpressionNode ex = (ExpressionNode)node;
                    return GetTypeOfExpression(ex);
                case NodeType.NOT:
                    NotNode not = (NotNode)node;
                    string type = GetNodeType(not);
                    if (type.Equals("bool"))
                    {
                        return "bool";
                    }
                    Console.WriteLine($"SemanticError::Row {not.GetRow()}::Column {not.GetCol()}::Cannot implicitly convert {type} to bool!");
                    return null;
                default:
                    break;
            }
            return null;
        }
    }
}
