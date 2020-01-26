using System.Collections.Generic;
using System.IO;

namespace Interpreter
{
    /// <summary>
    /// class <c>Scanner</c> for scanning input file.
    /// Scans and groups successive characters in souce code file into tokens.
    /// </summary>
    class Scanner
    {
        private string sourceFilePath;
        private List<Token> allTokens;

        /// <summary>
        /// constructor for <c>Scanner</c> object.
        /// </summary>
        /// <param name="filepath">path to Mini-PL source code file</param>
        public Scanner(string filepath)
        {
            sourceFilePath = filepath;
            allTokens = new List<Token>();
        }

        /// <summary>
        /// method <c>Scan</c> for scanning file contents.
        /// </summary>
        /// <returns>true if scan was succesfull</returns>
        public bool Scan()
        {
            try
            {
                // convert each line in source file to tokens
                // and add them to list of tokens
                foreach(string line in File.ReadLines(sourceFilePath))
                {
                    List<Token> tmp = StringToTokens(line);
                    allTokens.AddRange(tmp);
                }
                // operation was succesfull
                return true;
            }
            catch
            {
                // error... file scanning was not succesful
                System.Console.WriteLine("Failed to scan file!\n");
                return false;
            }
        }

        /// <summary>
        /// method <c>StringToTokens</c> for extracting tokens from string
        /// </summary>
        /// <param name="line">string from which tokens will be extracted</param>
        /// <returns>list of tokens</returns>
        public List<Token> StringToTokens(string line)
        {
            List<Token> lineTokens = new List<Token>(); // list to contain extracted lines
            string currentToken = string.Empty;         // characters for current token in process
            bool isString = false;                      // if string --> there can be whitespaces

            // iterate though line and extract tokens
            for(int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                          

            }

            // all done... return extracted tokens
            return lineTokens;
        }

        /// <summary>
        /// method <c>GetScannedTokens</c> for returning list of tokens
        /// that were scanned from the input source code file.
        /// </summary>
        /// <returns>list of tokens</returns>
        public List<Token> GetScannedTokens()
        {
            return allTokens;
        }

    }
}
