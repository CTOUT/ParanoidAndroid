using System;

namespace ParanoidAndroid.Util
{
    /// <summary>
    /// Annotate feature entry points to enable automatic documentation table generation (see Tools/GenerateFeatureTable.csx).
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    internal sealed class FeatureTagAttribute : Attribute
    {
        public string Name { get; }
        public string Requires { get; } // e.g. "Royalty" or "Biotech|Ideology" or "None"
        public string EntryPoint { get; }
        public string Fallback { get; }
        public string Notes { get; }

        public FeatureTagAttribute(string name, string requires, string entryPoint, string fallback, string notes)
        {
            Name = name;
            Requires = requires;
            EntryPoint = entryPoint;
            Fallback = fallback;
            Notes = notes;
        }
    }
}
