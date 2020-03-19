using System;
using System.Collections.Generic;

namespace Interpreter
{
    /// <summary>
    /// Enum <c>NodeType</c> represents the type of node.
    /// </summary>
    enum NodeType
    {
        VARIABLE,       // node with variable
        INTEGER,        // node with constant integer
        STRING,         // node with constant string
        ASSIGN,         // node with assignment operation
        INIT,           // node with variable initialization
        FOR_LOOP,       // node that represents for loop,
        FUNCTION,       // node that represents function,
        LOGICAL_AND,    // node that represents logical AND expression
        EQUALITY,       // node that represents equality check
        LESS_THAN,      // node that represents less than comparison
        ADD,            // node that represents add operation
        MINUS,          // node that represents minus operation
        MULTIPLY,       // node that represents multiply operation
        DIVIDE,         // node that represents division operation
        NOT,            // node that represents NOT operator (unary)
        ERROR           // in case of errors... this can be used
    }

    /// <summary>
    /// Interface <c>INode</c> is node interface that defines all the methods that are 
    /// required of different nodes in AST.
    /// </summary>
    interface INode
    {
        /// <summary>
        /// Method <c>GetEvaluatedType</c> returns the type of node.
        /// </summary>
        /// <returns>the type of node</returns>
        NodeType GetNodeType();

        /// <summary>
        /// Method <c>PrettyPrint</c> prints the string representation of node.
        /// </summary>
        void PrettyPrint();

        /// <summary>
        /// Method <c>GetRow</c> returns the row in source code that corresponds the node in AST.
        /// </summary>
        int GetRow();

        /// <summary>
        /// Method <c>GetCol</c> returns the column in source code that corresponds the node in AST.
        /// </summary>
        int GetCol();
    }

    /// <summary>
    /// class <c>VariableNode</c> is variable holding node.
    /// When variables are parsed, variable node should be added to AST.
    /// </summary>
    class VariableNode : INode
    {
        private readonly string variableSymbol;  // symbol of variable
        private readonly string variableType;    // type of variable
        private readonly int row;                // row in source code
        private readonly int col;                // column in source code

        /// <summary>
        /// Constructor <c>VariableNode</c> creates new VariableNode-object.
        /// </summary>
        /// <param name="row">row in source code</param>
        /// <param name="col">col in source code</param>
        /// <param name="variableSymbol">symbol of variable (identifier)</param>
        /// <param name="variableType">type of variable</param>
        public VariableNode(int row, int col, string variableSymbol, string variableType)
        {
            this.variableType = variableType;
            this.variableSymbol = variableSymbol;
            this.row = row;
            this.col = col;
        }

        /// <summary>
        /// Method <c>GetVariableSymbol</c> returns the identifier symbol for variable.
        /// </summary>
        /// <returns>variable symbol (identifier)</returns>
        public string GetVariableSymbol() { return variableSymbol; }

        /// <summary>
        /// Method <c>GetVariableValue</c> returns the value (identifier) of variable.
        /// </summary>
        /// <returns>value of variable</returns>
        public string GetVariableType() { return variableType; }

        public NodeType GetNodeType() { return NodeType.VARIABLE; }

        public void PrettyPrint()
        {
            Console.WriteLine("Nodetype: " + NodeType.VARIABLE + ", symbol: " + variableSymbol + ", variable type: " + variableType);
        }

        public int GetRow() { return row; }

        public int GetCol() { return col; }

        public override string ToString() { return variableSymbol; }
    }

    /// <summary>
    /// Class <c>IntegerNode</c> is node that holds constant integer.
    /// </summary>
    class IntegerNode : INode
    {
        private readonly string integerValue;       // integer value in node
        private readonly int row;                   // row in source code
        private readonly int col;                   // column in source code

        /// <summary>
        /// Constructor <c>IntegerNode</c> creates new IntegerNode-object.
        /// </summary>
        /// <param name="row">row in source code</param>
        /// <param name="col">column in source code</param>
        /// <param name="integerValue">constant integer value</param>
        public IntegerNode(int row, int col, string integerValue)
        {
            this.integerValue = integerValue;
            this.row = row;
            this.col = col;
        }

        /// <summary>
        /// Method <c>GetIntegerValue</c> returns integer (string representation) valud from node.
        /// </summary>
        public string GetIntegerValue() { return integerValue; }

        public NodeType GetNodeType() { return NodeType.INTEGER; }

        public void PrettyPrint()
        {
            Console.WriteLine("Nodetype: " + NodeType.INTEGER + ", value: " + integerValue);
        }

        public int GetRow() { return row; }

        public int GetCol() { return col; }

