using System;
using System.Collections.Generic;

namespace Utils.Extensions
{
    public static class StringExtensions
    {
        public static string Password(this string s, string d)
        {
            var alpha = "abcdefghijklmnopqrstuvwxyz";
            var alphaUpper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var number = "0123456789";
            var special = "!@$%^&*-_+<>?";
            var stringChars = new char[6];
            string r = d;
            bool found = false;
            int maxFind = 5;
            var random = new Random();
            while (!found && maxFind > 0)
            {
                stringChars[0] = alpha[random.Next(alpha.Length)];
                stringChars[1] = alphaUpper[random.Next(alphaUpper.Length)];

                stringChars[2] = number[random.Next(number.Length)];

                stringChars[3] = special[random.Next(special.Length)];

                //stringChars[4] = number[random.Next(number.Length)];

                //stringChars[5] = special[random.Next(special.Length)];

                //stringChars[6] = number[random.Next(number.Length)];

                stringChars[4] = alpha[random.Next(alpha.Length)];
                stringChars[5] = alphaUpper[random.Next(alphaUpper.Length)];

                //stringChars[stringChars.Length-1] = alpha[random.Next(alpha.Length)];
                //for (int i = 1; i < stringChars.Length - 1; i++)
                //{
                //    stringChars[i] = chars[random.Next(chars.Length)];
                //}

                r = new String(stringChars);
                found = !(r.IndexOf(s) > -1);
                maxFind = maxFind - 1;
            }
            if (maxFind == 0) r = r.Replace(s, (random.Next(s.Length)).ToString());
            return r;
        }
        public static string Replace(this string s, IList<string> p, IList<string> r)
        {
            string s1 = s;
            for(var i = 0; i < p.Count; i++)
            {
                s1 = s1.Replace(p[i], r[i]);
            }
            return s1;
        }
        public static string Mobile(this string m)
        {
            if (m.Length > 9)
            {
                if (m.Left(1) == "0")
                    m = "+84" + m.Right(m.Length - 1);
                else
                if (m.Left(2) == "84")
                    m = "+" + m;
                else if (m.Left(3) != "+84")
                    m = "+84" + m;

                if (m.Length == 12) return m;
            }
            return "";
        }

        public static string Left(this string s, int n)
        {
            if (s.Length > n)
            {
                s = s.Substring(0, n);
            }
            return s;
        }
        //public static string Right(this string s, int n, bool IsN)
        //{
        //    if (IsN)
        //    {
        //        s = s.Right(n);
        //    }
        //    else
        //    {
        //        s = s.Substring(n);
        //    }
        //    return s;
        //}
        public static string Right(this string s, int n)
        {
            if (s.Length > n)
            {
                s = s.Substring(s.Length - n, n);
            }
            return s;
        }
    }
}
