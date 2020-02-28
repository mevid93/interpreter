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
        BOOLEAN,        // node with constant boolean
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
        NOT,            // node that represents NOT operator (unary)
        ERROR           // in case of errors... this can be used
    }

    /// <summary>
    /// class <c>Node</c> is abstract base class for nodes in abstract syntax tree.
    /// </summary>
    abstract class Node
    {
        protected string symbol;    // symbol of node
        protected NodeType type;    // type of node

        /// <summary>
        /// constructor <c>AST</c> creates AST-object.
        /// </summary>
        /// <param name="nodeType"></param>
        /// <param name="nodeSymbol"></param>
        public Node(NodeType nodeType, string nodeSymbol)
        {
            symbol = nodeSymbol;
            type = nodeType;
        }
        
        /// <summary>
        /// method <c>GetNodeValue</c> returns the value of token corresponding the node.
        /// </summary>
        /// <returns></returns>
        public string GetNodeSymbol()
        {
            return symbol;
        }

        /// <summary>
        /// method <c>GetNodeType</c> returns the type of node. This is useful for
        /// semantic analysis.
        /// </summary>
        /// <returns></returns>
        public NodeType GetNodeType()
        {
            return type;
        }

        /// <summary>
        /// method <c>PrintInformation</c> prints node information.
        /// This is only used for debugging.
        /// </summary>
        public virtual void PrintInformation()
        {
            Console.WriteLine("Nodetype: " + type + ", symbol: " + symbol);
        }
    }

    /// <summary>
    /// class <c>VariableNode</c> is variable holding node.
    /// When variables are parsed, variable node should be added to AST.
    /// </summary>
    class VariableNode: Node
    {
        private string variableType;    // type of variable

        /// <summary>
        /// constructor <c>VariableNode</c> creates new VariableNode-object.
        /// </summary>
        /// <param name="nodeType"></param>
        /// <param name="nodeSymbol"></param>
        /// <param name="type"></param>
        public VariableNode(string nodeSymbol, string type): base(NodeType.VARIABLE, nodeSymbol)
        {
            this.variableType = type;
        }

        /// <summary>
        /// method <c>GetType</c> returns the type of variable in node.
        /// </summary>
        public string GetVariableType()
        {
            return variableType;
        }

        /// <summary>
        /// Method <c>PrintInformation</c> prints the node information.
        /// Method is only used for debugging.
        /// </summary>
        public override void PrintInformation()
        {
            Console.WriteLine("Nodetype: " + type + ", symbol: " + symbol + ", variable type: " + variableType);
        }
    }

    /// <summary>
    /// class <c>IntegerNode</c> represents constant ingeger.
    /// </summary>
    class IntegerNode : Node
    {
        /// <summary>
        /// constructor <c>IntegerNode</c> creates new IntegerNode-object.
        /// </summary>
        /// <param name="nodeSymbol"></param>
        public IntegerNode(string nodeSymbol): base(NodeType.INTEGER, nodeSymbol)
        {
        }
    }

    /// <summary>
    /// class <c>StringNode</c> represents constant string.
    /// </summary>
    class StringNode: Node
    {
        /// <summary>
        /// constructor <c>StringNode</c> creates new StringNode-object.
        /// </summary>
        /// <param name="nodeSymbol"></param>
        public StringNode(string nodeSymbol):base(NodeType.STRING, nodeSymbol) { }
    }

    /// <summary>
    /// class <c>ExpressionNode</c> represents nodes that have two child nodes.
    /// There are multiple use cases for this node.
    /// </summary>
    class ExpressionNode: Node
    {
        private Node lhs;       // left child tree
        private Node rhs;       // right child tree

        /// <summary>
        /// constructor <c>ExpressionNode</c> creates ExpressionNode-object.
        /// </summary>
        /// <param name="nodeType"></param>
        /// <param name="symbol"></param>
        /// <param name="value"></param>
        public ExpressionNode(NodeType nodeType, string nodeSymbol, Node lhs, Node rhs) : base(nodeType, nodeSymbol)
        {
            this.lhs = lhs;
            this.rhs = rhs;
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

        /// <summary>
        /// method <c>PrintInformation</c> prints node information.
        /// Is only used for debugging.
        /// </summary>
        public override void PrintInformation()
        {
            Console.WriteLine("Nodetype: " + type + ", symbol: " + symbol);
            if (lhs != null)
            {
                Console.Write("LHS: ");
                lhs.PrintInformation();
            }
            if (rhs != null)
            {
                Console.Write("RHS: ");
                rhs.PrintInformation();
            }
        }
    }

    /// <summary>
    /// class <c>NotNode</c> represents not operator.
    /// </summary>
    class NotNode: Node
    {
        private Node child;     // unary operator can only have one child

        /// <summary>
        /// constructor <c>NotNode</c> creates NotNode-object.
        /// </summary>
        /// <param name="nodeSymbol"></param>
        /// <param name="child"></param>
        public NotNode(string nodeSymbol, Node child):base(NodeType.NOT, nodeSymbol)
        {
            this.child = child;
        }

        /// <summary>
        /// method <c>PrintInformation</c> prints node information.
        /// Is only used for debugging.
        /// </summary>
        public override void PrintInformation()
        {
            Console.WriteLine("Nodetype: " + type + ", symbol: " + symbol);
            if (child != null)
            {
                child.PrintInformation();
            }
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

        /// <summary>
        /// constructor <c>ForloopNode</c> creates new ForloopNode-object.
        /// </summary>
        /// <param name="nodeType"></param>
        /// <param name="nodeSymbol"></param>
        /// <param name="variable"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public ForloopNode(string nodeSymbol, Node variable, Node start, Node end): base(NodeType.FOR_LOOP, nodeSymbol)
        {
            this.variable = variable;
            this.start = start;
            this.end = end;
            statements = new List<Node>();
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

        /// <summary>
        /// method <c>PrintInformation</c> prints node information.
        /// This method is only used for debugging.
        /// </summary>
        /// <returns></returns>
        public override void PrintInformation()
        {
            Console.WriteLine("Nodetype: " + type + ", symbol: " + symbol);
            if(variable != null)
            {
                variable.PrintInformation();
            }
            if (start != null)
            {
                Console.Write("START: ");
                start.PrintInformation();
            }
            if (end != null)
            {
                Console.Write("END: ");
                end.PrintInformation();
            }
            foreach(Node statement  in statements)
            {
                statement.PrintInformation();
            }
        }
    }

    /// <summary>
    /// class <c>FunctionNode</c> represents function in AST.
    /// Mini-PL only supports functions with single parameter.
    /// </summary>
    class FunctionNode: Node
    {
        private Node parameter;     // parameter node
        
        /// <summary>
        /// constructor <c>FunctionNode</c> represents function call in AST.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="parameter"></param>
        public FunctionNode(string symbol, Node parameter): base(NodeType.FUNCTION, symbol)
        {
            this.parameter = parameter;
        }

        /// <summary>
        /// method <c>GetParameter</c> returns node representing function call parameter.
        /// </summary>
        /// <returns></returns>
        public Node GetParameter()
        {
            return parameter;
        }

        /// <summary>
        /// method <c>ToString</c> prints node information.
        /// This method is only used for debugging.
        /// </summary>
        /// <returns></returns>
        public override void PrintInformation()
        {
            Console.WriteLine("Nodetype: " + type + ", symbol: " + symbol);
            if (parameter != null)
            {
                parameter.PrintInformation();
            }
        }
    }
}
