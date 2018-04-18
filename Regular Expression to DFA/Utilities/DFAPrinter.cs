using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;


namespace Regular_Expression_to_DFA.Utilities
{
    public class DFAPrinter
    {
        private DFA dfa;
        public DFAPrinter(DFA graph)
        {
            dfa = graph;
        }
        public void DrawGraph()
        {
            var transition = dfa.Transitions;
           var  start = dfa.Start;
           var end = dfa.End;

            //create a form
            Form form = new Form();
            //create a viewer object 
            GViewer viewer = new GViewer();
            //create a graph object 
            Graph graph = new Graph("graph");
            //create the graph content 
            for (int i = 0; i < transition.Count; i++)
            {
                var nodeFrom = transition[i].Key.Key;

                var nodeTo = transition[i].Value;
                var edge = graph.AddEdge(nodeFrom, nodeTo);
                edge.LabelText = transition[i].Key.Value.ToString();

            }
            var startNode = graph.FindNode(start);
            startNode.Attr.FillColor = Color.Green;
            startNode.Attr.Shape = Shape.Circle;
           
            foreach(var item in end)
            graph.FindNode(item).Attr.Shape = Shape.DoubleCircle;      

            //bind the graph to the viewer 
            viewer.Graph = graph;

            //associate the viewer with the form 
            form.SuspendLayout();
            viewer.Dock = DockStyle.Fill;
            form.Controls.Add(viewer);
            form.ResumeLayout();

            //show the form 
            form.ShowDialog();
        }
        public void ConsolePrintGraph()
        {            
            Console.WriteLine();
            Console.WriteLine("DFA Descriptors: DFA=(Q, Σ, δ, q0, F)");
            Console.WriteLine(@"Q = {0}", CreateListOfItems<string>(dfa.States));
            Console.WriteLine(@"Σ = {0}", CreateListOfItems<char>(dfa.Alphabet));

            Console.WriteLine("δ = ");
            for (int i = 0; i < dfa.Transitions.Count; i++)
            {
                var nodeFrom = dfa.Transitions[i].Key.Key;
                var nodeTo = dfa.Transitions[i].Value;
                var character = dfa.Transitions[i].Key.Value.ToString();
                Console.WriteLine($"Transition {i}: {nodeFrom},{character} -> {nodeTo}");
            }

            Console.WriteLine($"q0 = {{{dfa.Start}}}");            
            Console.WriteLine("F =  {0} ", CreateListOfItems(dfa.End));
        }

        private string CreateListOfItems<Entity>(List<Entity> items)
        {
            var builder = new StringBuilder();
            builder.Append('{');
            foreach (var item in items) builder.Append(item).Append(" ");
            return builder.ToString().TrimEnd().Replace(' ', ',')+'}';          
        }
    }
}
