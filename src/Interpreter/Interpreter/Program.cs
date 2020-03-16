using System.Collections.Generic;
using System.IO;
using System;

namespace Interpreter
{
    /// <summary>
    /// Class <c>Program</c> is the Driver-class for interpreter.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Static method <c>Main</c> starts the Mini-PL interpeter.
        /// </summary>
        /// <param name="args">input parameters (source code file)</param>
        static int Main(string[] args)
        {
            // user must provide path to source code file as input parameter
            if (args.Length != 0)
            {
                Console.WriteLine($"IOError::Please provide path to Mini-PL source file!");
                Console.WriteLine("Expected command is: <program.exe> <sourcecode.txt>");
                return -1;
            }

            // check that input source code file exists
            string sourceFilePath = "C:\\Users\\Marski\\Desktop\\code1.txt";
            if (!File.Exists(sourceFilePath))
            {
                Console.WriteLine($"IOError::Invalid sourcecode file. File not found!");
                return -1;
            }

            // create Scanner-object for lexical analysis
            Scanner scanner = new Scanner(sourceFilePath);

            // create Parser-object for syntax analysis
            Parser parser = new Parser(scanner);

            // syntax analysis and create AST intermediate representation
            List<Node> ast = parser.Parse();

            // semantic analysis
            Semantix semalys = new Semantix(ast);
            semalys.CheckConstraints();

            // check that no errors were detected in source code
            if (parser.NoErrorsDetected() && semalys.NoErrorsDetected())
            {
                // create new Interpreter-object and execute AST
                Interpreter interpreter = new Interpreter(ast);
                interpreter.Execute();
            }

            return 0;
        }
    }
}
