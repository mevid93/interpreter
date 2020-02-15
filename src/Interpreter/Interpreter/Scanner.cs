using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Interpreter
{
    /// <summary>
    /// class <c>Scanner</c> for scanning input file.
    /// Scans and groups successive characters in souce code file into tokens.
    /// </summary>
    class Scanner
    {
        private string sourceFilePath;          // path to source code file
        private List<Token> allTokens;          // list of all tokens extracted from source code file
        private bool scanSuccessed;             // flag telling if scan was succesfull or not
        private bool processingCommentblock;    // flag telling if processing multiline comment block /* */
        private int currentLineNum;             // line of source code that is processed

        /// <summary>
        /// constructor for <c>Scanner</c> object.
        /// </summary>
        /// <param name="filepath">path to Mini-PL source code file</param>
        public Scanner(string filepath)
        {
            sourceFilePath = filepath;
            allTokens = new List<Token>();
            scanSuccessed = true;
            currentLineNum = 1;
        }

        /// <summary>
        /// method <c>Scan</c> for scanning file contents.
        /// </summary>
        /// <returns>true if scan was succesfull</returns>
        public bool Scan()
        {
            try
            {
                // read all lines from source files and find tokens from source code
                foreach(string line in File.ReadLines(sourceFilePath))
                {
                    List<Token> tmp = StringToTokens(line);
                    allTokens.AddRange(tmp);
                    currentLineNum++;
                }
                // operation was succesfull
                return scanSuccessed;
            }
            catch
            {
                // error... file scanning was not succesful
                System.Console.WriteLine("IOError::Failed to scan file!\n");
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
            List<Token> lineTokens = new List<Token>(); // list holding extracted tokens

            // iterate through line and extract tokens
            for(int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                // if processing comment block
                if (processingCommentblock)
                {
                    if(c == '*' && i < line.Length - 1 && line[i + 1] == '/')
                    {
                        i++;
                        processingCommentblock = false;
                    }
                    continue;
                }

                // skip initial whitespaces
                if (char.IsWhiteSpace(c))
                {
                    continue;
                }

                // single character tokens
                char[] singleCharTokens = { '(', ')', '+', '-', '*', '<', '&', '!', ';', '='};
                if (singleCharTokens.Contains(c))
                {
                    lineTokens.Add(new Token(c.ToString(), Token.FindTokenType(c.ToString()), currentLineNum, i));
                    continue;
                }

                // :-character (two possible cases else error)
                if(c == ':')
                {
                    // 1. assingment --> next character is =
                    if (i < line.Length - 1 && line[i + 1] == '=')
                    {
                        lineTokens.Add(new Token(":=", TokenType.ASSIGNMENT, currentLineNum, i));
                        i++;
                        continue;
                    }
                    // 2. else :
                    lineTokens.Add(new Token(":", TokenType.SEPARATOR, currentLineNum, i));
                    continue;
                }

                if (c == '/')
                {
                    // if next char is / then comment block read next line
                    if(i < line.Length - 1 && line[i + 1] == '/')
                    {
                        return lineTokens;
                    }
                    // if next char is */ then comment block read till next */
                    if (i < line.Length - 1 && line[i + 1] == '*')
                    {
                        processingCommentblock = true;
                        i++;
                        continue;
                    }
                    // else div
                    lineTokens.Add(new Token("/", TokenType.DIVIDE, currentLineNum, i));
                    continue;
                }

                if (c == '.')
                {
                    // if next char is . then comment block read next line
                    if (i < line.Length - 1 && line[i + 1] == '.')
                    {
                        lineTokens.Add(new Token("..", TokenType.RANGE, currentLineNum, i));
                        i++;
                        continue;
                    }
                    // else error
                    System.Console.WriteLine($"SyntaxError::Line {currentLineNum}::Column {i}::Invalid usage of .!");
                    scanSuccessed = false;
                    continue;
                }

                if (c == '"')
                {
                    string value = "";
                    bool ended = false;
                    int col = i;
                    i++;
                    // keep scanning until next " if line ends before ", then error
                    while(i < line.Length)
                    {
                        if(line[i] == '"')
                        {
                            ended = true;
                            break;
                        }
                        value += line[i];
                        i++;
                    }
                    if (ended)
                    {
                        lineTokens.Add(new Token(value, TokenType.VAL_STRING, currentLineNum, i));
                        continue;
                    }
                    System.Console.WriteLine($"SyntaxError::Line {currentLineNum}::Column {col}::Invalid usage of \"");
                    scanSuccessed = false;
                }

                if (char.IsDigit(c) && c != '0')
                {
                    // read any additional digits and return number
                    string number = "" + c;
                    while(i + 1 < line.Length && char.IsDigit(line[i + 1]))
                    {
                        number += line[i + 1];
                        i++;
                    }
                    lineTokens.Add(new Token(number, TokenType.VAL_INTEGER, currentLineNum, i));
                    continue;
                }

                if (IsAlphabet(c))
                {
                    // read any additional letters and digits
                    string letters = "" + c;
                    while(char.IsDigit(line[i + 1]) || IsAlphabet(line[i + 1]))
                    {
                        letters += line[i + 1];
                        i++;
                    }
                    // check if one of the reserwed keywords
                    string[] keywords = {"var", "for", "end", "in", "do", "read", "print", "int", "string", "bool", "assert" };
                    if(keywords.Contains(letters))
                    {
                        lineTokens.Add(new Token(letters, Token.FindTokenType(letters), currentLineNum, i));
                        continue;
                    }
                    // else return ident
                    lineTokens.Add(new Token(letters, TokenType.IDENTIFIER, currentLineNum, i));
                    continue;
                }

                // else anounce an error
                System.Console.WriteLine($"SyntaxError::Line {currentLineNum}::Column {i}::Invalid token detected!");
                scanSuccessed = false;
            }

            // all done... return extracted tokens
            return lineTokens;
        }

        /// <summary>
        /// method <c>IsAlphabet</c> for checking if character is alphabet character (a-z, A-Z).
        /// </summary>
        /// <param name="c"></param>
        /// <returns>true if character is alphabet character</returns>
        private bool IsAlphabet(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
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

        /// <summary>
        /// method <c>PrintScannedTokens</c> for printing scanned tokens.
        /// used only for debugging.
        /// </summary>
        public void PrintScannedTokens()
        {
            foreach(Token token in allTokens)
            {
                System.Console.WriteLine(token.ToString());
            }
        }

    }
}
