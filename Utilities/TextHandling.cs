using System;

namespace Plus.Utilities
{
    public static class TextHandling
    {
        public static int Parse(string a)
        {
            int w = 0, i = 0, length = a.Length, k;

            if (length == 0)
                return 0;

            do
            {
                k = a[i++];
                if (k < 48 || k > 59)
                    return 0;
                w = 10*w + k - 48;
            } while (i < length);

            return w;
        }

        public static string GetString(double k)
        {
            return k.ToString(PlusEnvironment.CultureInfo);
        }
    }
}