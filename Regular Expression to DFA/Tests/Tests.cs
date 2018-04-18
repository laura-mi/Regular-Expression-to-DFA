using Regular_Expression_to_DFA.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Regular_Expression_to_DFA.Tests
{
    /// <summary>
    /// Tests used to check if the regex is parsed well before creating the syntactic tree for the brute force parsing models
    /// </summary>
    public static class Tests
    {
        static readonly string[] TestInputs =
            {
            "a|b",
            "a*b*|(a|b)ab",
              "a*b*|(a|b)a",
            "a*b*(a|b)a",
            "(ab)*ab(a*b*)ab|(a|b)ab(ab)*ab"
            };
        static readonly string[] TestOutputs =
        {
            ".|#ab",
            ".|#..XX**.bXXXXaXbX|aXXXXXXXXXXXXXXXXXXab",
            ".|#..XX**|aXXXXaXbXab",
            "..#.aXX.|XXXXXX**abXXXXXXXXXXXXaXb"

        };
        static readonly string[] SymbolsOutput =
        {
            "(a)|(b)",
            "(a*.b*)|(((a)|(b)).a.b)",
            "(a*.b*)|(((a)|(b)).a)",
            "a*.b*.((a)|(b)).a",
            "((a.b)*.a.b.(a*.b*).a.b)|(((a)|(b)).a.b.(a.b)*.a.b)"
        };
        [Fact]
        public static void TheSymbolsProcessTest()
        {
            int i = 0;
            while(i < SymbolsOutput.Length)
            {
                var test = TestInputs[i];
                var sentence = RegexUtilities.PreProcessLanguage(test);

                StringBuilder builder = new StringBuilder();
                foreach (char item in sentence)
                    builder.Append(item);
              var output = builder.ToString();

                Assert.Equal(output, SymbolsOutput[i]);
                i++;
            }
            Console.ReadLine();
        }
        [Fact]
        public static void TheSyntaxTreeTest()
        {
            int i = 0;
            while (i < TestOutputs.Length)
            {
                var test = TestInputs[i];
                var graph = new TreeExpression_Brute(test);
                Assert.Equal(graph.TreeString, TestOutputs[i]);
                Console.WriteLine(graph.TreeString);
                i++;
            }
            Console.ReadLine();
        }
    }
}
