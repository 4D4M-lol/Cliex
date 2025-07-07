namespace Cliex
{
    public static class Variables
    {
        private static Dictionary<string, object?> Storage = new();

        public static void SetVariable(string name, object? value, bool update = true)
        {
            if (Storage.ContainsKey(name) && !update) return;

            Storage[name.Replace(' ', '-')] = value;
        }

        public static object? GetVariable(string name, object? def = null)
        {
            Storage.TryGetValue(name, out object? value);

            return value ?? def;
        }

        public static TValue? GetVariable<TValue>(string name, TValue? def = default)
        {
            if (Storage.TryGetValue(name, out object? value) is TValue ret) return ret;

            return def;
        }

        public static bool HasVariable(string name)
        {
            return Storage.ContainsKey(name.Replace(' ', '-'));
        }
    }

}