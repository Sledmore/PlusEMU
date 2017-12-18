namespace Plus.Utilities
{
    public static class TextHandling
    {
        public static string GetString(double k)
        {
            return k.ToString(PlusEnvironment.CultureInfo);
        }
    }
}