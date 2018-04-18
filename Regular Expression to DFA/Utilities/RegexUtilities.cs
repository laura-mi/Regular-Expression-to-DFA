using Regular_Expression_to_DFA.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regular_Expression_to_DFA.Utilities
{
    /// <summary>
    /// Static methods used to pre-process a regex
    /// </summary>
    public class RegexUtilities
    {        
        private static char[] AddPharantesisToLanguage(char[] nodes)
        {
            var length = nodes.Length;
            //Add pharantesis
            for (int i = 0; i < length; i++)
            {
                var item = nodes[i];
                if (item.IsReunion()) //Add pharanthesis
                {
                    var left = IsolateLeft(i, nodes);
                    var right = IsolateRight(i, nodes);
                    nodes = Concatenate(left, right);
                    i += 2; //previously added pharantesis
                    length += 4;
                }
            }
            return nodes;
        }
        public static char[] AddConcatenationSymbol(char[] nodes)
        {
            var length = nodes.Length;
            var symbols = new bool[length];
            for (int i = 1; i < length; i++)
            {
                var item = nodes[i];
                var previousItem = nodes[i - 1];
                if (item.isLetter() &&
                    (previousItem.isLetter() || previousItem.IsKleene() || previousItem == ')')
                    || item == '(' &&
                    (previousItem.isLetter() || previousItem.IsKleene() || previousItem == ')'))
                    symbols[i] = true;
            }
            //count the nr of symbols 
            int nrOfSymbols = 0;
            for (int j = 0; j < length; j++)
                if (symbols[j]) nrOfSymbols++;

            var initialLength = length;
            length += nrOfSymbols;
            var extendedNodes = new char[length];
            for (int j = 0, k = 0; j < initialLength;)
            {
                if (symbols[j])

                    extendedNodes[k++] = '.';

                extendedNodes[k++] = nodes[j++];
            }
            return extendedNodes;
        }

        public static char[] PreProcessLanguage(string language)
        {
            var nodes = language.ToCharArray();
            nodes = AddPharantesisToLanguage(nodes);                     
            nodes = AddConcatenationSymbol(nodes);  //Add concatenation symbol   
            return nodes;            
        }

        /// <summary>
        ///  Adds the left side of an OR expression into pharantesis
        /// </summary>
        /// <param name="position"></param>
        /// <param name="nodes"></param>
        /// <returns></returns>
        private static char[] IsolateLeft(int position,char[] nodes)
        {
            int length = position+2;

            char[] isolated = new char[length];
            //last element must be closed pharantesis
            isolated[length - 1] = ')';

            int k = length - 2;
            int i = position - 1;
            while (i>=0)
            {
                var current = nodes[i];
                if (current == ')') //copy all in these pharantesis
                {
                    while (i >= 0 && nodes[i] != '(')
                        isolated[k--] = nodes[i--];
                    isolated[k--] = nodes[i--];
                }
                else
                if (current != '(') 
                    isolated[k--] = nodes[i--];
                else
                {
                    //it means we are in a patch of closed pharantesis
                    isolated[k--] = '('; //so here will begin our patch
                    
                    //copy all the rest
                    while (i >= 0) isolated[k--] = nodes[i--];
                }
                
            }
            if (k == 0 && isolated[k] == 0)
               isolated[k] = '(';         
            return isolated;
        }

        /// <summary>
        ///  Adds the right side of an OR expression into pharantesis
        /// </summary>
        /// <param name="position"></param>
        /// <param name="nodes"></param>
        /// <returns></returns>
        private static char[] IsolateRight(int position,char[] nodes)
        {
            var length = nodes.Length - 1 - position + 2;

            char[] isolated = new char[length];
            //fist element must be closed pharantesis
            isolated[0] = '(';
            int k = 1;
            int i = position + 1;
            while(i < nodes.Length)
            {
                var current = nodes[i];
                if (current == '(') //copy all in these pharantesis
                {
                    while (i < nodes.Length && nodes[i] != ')')
                        isolated[k++] = nodes[i++];
                    isolated[k++] = nodes[i++];
                }
                else
                {
                    if (current != ')')
                        isolated[k++] = nodes[i++];
                    else
                    {
                        //it means we are in a patch of closed pharantesis
                        isolated[k++] = ')'; //so here will begin our patch                    
                                             //copy all the rest
                        while (i < nodes.Length) isolated[k++] = nodes[i++];
                    }
                }
            }
            if (k == length-1 && isolated[k] == 0)
                isolated[k] = ')';

            //If we only have one character, the pharantesis are pointless
            //if (isolated.Length == 3)
            //{
            //    var node = new char[1];
            //    node[0] = isolated[1];
            //    return node;
            //}
            return isolated;
        }

        /// <summary>
        /// Concatenate two expressions with OR operator
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private static char[] Concatenate(char[] left, char[] right)
        {
            int i;
            var concatenated = new char[left.Length + right.Length+1];
            for (i = 0; i < left.Length; i++)
                concatenated[i] = left[i];

            int k = i;
            concatenated[k++] = '|';

            for (i = 0; i < right.Length; i++)
                concatenated[k++] = right[i];
            return concatenated;
        }
    }
}
