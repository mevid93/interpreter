using System.Collections.Generic;
using System.IO;
using System;

namespace Interpreter
{
    class Program
    {
        /// <summary>
        /// static method <c>Main</c> for starting Mini-PL interpeter.
        /// </summary>
        /// <param name="args">input parameters (source code file)</param>
        static int Main(string[] args)
        {
            // User must provide path to source code file as input parameter
            if (args.Length != 0)
            {
                Console.WriteLine($"IOError::Please provide path to Mini-PL source file!");
                Console.WriteLine("Expected command is: <program.exe> <sourcecode.txt>");
                return -1;
            }

            // Check that input source code file exists
            string sourceFilePath = "C:\\Users\\Marski\\Desktop\\code1.txt";    // temp dev solution for testing
            if (!File.Exists(sourceFilePath))
            {
                Console.WriteLine($"IOError::Invalid sourcecode file. File not found!");
                return -1;
            }
            
            // Create Scanner-object for lexical analysis
            Scanner scanner = new Scanner(sourceFilePath);

            // Create Parser-object for syntax analysis
            Parser parser = new Parser(scanner);
            
            // Syntax analysis and create AST intermediate representation
            List<Node> ast = parser.Parse();

            // Semantic analysis
            Semantix semalys = new Semantix(ast);
            semalys.CheckConstraints();

            // Interpreter
            if (parser.NoErrorsDetected() && semalys.NoErrorsDetected())
            {
                // No errors were detected from the source code
                // Create new Interpreter-object and execute AST
                Interpreter interpreter = new Interpreter(ast);
                interpreter.Execute();
            }

            // Done
            return 0;
        }
    }
}
