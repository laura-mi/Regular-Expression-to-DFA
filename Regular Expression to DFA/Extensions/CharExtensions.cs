using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regular_Expression_to_DFA.Extensions
{
    public static class CharExtensions
    {
        public static bool IsSymbol(this char item)
        {
            return (item.isConcat() || item.IsKleene() || item.IsReunion());
        }
        public static bool IsReunion(this char item)
        {
            return (item == '|');
        }
        public static bool IsKleene(this char item)
        {
            return (item == '*');
        }
        public static bool isConcat(this char item)
        {
            return (item == '.');
        }
        public static bool isLetter(this char item)
        {
            return (item >= 'a' && item <= 'z');
        }
    }
}
