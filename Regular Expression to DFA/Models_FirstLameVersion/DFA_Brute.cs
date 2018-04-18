using Regular_Expression_to_DFA.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regular_Expression_to_DFA
{
    public class DFA_Brute
    {
        public RegularExpression_Brute Expression;        
        public int[] StartState;
        public int FinalState;
        public List<int[]> States = new List<int[]>();
        public List<KeyValuePair<KeyValuePair<string, char>, string>> Transitions
            = new List<KeyValuePair<KeyValuePair<string, char>, string>>();
        public string Start;
        public List<string> End = new List<string>();
        
        public DFA_Brute(RegularExpression_Brute exp)
        {
            Expression = exp;            
            var expressionTree = Expression.SyntaxTree;
            //start state of Dfa is firstpos(n0), where n0 is the root of Tree           
            StartState = Expression.Firstpos[expressionTree.Root];
            //accepting states = those containing the # endmarker symbol
            FinalState = expressionTree.RightNodePos(expressionTree.Root);

            CreateTransitions();
          RenameStates();
          
        }

        private void CreateTransitions()
        {
            var markedStates = new HashSet<int[]>();
            //initialize states to contain only the unmarked
            // state firstpos(n0), where n0 is the root of syntax tree T for (r)#;
            
            States.Add(StartState); 
            int k = 0;
            
            while (States.Count > markedStates.Count)//there is an unmarked state S in states
            {
                var currentState = States[k];
                //mark S;                
                markedStates.Add(currentState);
                foreach(var inputSymbol in Expression.Alphabet)
                {
                    //a = inputSymbol
                    //let U be the union of followpos(p) for all p in S that correspond to a;
                    int[] union = null;
                    foreach (var component in currentState )
                    {                        
                        if(Expression.SyntaxTree.Tree[component] == inputSymbol)
                        {
                            int[] follows = Expression.Followpos[component];
                            union = union.Reunion(follows);
                        }
                    }
                    //if(U is not in Dstates)
                    // add U as unmarked state to Dstates;
                    if (union != null && union.Length >= 1)
                    {
                        Array.Sort(union);
                        if (!ContainsState(union))
                            States.Add(union);
                        // tranition[S, a] = U;
                        var fromTransition = new KeyValuePair<string, char>(currentState.ListToSetString(), inputSymbol);
                        var transition = new KeyValuePair<KeyValuePair<string, char>, string>
                            (fromTransition, union.ListToSetString());
                        Transitions.Add(transition);
                    }         
                }
                k++;
            }
        }

      
        private bool ContainsState(int[] state)
        {
            if (state == null) throw new Exception("State can't be null");
            foreach (var item in States)
            {
                if(item.Length == state.Length)
                {
                    bool contains = true;
                    int i = 0;
                    while (contains && i<state.Length)
                        if (item[i] != state[i++]) contains = false;
                    if (contains == true)
                        return true;
                }
            }
            return false;
        }   

        private void RenameStates()
        {
            var dictionary = new Dictionary<string, string>();
            int k = 0;
            foreach(var item in Transitions)
            {
                var from = item.Key.Key;
                var to = item.Value;
                if (!dictionary.ContainsKey(from))
                    dictionary.Add(from, (k++).ToString());
                if (!dictionary.ContainsKey(to))
                    dictionary.Add(to, (k++).ToString());         
            }
            var renamedTransitions = new List<KeyValuePair<KeyValuePair<string, char>, string>>();
            foreach(var item in Transitions)
            {
                var from = new KeyValuePair<string, char>(dictionary[item.Key.Key], item.Key.Value);
                var transition = new KeyValuePair<KeyValuePair<string, char>, string>(from, dictionary[item.Value]);
                renamedTransitions.Add(transition);
            }
            Transitions = renamedTransitions;
            Start = dictionary[StartState.ListToSetString()];

            List<int[]> final = new List<int[]>();
            foreach(var item in States)
            {
                if (item != null)
                    if (item.Contains(FinalState))
                    {
                        final.Add(item);
                        End.Add(dictionary[item.ListToSetString()]);
                    }
            } 
            
        }
    }
}
