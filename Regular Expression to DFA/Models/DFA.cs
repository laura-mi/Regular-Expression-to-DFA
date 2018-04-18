using Regular_Expression_to_DFA.Models;
using Regular_Expression_to_DFA.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regular_Expression_to_DFA
{
    public class DFA
    {
        public RegularExpression Expression;        
        private Node[] StartState;
        private Node FinalState;

        //DFA descriptors
        //Un AFD este este un tuplu A=(Q, Σ, δ, q0, F), in care:
        //Q este o multime finita de stari
        private List<Node[]> states = new List<Node[]>();
        public List<string> States = new List<string>();

        // Σ este o multime finita de simboli de intrare(alfabetul simbolilor de intrare)
        public List<char> Alphabet;

        //δ este o functie de tranzitie: δ:Q × Σ-> Q
        //Transitions
        public List<KeyValuePair<KeyValuePair<string, char>, string>> Transitions
            = new List<KeyValuePair<KeyValuePair<string, char>, string>>();

        //q0 este starea initiala
        //F este o multime de stari finale.
        //Start and Final states
        public string Start;
        public List<string> End = new List<string>();
     
        public DFA(RegularExpression exp)
        {
            Expression = exp;
            Alphabet = exp.Alphabet;
            var expressionTree = Expression.SyntaxTree;
            //start state of Dfa is firstpos(n0), where n0 is the root of Tree           
            StartState = Expression.Firstpos[expressionTree.Root];
            //accepting states = those containing the # endmarker symbol
            FinalState = expressionTree.RightNodePos(expressionTree.Root);

            CreateTransitions();
            RenameStates();
          
        }

        /// <summary>
        /// Creates the list with all the DFA's transitions
        /// </summary>
        private void CreateTransitions()
        {
            var markedStates = new HashSet<Node[]>();
            //initialize states to contain only the unmarked
            // state firstpos(n0), where n0 is the root of syntax tree T for (r)#;
            Array.Sort<Node>(StartState);
            states.Add(StartState);
          
            int k = 0;
            
            while (states.Count > markedStates.Count)//there is an unmarked state S in states
            {
                var currentState = states[k];
                //mark S;                
                markedStates.Add(currentState);
                foreach(var inputSymbol in Expression.Alphabet)
                {
                    //a = inputSymbol
                    //let U be the union of followpos(p) for all p in S that correspond to a;
                    Node[] union = null;
                    foreach (var component in currentState )
                    {                        
                        if(component.Value == inputSymbol)
                        {
                            var follows = Expression.Followpos[component];
                            union = union.Reunion(follows);
                        }
                    }
                    //if(U is not in Dstates)
                    // add U as unmarked state to Dstates;
                    if (union != null && union.Length >= 1)
                    {
                        //  union = union.Sort();
                        Array.Sort<Node>(union);
                        if (!ContainsState(union))
                            states.Add(union);
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
              
        /// <summary>
        /// Checks if an array of nodes contains a specific state
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private bool ContainsState(Node[] state)
        {
            if (state == null) throw new Exception("State can't be null");
            foreach (var item in states)
            {
                if(item.Length == state.Length)
                {
                    bool contains = true;
                    int i = 0;
                    while (contains && i<state.Length)
                        if (item[i].Index != state[i++].Index) contains = false;
                    if (contains == true)
                        return true;
                }
            }
            return false;
        }   

        /// <summary>
        /// Rename states for a better readability of the graph
        /// </summary>
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

            List<Node[]> final = new List<Node[]>();
            foreach(var item in states)
            {
                if (item != null)
                {
                    if (item.Contains(FinalState))
                    {
                        final.Add(item);
                        End.Add(dictionary[item.ListToSetString()]);
                    }
                    States.Add(dictionary[item.ListToSetString()]);
                }
            } 
            
        }
    }
}
