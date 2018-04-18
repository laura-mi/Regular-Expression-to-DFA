using Regular_Expression_to_DFA.Extensions;
using Regular_Expression_to_DFA.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regular_Expression_to_DFA.Models
{
   public class TreeExpression
    {
        public Node Root;
        private Stack<char> nodesStack = new Stack<char>();
        public TreeExpression(string input)
        {
            var infix = RegexUtilities.AddConcatenationSymbol(input.ToCharArray());
            var postfix = InfixToPostfixExpression(infix);
            var root = ParsePostfix(postfix);
            Root = new Node('.',0, root, new Node('#',LeftNodePos(0)));            
            AssignNumbersToNodes(Root, 0);         
        }
      
        /// <summary>
        /// Assign an index to each node
        /// </summary>
        /// <param name="node"></param>
        /// <param name="number"></param>
        private void AssignNumbersToNodes(Node node,int number)
        {
            if (node != null)
            {
                node.Index = number;
                AssignNumbersToNodes(node.Left, LeftNodePos(number));
                AssignNumbersToNodes(node.Right, RightNodePos(number));
            }
        }
    
        /// <summary>
        /// Transform a binary tree from the tree structure into a vectorial one. Flatten the tree
        /// I'm no longer using this method 
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        private char NodesToList(Node tree)
        {
            char current = '\0';
            if (tree != null)
            {
                current = tree.Value;
                var left = NodesToList(tree.Left);               
                var right = NodesToList(tree.Right);
                
                nodesStack.Push(right);
                nodesStack.Push(left);
            }
            else
            {
                nodesStack.Push('\0');
                nodesStack.Push('\0');
            }              
            return current;         

        }
        /// <summary>
        /// Parse a postfix regex in order to create a binary tree of it
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private Node ParsePostfix(char[] input)
        {
            var stack = new Stack<Node>();
            for(int i=0;i<input.Length;i++)
            {
                var current = input[i];

                if (current.isLetter())
                    stack.Push(new Node(current));
                else if (current.IsKleene())
                {
                    var c = stack.Pop();
                    var node = new Node(current, c);
                    stack.Push(node);
                }
                else if (current.IsReunion() || current.isConcat())
                {                    
                    var c2 = stack.Pop();
                    var c1 = stack.Pop();
                    var node = new Node(current, c1, c2);
                    stack.Push(node);
                }
                else throw new Exception("Wrong format!");
            }
            return stack.Peek();
        }

        /// <summary>
        /// Turn an infix regex into a postfix one
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private char[] InfixToPostfixExpression(char[] input)
        {
            var stack = new Stack<char>();
            var rezidual = new Stack<char>();
            foreach(var item in input)
            {
                if (item == '(') stack.Push(item);
                else                
                    if (item == ')') 
                    {
                        while (stack.Peek() != '(')
                            rezidual.Push(stack.Pop());
                        stack.Pop(); //remove the '('
                    }
                    else
                    {
                        while (stack.Count > 0)
                        {
                            var current = stack.Peek();

                            var itemPriority = GetPriority(item);
                            var currentPriority = GetPriority(current);

                            if (currentPriority >= itemPriority )
                            {
                                rezidual.Push(stack.Pop());
                            }
                            else
                            {
                                break;
                            }
                        }
                        stack.Push(item);
                    }
                
            }
            while (stack.Count>0)
                rezidual.Push(stack.Pop());

            while (rezidual.Count > 0)
                stack.Push(rezidual.Pop());
            var result = new List<char>();
            foreach (var item in stack)
                result.Add(item);


            return result.ToArray();

        }
        private int GetPriority(char item)
        {
            if (item == '(') return 1;
            if (item.isConcat()) return 3;
            if (item.IsReunion()) return 2;
            if (item.IsKleene()) return 4;
            return 5;
        }
            
        public int LeftNodePos(int position)
        {
            return 2 * position + 1;
        }
        public int RightNodePos(int position)
        {
            return 2 * position + 2;
        }

        public bool IsLeaf(Node node)
        {
            return (node.Left == null && node.Right==null);
        }
        public bool IsKleene(Node node)
        {
            return node.Value.IsKleene();
        }
        public bool IsConcat(Node node)
        {
            return node.Value.isConcat();
        }
        public bool IsReunion(Node node)
        {
            return node.Value.IsReunion();
        }
        public Node LeftNodePos(Node position)
        {
            return position.Left;
        }
        public Node RightNodePos(Node position)
        {
            return position.Right;
        }
    }
}
