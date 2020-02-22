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
        private bool processingCommentblock;    // flag telling if processing multiline comment block /* */
        private int rowNum;                     // row of source code that is processed
        private int colNum;                     // column of source code that is processed
        private string[] lines;                 // source code lines

        /// <summary>
        /// constructor for <c>Scanner</c> object.
        /// </summary>
        /// <param name="filepath">path to Mini-PL source code file</param>
        public Scanner(string filepath)
        {
            sourceFilePath = filepath;
            processingCommentblock = false;
            rowNum = 0;
            colNum = 0;
            lines = File.ReadAllLines(filepath);
        }

        /// <summary>
        /// method <c>Scan</c> for scanning next token from input source code.
        /// </summary>
        /// <returns>next token</returns>
        public Token ScanNextToken()
        {
            Token token = ExtractNextToken(false);
            return token;
        }

        /// <summary>
        /// method <c>PeekNextToken</c> peeks the next token from input source code.
        /// </summary>
        /// <returns></returns>
        public Token PeekNextToken()
        {
            Token token = ExtractNextToken(true);
            return token;
        }

        /// <summary>
        /// method <c>ExtractNextToken</c> scans the next token from input source code.
        /// parameter isPeek tells if we want to update current position in the source code
        /// at the end of the scan or not. In case isPeek is false, then the position is updated.
        /// </summary>
        /// <param name="isPeek">false if the current position in the source code should not be updated</param>
        /// <returns>scanned token</returns>
        private Token ExtractNextToken(bool isPeek)
        {
            // temporary variables in case the row and col numbers should not be updated
            int tmpRow = rowNum;
            int tmpCol = colNum;
            string error;

            // go through the lines   
            for (int r = tmpRow; r < lines.Length; r++)
            {
                string line = lines[r];

                // go through the columns
                for (int c = tmpCol; c < line.Length; c++)
                {
                    // check if currently processing comment block
                    // then we have to check if there is end of comment block
                    if (processingCommentblock)
                    {
                        if (line[c] == '*' && c < line.Length - 1 && line[c + 1] == '/')
                        {
                            c++;
                            processingCommentblock = false;
                        }
                        continue;
                    }

                    // check if the next character is whitespace
                    // if it is, then continue to next character
                    // skip initial whitespaces
                    if (char.IsWhiteSpace(line[c])) continue;

                    // check if next character is single characer token
                    // if it is, return that single character token
                    char[] singleCharTokens = { '(', ')', '+', '-', '*', '<', '&', '!', ';', '=' };
                    if (singleCharTokens.Contains(line[c]))
                    {
                        HandleRowAndColUpdate(r, c + 1, !isPeek);
                        return new Token(line[c].ToString(), Token.FindTokenType(line[c].ToString()), r, c);
                    }

                    // check if the character is :
                    // if it is, there are two possible cases
                    if (line[c] == ':')
                    {
                        if (c < line.Length - 1 && line[c + 1] == '=')
                        {
                            HandleRowAndColUpdate(r, c + 2, !isPeek);
                            return new Token(":=", TokenType.ASSIGNMENT, r, c);
                        }
                        HandleRowAndColUpdate(r, c + 1, !isPeek);
                        return new Token(":", TokenType.SEPARATOR, r, c);
                    }

                    // check if character is /
                    // if it is, there are three possible valid options
                    // comment for the rest of the line, start of comment block or division
                    if (line[c] == '/')
                    {
                        if (line[c] < line.Length - 1 && line[c + 1] == '/') break;
                        if (line[c] < line.Length - 1 && line[c + 1] == '*')
                        {
                            processingCommentblock = true;
                            c++;
                            continue;
                        }
                        HandleRowAndColUpdate(r, c + 1, !isPeek);
                        return new Token("/", TokenType.DIVIDE, r, c);
                    }

                    // check if character is .
                    // if it is, there is only one valid option, else it is error
                    if (line[c] == '.')
                    {
                        if (c < line.Length - 1 && line[c + 1] == '.')
                        {
                            HandleRowAndColUpdate(r, c + 2, !isPeek);
                            return new Token("..", TokenType.RANGE, r, c);
                        }
                        HandleRowAndColUpdate(r, c + 1, !isPeek);
                        error = $"LexicalError::Row {r}::Column {c + 1}::Expected '.'";
                        System.Console.WriteLine(error);
                        return new Token(error, TokenType.ERROR, r, c);
                    }

                    // check if character is "
                    // if it is, we are working with string and need to keep scanning until it ends
                    if (line[c] == '"')
                    {
                        string value = "";
                        bool ended = false;
                        c++;
                        while (c < line.Length)
                        {
                            if (line[c] == '"')
                            {
                                ended = true;
                                break;
                            }
                            if(line[c] == '\\' && c < line.Length && line[c + 1] == 'n')
                            {
                                c++;
                                value += "\n";
                            }
                            else
                            {
                                value += line[c];
                            }
                            c++;
                        }
                        if (ended)
                        {
                            HandleRowAndColUpdate(r, c + 1, !isPeek);
                            return new Token(value, TokenType.VAL_STRING, r, c);
                        }
                        HandleRowAndColUpdate(r, c + 1, !isPeek);
                        error = $"LexicalError::Row {r}::Column {c}::Expected '\"'";
                        System.Console.WriteLine(error);
                        return new Token(error, TokenType.ERROR, r, c);
                    }

                    // check if character is digit
                    // if it is, read any additional digits and return number
                    if (char.IsDigit(line[c]))
                    {
                        string number = "" + line[c];
                        while (c + 1 < line.Length && char.IsDigit(line[c + 1]))
                        {
                            number += line[c + 1];
                            c++;
                        }
                        HandleRowAndColUpdate(r, c + 1, !isPeek);
                        return new Token(number, TokenType.VAL_INTEGER, r, c);
                    }

                    // check if next character is alphabet
                    // if it is, read any additional letters and digits
                    if (IsAlphabet(line[c]))
                    {
                        string letters = "" + line[c];
                        while (char.IsDigit(line[c + 1]) || IsAlphabet(line[c + 1]))
                        {
                            letters += line[c + 1];
                            c++;
                        }
                        string[] keywords = { "var", "for", "end", "in", "do", "read", "print", "int", "string", "bool", "assert" };
                        HandleRowAndColUpdate(r, c + 1, !isPeek);
                        if (keywords.Contains(letters)) return new Token(letters, Token.FindTokenType(letters), r, c);
                        return new Token(letters, TokenType.IDENTIFIER, r, c);
                    }

                    // if ended here, if means that we were not able to scan the content into valid token
                    error = $"LexicalError::Row {r}::Column {c}::Illegal character!";
                    System.Console.WriteLine(error);
                    HandleRowAndColUpdate(r, c + 1, !isPeek);
                    return new Token(error, TokenType.ERROR, r, c);
                }

                // new line --> reset column 
                tmpCol = 0;
            }

            // if ended here, it means that all rows have allready been scanner --> end of file
            return new Token("EOF", TokenType.EOF, rowNum, colNum);
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
        /// method <c>HandleRowAndColUpdate</c> updates row and column number if the third
        /// boolean paramter is true.
        /// </summary>
        private void HandleRowAndColUpdate(int row, int col, bool update)
        {
            if (update)
            {
                rowNum = row;
                colNum = col;
            }
        }

    }
}
