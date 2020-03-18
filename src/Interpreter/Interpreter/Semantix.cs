using System;
using System.Collections.Generic;

namespace Interpreter
{
    /// <summary>
    /// Class <c>Semantix</c> holds functionality to do semantic analysis for
    /// intermediate representation of source code. In other words, it takes
    /// AST as input, checks semantic constraints and reports any errors it finds.
    /// </summary>
    class Semantix
    {
        private readonly List<INode> ast;       // AST representation of source code
        private bool errorsDetected;            // flag telling about status of semantic analysis
        private SymbolTable symbolTable;        // stack like scoped scoped symbol table    

        /// <summary>
        /// Constructor <c>Semantix</c> creates new Semantix-object.
        /// </summary>
        /// <param name="ast">AST</param>
        public Semantix(List<INode> ast)
        {
            this.ast = ast;
            symbolTable = new SymbolTable();
        }

        /// <summary>
        /// Method <c>NoErrorsDetected</c> returns the result of semantic analysis.
        /// </summary>
        /// <returns>true if no errors were detected</returns>
        public bool NoErrorsDetected()
        {
            return !errorsDetected;
        }

        /// <summary>
        /// Method <c>CheckConstraints</c> checks the semantic constraints of source code.
        /// </summary>
        public void CheckConstraints()
        {
            foreach (INode statement in ast)
            {
                CheckStatement(statement);
            }
        }

