using System;
using System.Collections.Generic;

namespace Interpreter
{
    /// <summary>
    /// enum <c>NodeType</c> represents the type of node.
    /// </summary>
    enum NodeType {
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
    /// class <c>Node</c> is node interface that must be implemented by nodes.
    /// </summary>
    interface Node
    {

        /// <summary>
        /// method <c>CheckType</c> returns the type of node.
        /// </summary>
        /// <returns></returns>
        NodeType CheckType();

        /// <summary>
        /// method <c>PrettyPrint</c> prints the string representation of node.
        /// </summary>
        void PrettyPrint();

        /// <summary>
        /// method <c>GetRow</c> returns the row in source column that corresponds the node in AST.
        /// </summary>
        int GetRow();

        /// <summary>
        /// method <c>GetCol</c> returns the column in source column that corresponds the node in AST.
        /// </summary>
        int GetCol();
    }

    /// <summary>
    /// class <c>VariableNode</c> is variable holding node.
    /// When variables are parsed, variable node should be added to AST.
    /// </summary>
    class VariableNode: Node
    {
        private string variableSymbol;  // symbol of variable
        private string variableType;    // type of variable
        private int row;                // row in source code
        private int col;                // column in source code

        /// <summary>
        /// constructor <c>VariableNode</c> creates new VariableNode-object.
        /// </summary>
        /// <param name="variableSymbol"></param>
        /// <param name="variableType"></param>
        public VariableNode(int row, int col, string variableSymbol, string variableType)
        {
            this.variableType = variableType;
            this.variableSymbol = variableSymbol;
        }

        /// <summary>
        /// method <c>GetVariableSymbol</c> returns the identifier symbol for variable.
        /// </summary>
        public string GetVariableSymbol()
        {
            return variableSymbol;
        }

        /// <summary>
        /// method <c>GetVariableValue</c> returns the value of variable.
        /// </summary>
        /// <returns></returns>
        public string GetVariableType()
        {
            return this.variableType;
        }

        public NodeType CheckType()
        {
            return NodeType.VARIABLE;
        }

        public void PrettyPrint()
        {
            Console.WriteLine("Nodetype: " + NodeType.VARIABLE + ", symbol: " + this.variableSymbol + ", variable type: " + this.variableType);
        }

        public int GetRow()
        {
            return this.row;
        }

        public int GetCol()
        {
            return this.col;
        }
    }

    /// <summary>
    /// class <c>IntegerNode</c> represents constant ingeger.
    /// </summary>
    class IntegerNode : Node
    {
        private string integerValue;
        private int row;                // row in source code
        private int col;                // column in source code

        /// <summary>
        /// constructor <c>IntegerNode</c> creates new IntegerNode-object.
        /// </summary>
        /// <param name="nodeSymbol"></param>
        public IntegerNode(int row, int col, string integerValue)
        {
            this.integerValue = integerValue;
            this.row = row;
            this.col = col;
        }

        /// <summary>
        /// method <c>GetIntegerValue</c> returns integer (string representation) from node.
        /// </summary>
        public string GetIntegerValue()
        {
            return this.integerValue;
        }

        public NodeType CheckType()
        {
            return NodeType.INTEGER;
        }

        public void PrettyPrint()
        {
            Console.WriteLine("Nodetype: " + NodeType.INTEGER + ", value: " + this.integerValue);
        }

        public int GetRow()
        {
            return this.row;
        }

        public int GetCol()
        {
            return this.col;
        }
    }

    /// <summary>
    /// class <c>StringNode</c> represents constant string.
    /// </summary>
    class StringNode: Node
    {
        private string stringValue;
        private int row;
        private int col;

        /// <summary>
        /// constructor <c>StringNode</c> creates new StringNode-object.
        /// </summary>
        /// <param name="stringValue"></param>
        public StringNode(int row, int col, string stringValue)
        {
            this.stringValue = stringValue;
            this.row = row;
            this.col = col;
        }

        /// <summary>
        /// method <c>GetStringValue</c> returns string value from node.
        /// </summary>
        /// <returns></returns>
        public string GetStringValue()
        {
            return this.stringValue;
        }

        public NodeType CheckType()
        {
            return NodeType.STRING;
        }

        public void PrettyPrint()
        {
            Console.WriteLine("Nodetype: " + NodeType.STRING + ", symbol: " + this.stringValue);
        }

        public int GetRow()
        {
            return this.row;
        }

        public int GetCol()
        {
            return this.col;
        }
    }

    /// <summary>
    /// class <c>ExpressionNode</c> represents nodes that have two child nodes.
    /// There are multiple use cases for this node.
    /// </summary>
    class ExpressionNode: Node
    {
        private NodeType type;      // type of node
        private string nodeSymbol;  // symbol of node (operation)
        private Node lhs;           // left child tree
        private Node rhs;           // right child tree
        private int row;
        private int col;

        /// <summary>
        /// constructor <c>ExpressionNode</c> creates ExpressionNode-object.
        /// </summary>
        /// <param name="nodeType"></param>
        /// <param name="symbol"></param>
        /// <param name="value"></param>
        public ExpressionNode(int row, int col, NodeType nodeType, string nodeSymbol, Node lhs, Node rhs)
        {
            this.lhs = lhs;
            this.rhs = rhs;
            this.type = nodeType;
            this.nodeSymbol = nodeSymbol;
            this.row = row;
            this.col = col;
        }

