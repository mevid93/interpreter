using System;
using System.Collections.Generic;

namespace Interpreter
{
    /// <summary>
    /// Class <c>Interpreter</c> holds the functionality to execute the AST.
    /// </summary>
    class Interpreter
    {
        private readonly List<INode> ast;       // AST representation of the source code
        private SymbolTable symbolTable;        // stack like scoped scoped symbol table
        private bool errorDetected;             // flag for runtime errors

        /// <summary>
        /// Constructor <c>Interpreter</c> creates Interpreter-object.
        /// </summary>
        /// <param name="ast">abstract syntax tree</param>
        public Interpreter(List<INode> ast)
        {
            this.ast = ast;
            symbolTable = new SymbolTable();
        }

        /// <summary>
        /// Method <c>Execute</c> executes the "statement" nodes in AST.
        /// </summary>
        public void Execute()
        {
            foreach(INode statement in ast)
            {
                if (errorDetected) return;
                ExecuteStatement(statement);
            }
        }

        /// <summary>
        /// Method <c>ExecuteStatement</c> executes individual source code statement.
        /// </summary>
        private void ExecuteStatement(INode node)
        {
            switch (node.GetNodeType())
            {
                case NodeType.INIT:
                    // execute initialization operation
                    ExecuteInitOperation(node);
                    break;
                case NodeType.ASSIGN:
                    // execute assignment operation
                    ExecuteAssignmentOperation(node);
                    break;
                case NodeType.FOR_LOOP:
                    // execute for loop
                    ExecuteForLoopOperation(node);
                    break;
                case NodeType.FUNCTION:
                    // execute function
                    ExecuteFunctionOperation(node);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Method <c>ExecuteInitOperation</c> executes a statement where variable is
        /// declared (and initialized).
        /// </summary>
        private void ExecuteInitOperation(INode node)
        {
            ExpressionNode ex = (ExpressionNode)node;
            VariableNode lhs = (VariableNode)ex.GetLhs();
            INode rhs = ex.GetRhs();

            string varIdentifier = lhs.GetVariableSymbol();
            string varType = lhs.GetVariableType();

            // if rhs expression is null, then the variable gets default value
            string value = null;
            if (rhs == null)
            {
                if (lhs.GetVariableType().Equals("int")) value = "0";
                if (lhs.GetVariableType().Equals("bool")) value = "false";
                Symbol s = new Symbol(varIdentifier, lhs.GetVariableType(), value, symbolTable.GetCurrentScope());
                symbolTable.DeclareSymbol(s);
                return;
            }

            // compute the value of rhs
            value = GetNodeValue(rhs);
            Symbol s1 = new Symbol(varIdentifier, lhs.GetVariableType(), value, symbolTable.GetCurrentScope());
            symbolTable.DeclareSymbol(s1);
        }

        /// <summary>
        /// Method <c>ExecuteAssignmentOperation</c> executes the assignment statement. 
        /// </summary>
        private void ExecuteAssignmentOperation(INode node)
        {
            ExpressionNode ex = (ExpressionNode)node;
            VariableNode lhs = (VariableNode)ex.GetLhs();
            INode rhs = ex.GetRhs();

            string varIdentifier = lhs.GetVariableSymbol();

            // check that expression type matches with variable type
            string value = GetNodeValue(rhs);
            string varType = symbolTable.GetSymbolByIdentifier(varIdentifier).GetSymbolType();
            symbolTable.UpdateSymbol(varIdentifier, value);
        }

        /// <summary>
        /// Method <c>ExecuteForLoopOperation</c> executes for loop expression. 
        /// </summary>
        private void ExecuteForLoopOperation(INode node)
        {
            ForloopNode forNode = (ForloopNode)node;
            VariableNode varNode = (VariableNode)forNode.GetVariable();
            INode start = forNode.GetStart();
            INode end = forNode.GetEnd();

            string varIdentifier = varNode.GetVariableSymbol();

            // scope changes when entering for loop
            symbolTable.AddScope();

            // get start and end values for for-loop
            string startString = GetNodeValue(start);
            string endString = GetNodeValue(end);
            int startValue = Int32.Parse(startString);
            int endValue = Int32.Parse(endString);

            for (int i = startValue; i <= endValue; i++)
            {
                // update variable value in symbol table
                symbolTable.UpdateSymbol(varNode.GetVariableSymbol(), i.ToString());

                // check all statements inside for loop
                foreach (INode statement in forNode.GetStatements())
                {
                    ExecuteStatement(statement);
                }
            }

            // scope changes when exiting for loop
            symbolTable.RemoveScope();
        }

        /// <summary>
        /// Method <c>ExecuteFunctionOperation</c> executes function call statement.
        /// </summary>
        private void ExecuteFunctionOperation(INode node)
        {
            FunctionNode funcNode = (FunctionNode)node;
            INode param = funcNode.GetParameter();

            if (funcNode.GetFunctionName().Equals("read"))
            {
                string inputValue = Console.ReadLine();
                VariableNode varNode = (VariableNode)param;
                string varType = symbolTable.GetSymbolByIdentifier(varNode.GetVariableSymbol()).GetSymbolType();
                if (varType.Equals("int"))
                {
                    try
                    {
                        Int32.Parse(inputValue);
                    }
                    catch
                    {
                        Console.WriteLine($"RuntimeError::Row {param.GetRow()}::Column {param.GetCol()}::Cannot convert input string to int!");
                        errorDetected = true;
                        return;
                    }
                }
                else if (varType.Equals("bool"))
                {
                    // boolean assingment not allowed in Mini-PL
                    Console.WriteLine($"RuntimeError::Row {param.GetRow()}::Column {param.GetCol()}::Cannot convert input string to bool!");
                    errorDetected = true;
                    return;
                }
                symbolTable.UpdateSymbol(varNode.GetVariableSymbol(), inputValue);
            }
            else if (funcNode.GetFunctionName().Equals("print"))
            {
                string value = GetNodeValue(param);
                Console.Write(value);
            }
            else if (funcNode.GetFunctionName().Equals("assert"))
            {
                string value = GetNodeValue(param);
                if (value.Equals("false"))
                {
                    Console.WriteLine("Expected the result to be true. Got false");
                }
            }
        }

        /// <summary>
        /// Method <c>GetTypeOfExpression</c> evaluates the expression and returns the value in string format.
        /// If the expression contains errors, then null is returned. Any error that is discovered, will be 
        /// printed to user.
        /// </summary>
        private string GetValueOfExpression(ExpressionNode node)
        {
            INode lhs = node.GetLhs();
            INode rhs = node.GetRhs();
            NodeType operation = node.GetNodeType();
            string operationSymbol = node.GetNodeSymbol();
            string lhsValue = GetNodeValue(lhs);
            string rhsValue = GetNodeValue(rhs);
            string lhsType = Semantix.GetEvaluatedType(lhs, symbolTable, ref errorDetected);
            string rhsType = Semantix.GetEvaluatedType(rhs, symbolTable, ref errorDetected);

            switch (operation)
            {
                case NodeType.LOGICAL_AND:
                    if (lhsValue.Equals("false") || rhsValue.Equals("false")) return "false";
                    return "true";
                case NodeType.EQUALITY:
                    if (lhsValue.Equals(rhsValue)) return "true";
                    return "false";
                case NodeType.LESS_THAN:
                    if (lhsType.Equals("string") && rhsType.Equals("string"))
                    {
                        if (String.Compare(lhsValue, rhsValue) < 0) return "true";
                        return "false";
                    }
                    if (lhsType.Equals("bool") && rhsType.Equals("bool"))
                    {
                        if (lhsValue.Equals("false") && rhsValue.Equals("true")) return "true";
                        return "false";
                    }
                    if (lhsType.Equals("int") && rhsType.Equals("int"))
                    {
                        int l1 = Int32.Parse(lhsValue);
                        int r1 = Int32.Parse(rhsValue);
                        if (l1 < r1) return "true";
                        return "false";
                    }
                    return null;
                case NodeType.MINUS:
                    int l = Int32.Parse(lhsValue);
                    int r = Int32.Parse(rhsValue);
                    return (l - r) + "";
                case NodeType.DIVIDE:
                    l = Int32.Parse(lhsValue);
                    r = Int32.Parse(rhsValue);
                    return (l / r) + "";
                case NodeType.MULTIPLY:
                    l = Int32.Parse(lhsValue);
                    r = Int32.Parse(rhsValue);
                    return (l * r) + "";
                case NodeType.ADD:
                    if (lhsType.Equals("int") && rhsType.Equals("int"))
                    {
                        l = Int32.Parse(lhsValue);
                        r = Int32.Parse(rhsValue);
                        return (l + r) + "";
                    }
                    if (lhsType.Equals("string") && rhsType.Equals("string"))
                    {
                        return lhsValue + rhsValue;
                    }
                    return null;
                default:
                    break;
            }

            return null;
        }

        /// <summary>
        /// Method <c>GetNodeValue</c> returns the value of single node in string representation.
        /// If errors are detected, returns null.
        /// </summary>
        private string GetNodeValue(INode node)
        {
            if (node == null) return null;

            switch (node.GetNodeType())
            {
                case NodeType.INTEGER:
                    IntegerNode intNode = (IntegerNode)node;
                    return intNode.GetIntegerValue();
                case NodeType.STRING:
                    StringNode stringNode = (StringNode)node;
                    return stringNode.GetStringValue();
                case NodeType.VARIABLE:
                    VariableNode varNode = (VariableNode)node;
                    return symbolTable.GetSymbolByIdentifier(varNode.GetVariableSymbol()).GetCurrentValue();
                case NodeType.ADD:
                case NodeType.DIVIDE:
                case NodeType.MINUS:
                case NodeType.MULTIPLY:
                case NodeType.LESS_THAN:
                case NodeType.LOGICAL_AND:
                case NodeType.EQUALITY:
                    ExpressionNode ex = (ExpressionNode)node;
                    return GetValueOfExpression(ex);
                case NodeType.NOT:
                    NotNode not = (NotNode)node;
                    string value = GetNodeValue(not.GetChildNode());
                    if (value.Equals("true")) return "false";
                    return "true";
                default:
                    break;
            }
            return null;
        }
    }
}
