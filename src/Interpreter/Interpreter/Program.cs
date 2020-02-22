using System.IO;

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
            // user must provide path to source code file as input parameter
            if (args.Length != 0)
            {
                System.Console.WriteLine($"IOError::Please provide path to Mini-PL source file!");
                return -1;
            }

            // check that input source code file exists
            //string sourceFilePath = args[0];
            string sourceFilePath = "C:\\Users\\Marski\\Desktop\\code1.txt";
            if (!File.Exists(sourceFilePath))
            {
                System.Console.WriteLine($"IOError::Invalid source file. File not found!");
                return -1;
            }
            
            // create scanner for lexical analysis
            Scanner scanner = new Scanner(sourceFilePath);

            // create parser for syntax analysis
            Parser parser = new Parser(scanner);



            // Semantic analysis

            // Interpreter

            // Done
            return 0;
        }
    }
}
