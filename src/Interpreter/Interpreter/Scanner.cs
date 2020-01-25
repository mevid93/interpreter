
namespace Interpreter
{
    /// <summary>
    /// class <c>Scanner</c> for scanning input file.
    /// Scans and groups successive characters in souce code file into tokens.
    /// </summary>
    class Scanner
    {
        private string sourceFilePath;

        /// <summary>
        /// constructor for <c>Scanner</c> object.
        /// </summary>
        /// <param name="filepath">path to Mini-PL source code file</param>
        public Scanner(string filepath)
        {
            sourceFilePath = filepath;
        }

        /// <summary>
        /// method <c>Scan</c> for scanning file contents.
        /// </summary>
        public void Scan()
        {
            System.Console.WriteLine("Not implemented!");
        }

    }
}
