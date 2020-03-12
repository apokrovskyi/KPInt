using System;
using System.Linq;

namespace KPInt_Server
{
    static class RandomNameGen
    {
        private static Random random = new Random();

        private const string vowels = "eyuioa";

        public static string GetName()
        {
            var len = (int)(6 + random.NextDouble() * 4 * (random.NextDouble() > 0.5 ? 1 : -1));

            var res = new char[len];

            int cons = 0;

            for (int i = 0; i < len; i++)
            {
                var val = (int)(random.NextDouble() * 26);
                if (val == 26)
                    res[i] = '\'';
                else
                    res[i] = (char)('a' + val);

                if (vowels.Contains(res[i]))
                    cons = 0;
                else
                    cons++;

                if (cons > 3)
                {
                    i--;
                    continue;
                }

                if (i == 0) res[i] = char.ToUpper(res[i]);
            }

            return new string(res);
        }
    }
}
