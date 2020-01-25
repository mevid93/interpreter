
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
                System.Console.WriteLine("Please provide path to Mini-PL source file.\n");
                return -1;
            }

            // check that input source code file exists
            string sourceFilePath = args[0];
            if (!System.IO.File.Exists(sourceFilePath))
            {
                System.Console.WriteLine("Invalid source file. File not found!\n");
                return -1;
            }

            // create scanner and scan the input file
            Scanner scanner = new Scanner(sourceFilePath);
           

            return 0;
        }
    }
}
