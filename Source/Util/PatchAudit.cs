using System.Linq;
using System.Reflection;
using System.Collections;

namespace ParanoidAndroid.Util
{
    internal static class PatchAudit
    {
        public static void LogPatchedMethods(object harmony)
        {
            var builder = new System.Text.StringBuilder();
            builder.AppendLine("[ParanoidAndroid][PatchAudit] Patched method owners:");
            int count = 0;
            var conflicts = new System.Collections.Generic.List<string>();
            var harmonyType = harmony.GetType();
            var getPatchedMethods = harmonyType.GetMethod("GetPatchedMethods", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var getPatchInfo = harmonyType.GetMethod("GetPatchInfo", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (getPatchedMethods == null)
            {
                LogUtil.Debug("PatchAudit", "Harmony reflection failed: GetPatchedMethods not found.");
                return;
            }
            var methodsObj = getPatchedMethods.Invoke(harmony, null) as IEnumerable;
            if (methodsObj == null) return;
            foreach (var m in methodsObj)
            {
                if (m is not MethodBase method) continue;
                count++;
                object? info = getPatchInfo?.Invoke(null, new object[] { method });
                if (info == null) continue;
                var infoType = info.GetType();
                int pc(string fieldName)
                {
                    var f = infoType.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (f == null) return 0;
                    if (f.GetValue(info) is System.Collections.ICollection col) return col.Count;
                    return 0;
                }

                int prefixes = pc("prefixes");
                int postfixes = pc("postfixes");
                int transpilers = pc("transpilers");
                int finalizers = pc("finalizers");
                var ownersProp = infoType.GetProperty("Owners", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var owners = ownersProp?.GetValue(info) as System.Collections.IEnumerable;
                var ownerList = owners?.Cast<object>().Select(o => o?.ToString()).Where(s => !string.IsNullOrWhiteSpace(s)).Cast<string>().ToList() ?? new System.Collections.Generic.List<string>();

                builder.Append(method.DeclaringType?.FullName).Append('.').Append(method.Name);
                if (prefixes != 0) builder.Append($" P:{prefixes}");
                if (postfixes != 0) builder.Append($" Po:{postfixes}");
                if (transpilers != 0) builder.Append($" T:{transpilers}");
                if (finalizers != 0) builder.Append($" F:{finalizers}");
                if (ownerList.Count > 0)
                {
                    builder.Append(" owners=").Append(string.Join(",", ownerList));
                    if (ownerList.Count > 1 && (transpilers > 0 || prefixes > 1))
                        conflicts.Add(method.DeclaringType?.FullName + "." + method.Name + " :: " + string.Join("|", ownerList));
                }
                builder.AppendLine();
            }
            builder.AppendLine($"Total patched methods enumerated: {count}");
            if (conflicts.Count > 0)
            {
                builder.AppendLine("-- Potential Conflicts --");
                foreach (var c in conflicts) builder.AppendLine(c);
            }
            LogUtil.Debug("PatchAudit", builder.ToString());
        }
    }
}