        /// <summary>
        /// Method <c>CheckStatement</c> performs semantical analysis for a single statement.
        /// </summary>
        /// <param name="node">statement node</param>
        private void CheckStatement(INode node)
        {
            switch (node.GetNodeType())
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
        /// Method <c>CheckInitOperation</c> checks that the initialization of variable is semantically correct.
        /// </summary>
        private void CheckInitOperation(INode node)
        {
            ExpressionNode ex = (ExpressionNode)node;
            VariableNode lhs = (VariableNode)ex.GetLhs();
            INode rhs = ex.GetRhs();

            string varIdentifier = lhs.GetVariableSymbol();
            string varType = lhs.GetVariableType();

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
            string type = GetEvaluatedType(rhs, symbolTable, ref errorsDetected);
            if (type != null)
            {
                if (type.Equals(varType))
                {
                    Symbol s = new Symbol(varIdentifier, lhs.GetVariableType(), "value", symbolTable.GetCurrentScope());
                    symbolTable.DeclareSymbol(s);
                }
                else
                {
                    Console.WriteLine($"SemanticError::Row {rhs.GetRow()}::Column {rhs.GetCol()}::Cannot implicitly convert {type} to {varType}!");
                    errorsDetected = true;
                }
            }
        }

        /// <summary>
        /// Method <c>CheckAssignmentOperation</c> checks that the assignment operation is semantically correct. 
        /// </summary>
        private void CheckAssignmentOperation(INode node)
        {
            ExpressionNode ex = (ExpressionNode)node;
            VariableNode lhs = (VariableNode)ex.GetLhs();
            INode rhs = ex.GetRhs();

            string varIdentifier = lhs.GetVariableSymbol();

            // check that variable is initialized before
            if (!symbolTable.IsSymbolInTable(varIdentifier))
            {
                Console.WriteLine($"SemanticError::Row {lhs.GetRow()}::Column {lhs.GetCol()}::Variable {varIdentifier} not declared in this scope!");
                errorsDetected = true;
                return;
            }

            // check that expression type matches with variable type
            string type = GetEvaluatedType(rhs, symbolTable, ref errorsDetected);
            string varType = symbolTable.GetSymbolByIdentifier(varIdentifier).GetSymbolType();
            if (type != null && !type.Equals(varType))
            {
                Console.WriteLine($"SemanticError::Row {rhs.GetRow()}::Column {rhs.GetCol()}::Cannot implicitly convert {type} to {varType}!");
                errorsDetected = true;
            }
        }

        /// <summary>
        /// Method <c>CheckAssignmentOperation</c> checks that the for loop is semantically correct. 
        /// </summary>
        private void CheckForLoopOperation(INode node)
        {
            ForloopNode forNode = (ForloopNode)node;
            VariableNode varNode = (VariableNode)forNode.GetVariable();
            INode start = forNode.GetStart();
            INode end = forNode.GetEnd();

            string varIdentifier = varNode.GetVariableSymbol();

            // check that variable is initialized before
            if (!symbolTable.IsSymbolInTable(varIdentifier))
            {
                Console.WriteLine($"SemanticError::Row {varNode.GetRow()}::Column {varNode.GetCol()}::Variable {varIdentifier} not declared in this scope!");
                errorsDetected = true;
                return;
            }

            // check that start node and end node types are int
            string typeStart = GetEvaluatedType(start, symbolTable, ref errorsDetected);
            string typeEnd = GetEvaluatedType(end, symbolTable, ref errorsDetected);
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
            foreach (INode statement in forNode.GetStatements())
            {
                CheckStatement(statement);
            }

            // scope changes when exiting for loop
            symbolTable.RemoveScope();
        }

        /// <summary>
        /// Method <c>CheckFunctionOperation</c> check that function call is semantically correct.
        /// </summary>
        private void CheckFunctionOperation(INode node)
        {
            FunctionNode funcNode = (FunctionNode)node;
            INode param = funcNode.GetParameter();

            if (funcNode.GetFunctionName().Equals("read"))
            {
                // check that the argument is variable and it is declared before
                if (param.GetNodeType() == NodeType.VARIABLE)
                {
                    VariableNode varNode = (VariableNode)param;
                    if(varNode.GetVariableSymbol() != null && !varNode.GetVariableSymbol().Equals(""))
                    {
                        GetEvaluatedType(param, symbolTable, ref errorsDetected);
                        return;
                    }
                }
                Console.WriteLine($"SemanticError::Row { param.GetRow()}::Column { param.GetCol()}::Argument is not variable!");
            }
            else if (funcNode.GetFunctionName().Equals("print"))
            {
                // enough to check that the argument has a type
                GetEvaluatedType(param, symbolTable, ref errorsDetected);
            }
            else if (funcNode.GetFunctionName().Equals("assert"))
            {
                // check that the argument type is bool
                string type = GetEvaluatedType(param, symbolTable, ref errorsDetected);
                if (type != null && !type.Equals("bool"))
                {
                    Console.WriteLine($"SemanticError::Row { param.GetRow()}::Column { param.GetCol()}::Cannot implicitly convert {type} to bool!");
                    errorsDetected = true;
                }
            }
        }

        // CLASS SPECIFIC STATIC METHODS

        /// <summary>
        /// Static method <c>GetTypeOfExpression</c> evaluates the expression and returns the type of the return value.
        /// If the expression contains errors, then null is returned. 
        /// </summary>
        /// <param name="node">Expression node that is going to be evaluated</param>
        /// <param name="symbolTable">symbol table containing all symbols</param>
        /// <param name="errorsDetected">flag that is updated if errors were found</param>
        public static string GetTypeOfExpression(ExpressionNode node, SymbolTable symbolTable, ref bool errorsDetected)
        {
            INode lhs = node.GetLhs();
            INode rhs = node.GetRhs();
            NodeType operation = node.GetNodeType();
            string operationSymbol = node.GetNodeSymbol();
            string lhsType = GetEvaluatedType(lhs, symbolTable, ref errorsDetected);
            string rhsType = GetEvaluatedType(rhs, symbolTable, ref errorsDetected);

            if (lhsType == null || rhsType == null)
            {
                return null;
            }

            string error = $"SemanticError::Row {node.GetRow()}::Column {node.GetCol()}::Operation {operationSymbol} not supported between types {lhsType} and {rhsType}!";

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
        /// Static method <c>GetEvaluatedType</c> returns the evaluated type of single node ("int", "string", "bool").
        /// If errors are detected, returns null.
        /// </summary>
        /// <param name="node">node which type is evaluated</param>
        /// <param name="symbolTable">symbol table containing all symbols</param>
        /// <param name="errorsDetected">flag which value is updated if errors are found</param>
        public static string GetEvaluatedType(INode node, SymbolTable symbolTable, ref bool errorsDetected)
        {
            if (node == null) return null;

            switch (node.GetNodeType())
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
                    errorsDetected = true;
                    return null;
                case NodeType.ADD:
                case NodeType.DIVIDE:
                case NodeType.MINUS:
                case NodeType.MULTIPLY:
                case NodeType.LESS_THAN:
                case NodeType.LOGICAL_AND:
                case NodeType.EQUALITY:
                    ExpressionNode ex = (ExpressionNode)node;
                    return Semantix.GetTypeOfExpression(ex, symbolTable, ref errorsDetected);
                case NodeType.NOT:
                    NotNode not = (NotNode)node;
                    string type = GetEvaluatedType(not.GetChildNode(), symbolTable, ref errorsDetected);
                    if (type.Equals("bool")) return "bool";
                    Console.WriteLine($"SemanticError::Row {not.GetRow()}::Column {not.GetCol()}::Cannot implicitly convert {type} to bool!");
                    return null;
                default:
                    break;
            }
            return null;
        }
    }
}
