using Regular_Expression_to_DFA.Extensions;
using Regular_Expression_to_DFA.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regular_Expression_to_DFA
{
    /// <summary>
    /// I inially parsed the regex by brute force. It is not the best option.
    /// Note that this will work only for small inputs
    /// </summary>
    public class TreeExpression_Brute
    {        
        public char[] Tree;
        public string TreeString;        
        public int Root = 0;

        private char[] sentence;
        private int[] children;
        public TreeExpression_Brute(string input)
        {
            sentence = RegexUtilities.PreProcessLanguage(input);
            long localLength = 0;
            foreach (var item in sentence)
                if (item.IsSymbol() || item.isLetter()) localLength++;
           
            /// Note that this will work only for small inputs
            localLength = (long)(Math.Pow(2, sentence.Length) / 3);           
            Tree = new char[localLength]; //We will get a crash here pretty fast. The hardware limit is ...hardware limit.

            Tree[Root] = '.';
            Tree[LeftNodePos(Root)] = GenerateTree(0, sentence.Length - 1, LeftNodePos(Root));
            Tree[RightNodePos(Root)] = '#';            
            CreateSyntax();
            FindLeaves();
        }

        private char GenerateTree(int start, int end, int root)
        {
            //Cases: 
            //I. We have only one node, so it is a leaf
            if (sentence[start].isLetter() && (end - start == 0))
                return sentence[start];
            if (end - start == 2 && sentence[start] == '(' && sentence[end] == ')')
                return sentence[start + 1];
            //Or a leaf with kleene
            if (sentence[start].isLetter() && (end - start == 1) && sentence[end].IsKleene())
            {
                Tree[LeftNodePos(root)] = GenerateTree(start, end-1, LeftNodePos(root));
                return '*';
            }
            if (end - start == 3 && sentence[start] == '(' && sentence[end - 1] == ')' && sentence[end].IsKleene())
            {
                Tree[LeftNodePos(root)] = GenerateTree(start+1, end - 2, LeftNodePos(root));
                return '*';
            }
            //II. We have more nodes so we must split the tree in anoter two
            // Will read the sentance from the end to it's start
            var currentPos = end;
            var current = sentence[currentPos];
            //1. If we have pharantesis, use them to do the split
            if (current == ')')
            {
                var closedPhar = 1;
                currentPos--;
                while (currentPos >= start && closedPhar > 0)
                {
                    
                    if (sentence[currentPos] == ')') closedPhar++;
                    if (sentence[currentPos] == '(') closedPhar--;
                    currentPos--;
                }
                if (closedPhar == 0) //we closed the pharantesis
                {
                    if (currentPos <= start) //Everything was in pharantesis,just ignore them
                        return GenerateTree(start + 1, end - 1,root);

                    //We must have an operation here
                    if (sentence[currentPos].IsSymbol())
                    {
                        Tree[LeftNodePos(root)] = GenerateTree(start, currentPos - 1, LeftNodePos(root));
                        Tree[RightNodePos(root)] = GenerateTree(currentPos + 1, end, RightNodePos(root));
                        return sentence[currentPos];
                    }
                    else throw new Exception("Wrong expression format");
                }
                else throw new Exception("Wrong expression format");
            }

            //2. If we have a letter, will have a single node tree with it and another with the rest
            if (current.isLetter())
            {
                if (sentence[currentPos - 1].IsSymbol())
                {
                    currentPos--;
                    Tree[LeftNodePos(root)] = GenerateTree(start, currentPos - 1, LeftNodePos(root));
                    Tree[RightNodePos(root)] = GenerateTree(currentPos + 1, end, RightNodePos(root));
                    return sentence[currentPos];
                }
                else throw new Exception("Wrong expression format");
            }

            //3. Might also have a kleene symbol, it will come with the rest of the content
            if(current.IsKleene())
            {
                //We can have Kleene after a letter, or after a pharantesis.
                currentPos--;
                if (sentence[currentPos].isLetter())
                {
                    currentPos--;
                    if (sentence[currentPos].IsSymbol())
                    {
                        Tree[LeftNodePos(root)] = GenerateTree(start, currentPos - 1, LeftNodePos(root));
                        
                        Tree[RightNodePos(root)] = GenerateTree(currentPos + 1, end, RightNodePos(root));

                        return sentence[currentPos];
                    }
                    else throw new Exception("Wrong expression format");
                }
                if(sentence[currentPos] == ')')
                {
                    var closedPhar = 1;
                    currentPos--;
                    while (currentPos >= start && closedPhar > 0)
                    {

                        if (sentence[currentPos] == ')') closedPhar++;
                        if (sentence[currentPos] == '(') closedPhar--;
                        currentPos--;
                    }
                    if (closedPhar == 0) //we closed the pharantesis
                    {
                        if (currentPos <= start) //Everything was in pharantesis
                        {
                            Tree[LeftNodePos(root)] = GenerateTree(start, end - 1, LeftNodePos(root));
                            return '*';
                        }
                        //We must have an operation here
                        if (sentence[currentPos].IsSymbol())
                        {
                            Tree[LeftNodePos(root)] = GenerateTree(start, currentPos - 1, LeftNodePos(root));
                            Tree[RightNodePos(root)] = GenerateTree(currentPos + 1, end, RightNodePos(root));
                            return sentence[currentPos];
                        }
                        else throw new Exception("Wrong expression format");
                    }
                    else throw new Exception("Wrong expression format");
                }
                else throw new Exception("Wrong expression format");
            }
            else throw new Exception("Wrong expression format");
            
        }
        public int LeftNodePos(int position)
        {
            return 2 * position + 1;
        }
        public int RightNodePos(int position)
        {
            return 2 * position + 2;
        }
        private void CreateSyntax()
        {
            StringBuilder builder = new StringBuilder();
            foreach (char item in Tree)
                if (item != 0)
                    builder.Append(item);
                else builder.Append(' ');
            TreeString = builder.ToString().TrimEnd().Replace(' ', 'X');
        }
        private void FindLeaves()
        {
            children = new int[Tree.Length];
            for (int i = 0; i < Tree.Length; i++)
            {
                if (LeftNodePos(i) < Tree.Length && Tree[LeftNodePos(i)] != 0)
                    children[i]++;
                if (RightNodePos(i) < Tree.Length && Tree[RightNodePos(i)] != 0)
                    children[i]++;
            }
           
        }
        public bool IsLeaf(int node)
        {
            return (children[node] == 0);
        }
        public bool IsKleene(int node)
        {
            return Tree[node].IsKleene();
        }
        public bool IsConcat(int node)
        {
            return Tree[node].isConcat();
        }
        public bool IsReunion(int node)
        {
            return Tree[node].IsReunion();
        }

    }


}