        public override string ToString() { return integerValue; }
    }

    /// <summary>
    /// Class <c>StringNode</c> is node that holds constant string.
    /// </summary>
    class StringNode : INode
    {
        private readonly string stringValue;    // sting value
        private readonly int row;               // row in source code
        private readonly int col;               // column in source code

        /// <summary>
        /// Constructor <c>StringNode</c> creates new StringNode-object.
        /// </summary>
        /// <param name="row">row in source code</param>
        /// <param name="col">column in source code</param>
        /// <param name="stringValue">string constant</param>
        public StringNode(int row, int col, string stringValue)
        {
            this.stringValue = stringValue;
            this.row = row;
            this.col = col;
        }

        /// <summary>
        /// Method <c>GetStringValue</c> returns string value from node.
        /// </summary>
        /// <returns>string constant</returns>
        public string GetStringValue() { return stringValue; }

        public NodeType GetNodeType() { return NodeType.STRING; }

        public void PrettyPrint()
        {
            Console.WriteLine("Nodetype: " + NodeType.STRING + ", symbol: " + stringValue);
        }

        public int GetRow() { return row; }

        public int GetCol() { return col; }

        public override string ToString() { return stringValue; }
    }

    /// <summary>
    /// class <c>ExpressionNode</c> is node that holds binary expression (two child nodes).
    /// </summary>
    class ExpressionNode : INode
    {
        private readonly NodeType type;         // type of node
        private readonly string nodeSymbol;     // symbol of node (operation)
        private readonly INode lhs;             // left child tree
        private readonly INode rhs;             // right child tree
        private readonly int row;               // row in source code
        private readonly int col;               // column in source code

        /// <summary>
        /// constructor <c>ExpressionNode</c> creates ExpressionNode-object.
        /// </summary>
        /// <param name="row">row in source code</param>
        /// <param name="col">column in source code</param>
        /// <param name="nodeType">type of node</param>
        /// <param name="nodeSymbol">symbol of binary operation</param>
        /// <param name="lhs">left child (lhs of operation)</param>
        /// <param name="rhs">right child (rhs of operation)</param>
        public ExpressionNode(int row, int col, NodeType nodeType, string nodeSymbol, INode lhs, INode rhs)
        {
            this.lhs = lhs;
            this.rhs = rhs;
            type = nodeType;
            this.nodeSymbol = nodeSymbol;
            this.row = row;
            this.col = col;
        }

        /// <summary>
        /// Method <c>GetNodeSymbol</c> returns the (operation) symbol of node.
        /// </summary>
        /// <returns>operation symbol</returns>
        public string GetNodeSymbol() { return nodeSymbol; }

        /// <summary>
        /// Method <c>GetLhs</c> returns left child node.
        /// </summary>
        /// <returns>left child</returns>
        public INode GetLhs() { return lhs; }

        /// <summary>
        /// Method <c>GetRhs</c> returns right child node.
        /// </summary>
        /// <returns>right child</returns>
        public INode GetRhs() { return rhs; }

        public NodeType GetNodeType() { return type; }

        public void PrettyPrint()
        {
            Console.WriteLine("Nodetype: " + type + ", symbol: " + nodeSymbol);
            if (lhs != null)
            {
                Console.Write("LHS: ");
                lhs.PrettyPrint();
            }
            if (rhs != null)
            {
                Console.Write("RHS: ");
                rhs.PrettyPrint();
            }
        }

        public int GetRow() { return row; }

        public int GetCol() { return col; }

        public override string ToString()
        {
            string result = "(";
            if(lhs != null)
            {
                result += lhs.ToString() + " ";
            }
            result += nodeSymbol + " ";
            if(rhs != null)
            {
                result += rhs.ToString();
            }
            result += ")";
            return result;
        }
    }

    /// <summary>
    /// Class <c>NotNode</c> is not for unary not operation.
    /// </summary>
    class NotNode : INode
    {
        private readonly INode child;   // unary operator can only have one child
        private readonly int row;       // row in source code
        private readonly int col;       // column in source code

        /// <summary>
        /// Constructor <c>NotNode</c> creates NotNode-object.
        /// </summary>
        /// <param name="row">row in source code</param>
        /// <param name="col">column in source code</param>
        /// <param name="child">taget expression of not operation</param>
        public NotNode(int row, int col, INode child)
        {
            this.child = child;
            this.row = row;
            this.col = col;
        }

        /// <summary>
        /// Method <c>GetChildNode</c> returns target expression node of not operation.
        /// </summary>
        /// <returns>target expression of not operation</returns>
        public INode GetChildNode() { return child; }

