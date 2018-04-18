using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regular_Expression_to_DFA.Models
{
    public class Node: IComparable
    {
        public char Value { get; set; }
        public int Index { get; set; }
        public Node Left { get; set; }
        public Node Right { get; set; }

        public Node(char value,int index, Node left, Node right)
        {
            Value = value;
            Index = index;
            Left = left;
            Right = right;
        }
        public Node(char value, int index)
        {
            Value = value;
            Index = index;
            Left = null;
            Right = null;
        }
        public Node(char value, int index,Node left)
        {
            Value = value;
            Left = left;
            Right = null;
        }
        public Node(char value, Node left, Node right)
        {
            Value = value;
            Left = left;
            Right = right;
        }
        public Node(char value)
        {
            Value = value;
            Left = null;
            Right = null;
        }
        public Node(char value, Node left)
        {
            Value = value;
            Left = left;
            Right = null;
        }

        public int CompareTo(object obj)
        {
            var node = obj as Node;
            if (node != null)
                return Index.CompareTo(node.Index);

            else return Index.CompareTo(obj);
        }
    }
}
