
using System.Collections.Generic;

namespace Interpreter
{
    /// <summary>
    /// class <c>Parser</c> performs the syntax analysis for source code.
    /// Parser also constructs the AST.
    /// Does top-down parsing by using LL(1) algorithm.
    /// </summary>
    class Parser
    {

        private Scanner scanner;

        /// <summary>
        /// constructor <c>Parser</c> creates new Parser-object.
        /// </summary>
        /// <param name="tokenScanner">scanner-object</param>
        public Parser(Scanner tokenScanner)
        {
            scanner = tokenScanner;
        }

        

        private void ProcedureProgram()
        {

        }


        // procedure statement

        // procedure expr

        // procedure term tail

        // something something ...

    }
}
