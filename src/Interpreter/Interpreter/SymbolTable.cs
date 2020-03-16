using System.Collections.Generic;


namespace Interpreter
{
    /// <summary>
    /// class <c>SymbolTable</c> represents the datastructure of semantic analysis
    /// where smybols and their status are stored.
    /// </summary>
    class SymbolTable
    {

        private Stack<Symbol> symbols;      // stack where all symbols are saved
        private int currentScope;           // scope that is undergoind process

        /// <summary>
        /// constructor <c>SymbolTable</c> creates new SymbolTable-object.
        /// </summary>
        public SymbolTable()
        {
            symbols = new Stack<Symbol>();
        }

        /// <summary>
        /// method <c>AddScope</c> adds new scope level to stack.
        /// </summary>
        public void AddScope()
        {
            currentScope++;
        }

        /// <summary>
        /// method <c>RemoveScope</c> removes last scope level from stack
        /// </summary>
        public void RemoveScope()
        {
            currentScope--;
            // remove all symbols from the symbol table, where the scope is higher than the current scope.
            // top of stack is filled with symbols of higher scope
            while(symbols.Count > 0 && symbols.Peek().GetScope() > currentScope)
            {
                symbols.Pop();
            } 
        }

        /// <summary>
        /// method <c>GetCurrentScope</c> returns the current scope of symbol table.
        /// </summary>
        public int GetCurrentScope()
        {
            return this.currentScope;
        }

        /// <summary>
        /// method <c>IsSymbolInTable</c> checks if given symbol is in symbol table (stack).
        /// </summary>
        public bool IsSymbolInTable(string identifier)
        {
            foreach(Symbol s in symbols)
            {
                if (s.GetIdentifier().Equals(identifier))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// method <c>DeclareSymbol</c> adds the symbol to symbol table (stack).
        /// </summary>
        public void DeclareSymbol(Symbol newSymbol)
        {
            symbols.Push(newSymbol);
        }

        /// <summary>
        /// method <c>GetSymbolByIdentifier</c> returns the symbol that matches the identifier.
        /// </summary>
        public Symbol GetSymbolByIdentifier(string identifier)
        {
            foreach(Symbol s in symbols)
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
            foreach(Symbol s in symbols)
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
    /// class <c>Symbol</c> represents single symbol in the table
    /// </summary>
    public class Symbol
    {
        private string identifier;          // variable symbol  (identifier)
        private string type;                // variable type    ("string", "int", "bool")
        private string currentValue;        // value that symbols is currently holding
        private int scope;                  // scope of variable (lower means wider scope)

        /// <summary>
        /// constructor <c>Symbol</c> creates new Symbol-object.
        /// </summary>
        public Symbol(string identifier, string type, string currentValue, int scope)
        {
            this.identifier = identifier;
            this.type = type;
            this.currentValue = currentValue;
            this.scope = scope;
        }

        /// <summary>
        /// method <c>GetIdentifier</c> returns identifier of symbol.
        /// </summary>
        /// <returns></returns>
        public string GetIdentifier()
        {
            return this.identifier;
        }

        /// <summary>
        /// method <c>GetSymbolType</c> returns the type of value the symbol is holding
        /// </summary>
        /// <returns></returns>
        public string GetSymbolType()
        {
            return this.type;
        }

        /// <summary>
        /// method <c>GetCurrentValue</c> returns the value the symbol is currently holding.
        /// </summary>
        /// <returns></returns>
        public string GetCurrentValue()
        {
            return this.currentValue;
        }

        /// <summary>
        /// Method <c>SetValue</c> sets the value of symbol.
        /// </summary>
        public void SetValue(string newValue)
        {
            currentValue = newValue;
        }

        /// <summary>
        /// method <c>GetScope</c> returns the scope of symbol.
        /// </summary>
        /// <returns></returns>
        public int GetScope()
        {
            return this.scope;
        }
    }
}
