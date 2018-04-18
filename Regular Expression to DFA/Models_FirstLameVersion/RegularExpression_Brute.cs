using Regular_Expression_to_DFA.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regular_Expression_to_DFA
{

    public class RegularExpression_Brute
    {
        public List<char> Alphabet = new List<char>();
        public string Regex;        
        public TreeExpression_Brute SyntaxTree;
       
        public int[][] Firstpos;
        public int[][] Lastpos;
        public int[][] Followpos;

        public RegularExpression_Brute(string input)
        {
            Regex = input;
            SetAlphabet();

            SyntaxTree = new TreeExpression_Brute(input);
            CreateFollowGraph();
        }
        private void SetAlphabet()
        {
            var localAlphabet = new HashSet<char>();
            foreach (var item in Regex.ToCharArray()) if(item.isLetter()) localAlphabet.Add(item);
            foreach (var item in localAlphabet) Alphabet.Add(item);
        }
        public void CreateFollowGraph()
        {
            var length = SyntaxTree.Tree.Length;            
            Firstpos = new int[length][];
            Lastpos = new int[length][];
            Followpos = new int[length][];
            for (int i = length - 1; i >= 0; i--)
            {
                var value = SyntaxTree.Tree[i];
                if (value != 'X' && value != 0)
                {
                     Firstpos[i] = GetFirstpos(i);
                    Lastpos[i] = GetLastpos(i);
                }
            }
            for (int i = length - 1; i >= 0; i--)
            {
                var value = SyntaxTree.Tree[i];
                if (value.IsKleene() || value.isConcat())                
                    GetFollowpos(i);                
            }
        }
        private bool Nullable(int node)
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
        private int[] GetFirstpos(int node)
        {
            if (SyntaxTree.IsLeaf(node))               
                  return ArrayExtensions.SingleElementArray(node);

            var c1 = SyntaxTree.LeftNodePos(node);
            var c2 = (SyntaxTree.RightNodePos(node));

            if (SyntaxTree.IsConcat(node))          
                return Nullable(c1) ? GetFirstpos(c1).Reunion(GetFirstpos(c2)) : GetFirstpos(c1);                
            

            if (SyntaxTree.IsReunion(node))     
                return GetFirstpos(c1).Reunion( GetFirstpos(c2));


            if (SyntaxTree.IsKleene(node)) return GetFirstpos(c1);

            return ArrayExtensions.EmptyArray();

        }
        private int[] GetLastpos(int node)
        {
            if (SyntaxTree.IsLeaf(node))   
                 return ArrayExtensions.SingleElementArray(node);
                
            var c1 = SyntaxTree.LeftNodePos(node);
            var c2 = SyntaxTree.RightNodePos(node);

            if (SyntaxTree.IsConcat(node))
                return Nullable(c2) ? GetLastpos(c1).Reunion(GetLastpos(c2)) : GetLastpos(c2);


            if (SyntaxTree.IsReunion(node))
                return GetLastpos(c1).Reunion(GetLastpos(c2));


            if (SyntaxTree.IsKleene(node)) return GetLastpos(c1);

            return ArrayExtensions.EmptyArray();

        }
        private void GetFollowpos(int node)
        {
            
            var c1 = SyntaxTree.LeftNodePos(node);
            var c2 = SyntaxTree.RightNodePos(node);

            if (SyntaxTree.IsConcat(node))
            {
                var codes = Lastpos[c1]; 
                foreach(var code in codes)
                Followpos[code] = Followpos[code].Reunion(Firstpos[c2]);
            }

            if (SyntaxTree.IsKleene(node))
            {
                var codes = Lastpos[node];
                foreach (var code in codes)
                        Followpos[code] = (Followpos[code]).Reunion(Firstpos[node]);
            }
                       
        }      
    }

}
