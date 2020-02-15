
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

        private List<Token> tokens;     // list of tokens that scanner was able to extract
        private bool errorsDetected;    // flag that tells if errors were detected

        /// <summary>
        /// constructor <c>Parser</c> creates new Parser-object.
        /// </summary>
        /// <param name="scannedTokens">list of scanned tokens</param>
        public Parser(List<Token> scannedTokens)
        {
            tokens = scannedTokens;
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