        /// <summary>
        /// method <c>GetNodeSymbol</c> returns the (operation) symbol of node.
        /// </summary>
        /// <returns></returns>
        public string GetNodeSymbol()
        {
            return this.nodeSymbol;
        }

        /// <summary>
        /// method <c>GetLhs</c> returns lhs.
        /// </summary>
        /// <returns></returns>
        public Node GetLhs()
        {
            return lhs;
        }

        /// <summary>
        /// method <c>GetRhs</c> returns rhs.
        /// </summary>
        /// <returns></returns>
        public Node GetRhs()
        {
            return rhs;
        }

        public NodeType CheckType()
        {
            return this.type;
        }

        public void PrettyPrint()
        {
            Console.WriteLine("Nodetype: " + this.type + ", symbol: " + this.nodeSymbol);
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

        public int GetRow()
        {
            return this.row;
        }

        public int GetCol()
        {
            return this.col;
        }
    }

    /// <summary>
    /// class <c>NotNode</c> represents not operator.
    /// </summary>
    class NotNode: Node
    {
        private Node child; // unary operator can only have one child
        private int row;
        private int col;

        /// <summary>
        /// constructor <c>NotNode</c> creates NotNode-object.
        /// </summary>
        /// <param name="nodeSymbol"></param>
        /// <param name="child"></param>
        public NotNode(int row, int col, Node child)
        {
            this.child = child;
            this.row = row;
            this.col = col;
        }

        /// <summary>
        /// method <c>GetChildNode</c> returns child node of node.
        /// </summary>
        /// <returns></returns>
        public Node GetChildNode()
        {
            return this.child;
        }

        public NodeType CheckType()
        {
            return NodeType.NOT;
        }

        public void PrettyPrint()
        {
            Console.WriteLine("Nodetype: " + NodeType.NOT);
            if (child != null)
            {
                child.PrettyPrint();
            }
        }

        public int GetRow()
        {
            return this.row;
        }

        public int GetCol()
        {
            return this.col;
        }
    }

    /// <summary>
    /// class <c>ForloopNode</c> represents for loop in AST.
    /// </summary>
    class ForloopNode: Node
    {
        private Node variable;              // variable that is assigned values in range from start to end
        private Node start;                 // start value
        private Node end;                   // end value
        private List<Node> statements;      // list of statements that are executed in each iteration
        private int row;
        private int col;

        /// <summary>
        /// constructor <c>ForloopNode</c> creates new ForloopNode-object.
        /// </summary>
        /// <param name="nodeType"></param>
        /// <param name="nodeSymbol"></param>
        /// <param name="variable"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public ForloopNode(int row, int col, Node variable, Node start, Node end)
        {
            this.variable = variable;
            this.start = start;
            this.end = end;
            this.statements = new List<Node>();
            this.row = row;
            this.col = col;
        }

        /// <summary>
        /// method <c>AddStatement</c> adds new child (statement) to child nodes.
        /// </summary>
        public void AddStatement(Node node)
        {
            if (node != null) statements.Add(node);
        }

        /// <summary>
        /// method <c>GetStart</c> returns the node that evaluates as starting value for range.
        /// </summary>
        /// <returns></returns>
        public Node GetStart()
        {
            return start;
        }

        /// <summary>
        /// method <c>GetEnd</c> returns the node that evaluates as end value for range.
        /// </summary>
        /// <returns></returns>
        public Node GetEnd()
        {
            return end;
        }

        /// <summary>
        /// method <c>GetStatements</c> returns all the child nodes (statements).
        /// </summary>
        /// <returns></returns>
        public List<Node> GetStatements()
        {
            return statements;
        }

        public NodeType CheckType()
        {
            return NodeType.FOR_LOOP;
        }

        public void PrettyPrint()
        {
            Console.WriteLine("Nodetype: " + NodeType.FOR_LOOP);
            if(variable != null)
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
            foreach(Node statement  in statements)
            {
                statement.PrettyPrint();
            }
        }

        public int GetRow()
        {
            return this.row;
        }

        public int GetCol()
        {
            return this.col;
        }
    }

    /// <summary>
    /// class <c>FunctionNode</c> represents function in AST.
    /// Mini-PL only supports functions with single parameter.
    /// </summary>
    class FunctionNode: Node
    {
        private string functionSymbol;      // function symbol
        private Node parameter;             // parameter node
        private int row;
        private int col;
        
        /// <summary>
        /// constructor <c>FunctionNode</c> represents function call in AST.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="parameter"></param>
        public FunctionNode(int row, int col, string symbol, Node parameter)
        {
            this.parameter = parameter;
            this.functionSymbol = symbol;
            this.row = row;
            this.col = col;
        }

        /// <summary>
        /// method <c>GetParameter</c> returns node representing function call parameter.
        /// </summary>
        /// <returns></returns>
        public Node GetParameter()
        {
            return parameter;
        }

        public NodeType CheckType()
        {
            return NodeType.FUNCTION;
        }

        public void PrettyPrint()
        {
            Console.WriteLine("Nodetype: " + NodeType.FUNCTION + ", symbol: " + this.functionSymbol);
            if (parameter != null)
            {
                parameter.PrettyPrint();
            }
        }

        public int GetRow()
        {
            return this.row;
        }

        public int GetCol()
        {
            return this.col;
        }
    }
}
