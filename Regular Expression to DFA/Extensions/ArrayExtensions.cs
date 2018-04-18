using Regular_Expression_to_DFA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regular_Expression_to_DFA.Extensions
{
    public static class ArrayExtensions
    {
        public static int[] EmptyArray()
        {
            var list = new int[0];
            return list;
        }
        public static Node[] EmptyNodeArray()
        {
            var list = new Node[0];
            return list;
        }
        public static int[] SingleElementArray(int element)
        {
            var list = new int[1];
            list[0] = element;
            return list;
        }
        public static Node[] SingleElementArray(Node element)
        {
            var list = new Node[1];
            list[0] = element;
            return list;
        }
        
        public static int[] Reunion(this int[] x, int[] y)
        {
            if (x == null) return y;
            if (y == null) return x;

            var union = new int[x.Length + y.Length];
            int k = 0;
            for (int i = 0; i < x.Length; i++)
                union[k++] = x[i];
            for (int j = 0; j < y.Length; j++)
                union[k++] = y[j];
            return union;
        }
        public static Node[] Reunion(this Node[] x, Node[] y)
        {
            if (x == null) return y;
            if (y == null) return x;

            var union = new Node[x.Length + y.Length];
            int k = 0;
            for (int i = 0; i < x.Length; i++)
                union[k++] = x[i];
            for (int j = 0; j < y.Length; j++)
                union[k++] = y[j];
            return union;
        }
        public static int[] DistinctReunion(this int[] x, int[] y)
        {
            if (x == null) return y;
            if (y == null) return x;
            Array.Sort(x);
            Array.Sort(y);
           
            var union = new int[x.Length + y.Length];
            int k = 0;
            int i = 0;
            int j = 0;
            while( i < x.Length &&j< y.Length)
            {
                if(x[i]<y[j])
                    union[k++] = x[i++];
                if (x[i] > y[j])
                    union[k++] = y[j++];
                else
                {
                    union[k++] = x[i++];
                    j++;
                }
            }
            for (; i < x.Length; i++)
                union[k++] = x[i];
            for (; j < y.Length; j++)
                union[k++] = y[j];
            return union;
        }
        public static string ListToSetString(this int[] list)
        {
            if (list == null) return null;
            var buildList = new StringBuilder();
            foreach (var item in list)
                buildList.Append(item).Append(" ");
            var stringList = "{" + buildList.ToString().TrimEnd() + "}";
            return stringList;
        }
        public static string ListToSetString(this Node[] list)
        {
            if (list == null) return null;
            var buildList = new StringBuilder();
            foreach (var item in list)
                buildList.Append(item.Index).Append(" ");
            var stringList = "{" + buildList.ToString().TrimEnd() + "}";
            return stringList;
        }
    }
}
