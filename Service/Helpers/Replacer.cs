using System;
using System.Linq;

namespace Service.Helpers
{
    public static class Replacer
    {
        private static char[] specialCharacters = new char[] { '*', '?', '*', '(', ')', '/', ':', '.', ',', ' ', '\'' };

        public static string CorrentAlbum(string title)
        {
            switch (title)
            {
                case "H.A.A.R.P":
                    return "HAARP";
            }
            return title;
        }

        public static string RemoveSpecialCharacters(string str)
        {
            foreach (var c in specialCharacters)
            {
                str = str.Replace(c, '_');
            }
            return str;
        }
    }
}
