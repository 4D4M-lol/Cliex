namespace Cliex
{
    public static class Variables
    {
        private static Dictionary<string, object?> Storage => new();

        public static void SetVariable(string name, object? value, bool update = true)
        {
            if (Storage.ContainsKey(name) && !update) return;

            Storage[name] = value;
        }

        public static object? GetVariable(string name, object? def = null)
        {
            return Storage[name] ?? def;
        }

        public static TValue? GetVariable<TValue>(string name, TValue? def = default)
        {
            if (Storage[name] is TValue value) return value;

            return def;
        }

        public static bool HasVariable(string name)
        {
            return Storage.ContainsKey(name);
        }
    }
    
    
}