        public NodeType GetNodeType() { return NodeType.NOT; }

        public void PrettyPrint()
        {
            Console.WriteLine("Nodetype: " + NodeType.NOT);
            if (child != null)
            {
                child.PrettyPrint();
            }
        }

        public int GetRow() { return row; }

        public int GetCol() { return col; }

        public override string ToString()
        {
            return "!" + child.ToString();
        }
    }

    /// <summary>
    /// Class <c>ForloopNode</c> is for loop in AST.
    /// </summary>
    class ForloopNode : INode
    {
        private readonly INode variable;        // variable that is assigned values in range from start to end
        private readonly INode start;           // start value
        private readonly INode end;             // end value
        private List<INode> statements;         // list of statements that are executed in each iteration
        private readonly int row;               // row in source code
        private readonly int col;               // column in source code

        /// <summary>
        /// Constructor <c>ForloopNode</c> creates new ForloopNode-object.
        /// </summary>
        /// <param name="row">row in source code</param>
        /// <param name="col">column in source code</param>
        /// <param name="variable">iterator variable</param>
        /// <param name="start">start value for iteration</param>
        /// <param name="end">end value for iteration</param>
        public ForloopNode(int row, int col, INode variable, INode start, INode end)
        {
            this.variable = variable;
            this.start = start;
            this.end = end;
            statements = new List<INode>();
            this.row = row;
            this.col = col;
        }

        /// <summary>
        /// Method <c>AddStatement</c> adds new child (statement) to child nodes.
        /// </summary>
        public void AddStatement(INode node)
        {
            if (node != null) statements.Add(node);
        }

        /// <summary>
        /// Method <c>GetVariable</c> returns the iterator variable.
        /// </summary>
        /// <returns>iterator variable</returns>
        public INode GetVariable() { return variable; }

        /// <summary>
        /// Method <c>GetStart</c> returns the node that evaluates as starting value for range.
        /// </summary>
        /// <returns>start value of iteration</returns>
        public INode GetStart() { return start; }

        /// <summary>
        /// Method <c>GetEnd</c> returns the node that evaluates as end value for range.
        /// </summary>
        /// <returns>ending value of iteration</returns>
        public INode GetEnd() { return end; }

        /// <summary>
        /// Method <c>GetStatements</c> returns all the child nodes (statements).
        /// </summary>
        /// <returns>statements that are executed in for loop</returns>
        public List<INode> GetStatements() { return statements; }

        public NodeType GetNodeType() { return NodeType.FOR_LOOP; }

        public void PrettyPrint()
        {
            Console.WriteLine("Nodetype: " + NodeType.FOR_LOOP);
            if (variable != null)
            {
                variable.PrettyPrint();
            }
            if (start != null)
            {
                Console.Write("START: ");
                start.PrettyPrint();
            }
            if (end != null)
            {
                Console.Write("END: ");
                end.PrettyPrint();
            }
            foreach (INode statement in statements)
            {
                statement.PrettyPrint();
            }
        }

        public int GetRow() { return row; }

        public int GetCol() { return col; }
    }

    /// <summary>
    /// Class <c>FunctionNode</c> represents function in AST.
    /// </summary>
    class FunctionNode : INode
    {
        private readonly string functionSymbol;     // function symbol
        private readonly INode parameter;           // parameter node
        private readonly int row;                   // row in source code
        private readonly int col;                   // column in source code

        /// <summary>
        /// Constructor <c>FunctionNode</c> creates new FunctionNode-object.
        /// </summary>
        /// <param name="row">row in source code</param>
        /// <param name="col">column in source code</param>
        /// <param name="symbol">function symbol (function name)</param>
        /// <param name="parameter">parameter for function call</param>
        public FunctionNode(int row, int col, string symbol, INode parameter)
        {
            this.parameter = parameter;
            functionSymbol = symbol;
            this.row = row;
            this.col = col;
        }

        /// <summary>
        /// Method <c>GetParameter</c> returns node representing function call parameter.
        /// </summary>
        /// <returns>function call parameter</returns>
        public INode GetParameter() { return parameter; }

        /// <summary>
        /// Method <c>GetFunctionName</c> returns the name of the function.
        /// </summary>
        public string GetFunctionName() { return functionSymbol; }

        public NodeType GetNodeType() { return NodeType.FUNCTION; }

        public void PrettyPrint()
        {
            Console.WriteLine("Nodetype: " + NodeType.FUNCTION + ", symbol: " + functionSymbol);
            if (parameter != null)
            {
                parameter.PrettyPrint();
            }
        }

        public int GetRow() { return row; }

        public int GetCol() { return col; }
    }
}
