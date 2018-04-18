
using Regular_Expression_to_DFA.Utilities;
using System;

namespace Regular_Expression_to_DFA
{
    class Program
    {
        static void Main(string[] args)
        {
            // var input = "(a|b)*abb";
            Console.WriteLine("Introduce a regular expression: ");
            Console.Write("Expression = ");
            var input = Console.ReadLine();

            var expression = new RegularExpression(input);
            var dfa = new DFA(expression);

            var graphDrawer = new DFAPrinter(dfa);
            graphDrawer.ConsolePrintGraph();
            graphDrawer.DrawGraph();
          
        }        
    }

}
