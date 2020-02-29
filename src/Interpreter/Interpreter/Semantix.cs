using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter
{
    /// <summary>
    /// class <c>Semnatix</c> holds functionality to do semantic analysis for
    /// intermediate representation of source code. In other words, it takes
    /// AST as input, checks semantic constraints and reports any error it finds.
    /// </summary>
    class Semantix
    {
        private List<Node> ast;                                 // AST representation of source code
        private bool errorsDetected;                            // flag telling about status of semantic analysis
        private Dictionary<string, string[]> symbolTable;       // table containing infromation of all the symbols
        
        /// <summary>
        /// constructor <c>Semantix</c> creates new Semantix-object.
        /// </summary>
        /// <param name="ast"></param>
        public Semantix(List<Node> ast)
        {
            this.ast = ast;
            errorsDetected = false;
            // symbol table is dictionary where symbol is key, and corresponding value
            // is a array [type, value]. 
            symbolTable = new Dictionary<string, string[]>();
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
                // check that variable is declared before it is used

                // check that type of variable and its' value matches

                // check that the operands match with operations
                // (int --> int) (string --> string) (bool --> bool)
            }
        }

    }
}
