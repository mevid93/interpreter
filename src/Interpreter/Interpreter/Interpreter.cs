using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter
{
    class Interpreter
    {

        private List<Node> ast;

        public Interpreter(List<Node> ast)
        {
            this.ast = ast;
        }

        public void Execute()
        {

        }
    }
}
