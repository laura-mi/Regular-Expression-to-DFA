using Regular_Expression_to_DFA.Models;
using Regular_Expression_to_DFA.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regular_Expression_to_DFA
{
    /// <summary>
    /// Regular expression with a syntactic tree associated
    /// </summary>
    public class RegularExpression
    {
        public List<char> Alphabet = new List<char>();
        public string Regex;        
        public TreeExpression SyntaxTree;
       
        public Dictionary<Node, Node[]> Firstpos = new Dictionary<Node, Node[]>();
        public Dictionary<Node, Node[]> Lastpos = new Dictionary<Node, Node[]>();
        public Dictionary<Node, Node[]> Followpos = new Dictionary<Node, Node[]>();

        public RegularExpression(string input)
        {
            Regex = input;
            SetAlphabet();
            SyntaxTree = new TreeExpression(input);
            CreateFollowGraph();
        }
        /// <summary>
        /// Create a list with the letters from the regex
        /// </summary>
        private void SetAlphabet()
        {
            var localAlphabet = new HashSet<char>();
            foreach (var item in Regex.ToCharArray()) if(item.isLetter()) localAlphabet.Add(item);
            foreach (var item in localAlphabet) Alphabet.Add(item);
        }

        public void CreateFollowGraph()
        {          
            var start = SyntaxTree.Root;
            CreateStartFinalPos(start);
            CreateFollowPos(start);         
        }
        private bool Nullable(Node node)
        {
            if (SyntaxTree.IsLeaf(node))
                return false;

            if (SyntaxTree.IsConcat(node))
                return Nullable(SyntaxTree.LeftNodePos(node)) && Nullable(SyntaxTree.RightNodePos(node));

            if(SyntaxTree.IsReunion(node))
                return Nullable(SyntaxTree.LeftNodePos(node)) || Nullable(SyntaxTree.RightNodePos(node));

            if (SyntaxTree.IsKleene(node)) return true;

            return false;
        }
        private Node[] GetFirstpos(Node node)
        {
            if (SyntaxTree.IsLeaf(node))               
                  return ArrayExtensions.SingleElementArray(node);

            var c1 = SyntaxTree.LeftNodePos(node);
            var c2 = (SyntaxTree.RightNodePos(node));

            if (SyntaxTree.IsConcat(node))          
                return Nullable(c1) ? Firstpos[c1].Reunion(Firstpos[c2]) : Firstpos[c1];                
            

            if (SyntaxTree.IsReunion(node))     
                return Firstpos[c1].Reunion( Firstpos[c2]);


            if (SyntaxTree.IsKleene(node)) return Firstpos[c1];

            return ArrayExtensions.EmptyNodeArray();

        }
        private Node[] GetLastpos(Node node)
        {
            if (SyntaxTree.IsLeaf(node))   
                 return ArrayExtensions.SingleElementArray(node);
                
            var c1 = SyntaxTree.LeftNodePos(node);
            var c2 = SyntaxTree.RightNodePos(node);

            if (SyntaxTree.IsConcat(node))
                return Nullable(c2) ? Lastpos[c1].Reunion(Lastpos[c2]) : Lastpos[c2];


            if (SyntaxTree.IsReunion(node))
                return Lastpos[c1].Reunion(Lastpos[c2]);


            if (SyntaxTree.IsKleene(node)) return Lastpos[c1];

            return ArrayExtensions.EmptyNodeArray();
        }

        /// <summary>
        /// Create the StartPos and FinalPos arrays
        /// </summary>
        /// <param name="node"></param>
        public void CreateStartFinalPos(Node node)
        {
            if (node != null)
            {
                CreateStartFinalPos(node.Left);
                CreateStartFinalPos(node.Right);

                if (node.Value != '\0')
                {
                    var result = GetFirstpos(node);
                    Firstpos.Add(node, result);

                    result = GetLastpos(node);
                    Lastpos.Add(node, result);
                }    
            }
        }
        
        /// <summary>
        /// Create the followpos array
        /// </summary>
        /// <param name="node"></param>
        private void CreateFollowPos(Node node)
        {
            if (node != null)
            {
                var c1 = SyntaxTree.LeftNodePos(node);
                var c2 = SyntaxTree.RightNodePos(node);

                if (SyntaxTree.IsConcat(node))
                {
                    var codes = Lastpos[c1];
                    foreach (var code in codes)
                    {
                        if (!Followpos.ContainsKey(code))
                            Followpos.Add(code, ArrayExtensions.EmptyNodeArray());
                        Followpos[code] = Followpos[code].Reunion(Firstpos[c2]);
                    }
                }

                if (SyntaxTree.IsKleene(node))
                {
                    var codes = Lastpos[node];
                    foreach (var code in codes)
                    {
                        Followpos[code] = (Followpos[code]).Reunion(Firstpos[node]);
                        if (!Followpos.ContainsKey(code))
                            Followpos.Add(code, ArrayExtensions.EmptyNodeArray());

                    }
                }
                CreateFollowPos(node.Left);
                CreateFollowPos(node.Right);

            }
        }
    }

}
