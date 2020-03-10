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
            foreach(Node statement in ast)
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
                case NodeType.LOGICAL_AND:
                case NodeType.EQUALITY:
                case NodeType.LESS_THAN:
                case NodeType.ADD:
                case NodeType.MINUS:
                case NodeType.MULTIPLY:
                case NodeType.DIVIDE:
                    // check binary operations (+,-,*,/,&,=,<) are semantically correct
                    CheckBinaryOperations(node);
                    break;
                case NodeType.NOT:
                    // check that the division operation is semantically correct
                    CheckUnaryOperations(node);
                    break;
                default:
                    break; 
            }
        }

        /// <summary>
        /// method <c>CheckInitOperation</c> check that the declaration and initialization of 
        /// variable is semantically correct.
        /// </summary>
        private void CheckInitOperation(Node node)
        {
            ExpressionNode ex = (ExpressionNode)node;
            VariableNode lhs = (VariableNode)ex.GetLhs();
            ExpressionNode rhs = (ExpressionNode)ex.GetRhs();

            string varIdentifier = lhs.GetVariableSymbol();
            string varType = lhs.GetVariableType();

            // variablenode lhs is supposted to be declared/initialized
            // check that variable is not initialized before
            if (symbolTable.IsSymbolInTable(varIdentifier))
            {
                Console.WriteLine("Error... variable already declared in this scope...");
                errorsDetected = true;
                return;
            }

            // if rhs expression is null, then the variable gets default value
            if(rhs == null)
            {
                Symbol s = new Symbol(varIdentifier, lhs.GetVariableType(), "value", symbolTable.GetCurrentScope());
                symbolTable.DeclareSymbol(s);
                return;
            }

            // variable has not been declared before, so let's check that the expression matches the variable type
            string type = GetTypeOfExpression(rhs);
            if(type != null)
            {
                if (type.Equals(varType))
                {
                    Symbol s = new Symbol(varIdentifier, lhs.GetVariableType(), "value", symbolTable.GetCurrentScope());
                    symbolTable.DeclareSymbol(s);
                }
                else
                {
                    Console.WriteLine("Expression type does not match variable type...");
                    errorsDetected = true;
                }
            }
        }

        private void CheckAssignmentOperation(Node node)
        {

        }

        private void CheckForLoopOperation(Node node)
        {

        }

        private void CheckFunctionOperation(Node node)
        {

        }

        private void CheckBinaryOperations(Node node)
        {

        }

        private void CheckUnaryOperations(Node node)
        {

        }

        /// <summary>
        /// method <c>GetTypeOfExpression</c> evaluates the expression and returns the type of the return value.
        /// If the expression contains errors, then null is returned. Any error that is discovered, will be 
        /// printed to user.
        /// </summary>
        private string GetTypeOfExpression(ExpressionNode node)
        {
            Node lhs = node.GetLhs();               // get left hand side of expression
            Node rhs = node.GetRhs();               // get right hand side of expression
            NodeType operation = node.CheckType();  // return type of node (operator)
            string lhsType = null;
            string rhsType = null;

            switch (lhs.CheckType())
            {
                case NodeType.INTEGER:
                    lhsType = "int";
                    break;
                case NodeType.STRING:
                    lhsType = "string";
                    break;
                case NodeType.VARIABLE:
                // check that in symbol table
                default:
                    break;
            }

            switch (rhs.CheckType())
            {
                case NodeType.INTEGER:
                    lhsType = "int";
                    break;
                case NodeType.STRING:
                    lhsType = "string";
                    break;
                case NodeType.VARIABLE:
                // check that in symbol table
                default:
                    break;
            }

            switch (operation)
            {
                case NodeType.LESS_THAN:
                case NodeType.LOGICAL_AND:
                case NodeType.EQUALITY:
                    // lhs and rhs must be booleans
                    if (lhsType == "bool" && rhsType == "bool") return "bool";
                    Console.WriteLine("Error...Type mismatch in expression");
                    errorsDetected = true;
                    return null;
                case NodeType.MINUS:
                case NodeType.DIVIDE:
                case NodeType.MULTIPLY:
                    // lhs and rhs must be integers
                    if (lhsType == "int" && rhsType == "int") return "int";
                    Console.WriteLine("Error...Type mismatch in expression");
                    errorsDetected = true;
                    return null;
                case NodeType.ADD:
                    // lhs and rhs must be integers or strings
                    if (lhsType == "int" && rhsType == "int") return "int";
                    if (lhsType == "string" && rhsType == "string") return "string";
                    Console.WriteLine("Error...Type mismatch in expression at row " + node.GetRow() + " and col " + node.GetCol());
                    errorsDetected = true;
                    return null;
                default:
                    break;
            }
            
            return null;
        }
    }
}
