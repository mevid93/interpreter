using System.Collections.Generic;


namespace Interpreter
{
    /// <summary>
    /// Class <c>SymbolTable</c> represents the datastructure of semantic analysis
    /// where smybols and their status are stored.
    /// </summary>
    class SymbolTable
    {
        private Stack<Symbol> symbols;      // stack where all symbols are saved
        private int currentScope;           // scope that is undergoind process

        /// <summary>
        /// Constructor <c>SymbolTable</c> creates SymbolTable-object.
        /// </summary>
        public SymbolTable()
        {
            symbols = new Stack<Symbol>();
        }

        /// <summary>
        /// Method <c>AddScope</c> adds new scope level to stack.
        /// </summary>
        public void AddScope()
        {
            currentScope++;
        }

        /// <summary>
        /// Method <c>RemoveScope</c> removes last scope level from stack
        /// </summary>
        public void RemoveScope()
        {
            currentScope--;
            // remove all symbols from the symbol table, where the scope is higher than the current scope.
            // top of stack is filled with symbols of higher scope
            while (symbols.Count > 0 && symbols.Peek().GetScope() > currentScope)
            {
                symbols.Pop();
            }
        }

        /// <summary>
        /// Method <c>GetCurrentScope</c> returns the current scope of symbol table.
        /// </summary>
        public int GetCurrentScope()
        {
            return currentScope;
        }

        /// <summary>
        /// Method <c>IsSymbolInTable</c> checks if given symbol is in symbol table (stack).
        /// </summary>
        public bool IsSymbolInTable(string identifier)
        {
            foreach (Symbol s in symbols)
            {
                if (s.GetIdentifier().Equals(identifier))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Method <c>DeclareSymbol</c> adds the symbol to symbol table (stack).
        /// </summary>
        public void DeclareSymbol(Symbol newSymbol)
        {
            symbols.Push(newSymbol);
        }

        /// <summary>
        /// Method <c>GetSymbolByIdentifier</c> returns the symbol that matches the identifier.
        /// </summary>
        public Symbol GetSymbolByIdentifier(string identifier)
        {
            foreach (Symbol s in symbols)
            {
                if (s.GetIdentifier().Equals(identifier))
                {
                    return s;
                }
            }
            // could not find the symbol matching the identifier
            return null;
        }

        /// <summary>
        /// Method <c>UpdateSymbol</c> updates the value of symbol corresponding to given identifier.
        /// </summary>
        /// <param name="identifier">symbol to be updated</param>
        /// <param name="value">new value for symbol</param>
        public void UpdateSymbol(string identifier, string value)
        {
            foreach (Symbol s in symbols)
            {
                if (s.GetIdentifier().Equals(identifier))
                {
                    s.SetValue(value);
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Class <c>Symbol</c> represents single symbol in the symbol table.
    /// </summary>
    public class Symbol
    {
        private readonly string identifier;         // variable symbol  (identifier)
        private readonly string type;               // variable type    ("string", "int", "bool")
        private string currentValue;                // value that symbols is currently holding
        private readonly int scope;                 // scope of variable (lower means wider scope)

        /// <summary>
        /// Constructor <c>Symbol</c> creates new Symbol-object.
        /// </summary>
        /// <param name="identifier">identifier of symbol</param>
        /// <param name="type">type os symbol ("string", "int", "bool")</param>
        /// <param name="currentValue">valu of symbol</param>
        /// <param name="scope">scope of symbol</param>
        public Symbol(string identifier, string type, string currentValue, int scope)
        {
            this.identifier = identifier;
            this.type = type;
            this.currentValue = currentValue;
            this.scope = scope;
        }

        /// <summary>
        /// Method <c>GetIdentifier</c> returns identifier of symbol.
        /// </summary>
        /// <returns>symbol identifier</returns>
        public string GetIdentifier() { return identifier; }

        /// <summary>
        /// Method <c>GetSymbolType</c> returns the type of value the symbol is holding ("string", "int", "bool").
        /// </summary>
        /// <returns>type of symbol</returns>
        public string GetSymbolType() { return type; }

        /// <summary>
        /// Method <c>GetCurrentValue</c> returns the value the symbol is currently holding.
        /// </summary>
        /// <returns>value of symbol</returns>
        public string GetCurrentValue() { return currentValue; }

        /// <summary>
        /// Method <c>SetValue</c> sets the value of symbol.
        /// </summary>
        public void SetValue(string newValue) { currentValue = newValue; }

        /// <summary>
        /// Method <c>GetScope</c> returns the scope of symbol.
        /// </summary>
        /// <returns>scope of symbol</returns>
        public int GetScope() { return scope; }
    }
}
