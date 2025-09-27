using System.Text.RegularExpressions;
using System.Text;

// Simple console version of the previous script; invoked via VS Code task.
// Scans Source/ for [FeatureTag(...)] attributes and updates DLC_DEPENDENCIES.md between markers.

var root = Directory.GetCurrentDirectory();
// When run from solution root or this project directory, normalize to solution root.
if (Path.GetFileName(root).Equals("FeatureDocGen", StringComparison.OrdinalIgnoreCase))
    root = Path.GetFullPath(Path.Combine(root, "..", ".."));

string sourceDir = Path.Combine(root, "Source");
string mdFile = Path.Combine(root, "DLC_DEPENDENCIES.md");
if (!File.Exists(mdFile))
{
    Console.Error.WriteLine("DLC_DEPENDENCIES.md not found (expected at root). Aborting.");
    return 2;
}

var featureRegex = new Regex(@"\[FeatureTag\(""([^""]+)"",""([^""]*)"",""([^""]*)"",""([^""]*)"",""([^""]*)""\)\]", RegexOptions.Compiled);
var rows = new List<string>();

if (Directory.Exists(sourceDir))
{
    foreach (var file in Directory.EnumerateFiles(sourceDir, "*.cs", SearchOption.AllDirectories))
    {
        var text = File.ReadAllText(file);
        foreach (Match m in featureRegex.Matches(text))
        {
            if (!m.Success || m.Groups.Count < 6) continue;
            string Esc(string s) => string.IsNullOrWhiteSpace(s) ? "-" : s.Replace("|", "/");
            var name = Esc(m.Groups[1].Value);
            var req = Esc(m.Groups[2].Value);
            var entry = Esc(m.Groups[3].Value);
            var fallback = Esc(m.Groups[4].Value);
            var notes = Esc(m.Groups[5].Value);
            rows.Add($"| {name} | {req} | {entry} | {fallback} | {notes} |");
        }
    }
}

if (rows.Count == 0)
{
    Console.WriteLine("No FeatureTag attributes found. Leaving file unchanged.");
    return 0;
}

var lines = File.ReadAllLines(mdFile).ToList();
int begin = lines.FindIndex(l => l.Contains("<!-- AUTO-FEATURE-ROWS:BEGIN -->"));
int end = lines.FindIndex(l => l.Contains("<!-- AUTO-FEATURE-ROWS:END -->"));
if (begin == -1 || end == -1 || end <= begin)
{
    Console.Error.WriteLine("Marker block not found or malformed. Aborting.");
    return 3;
}

var header = lines.Take(begin + 1).ToList();
var tail = lines.Skip(end).ToList();
var newTable = rows.Distinct().OrderBy(r => r, StringComparer.OrdinalIgnoreCase).ToList();

var rebuilt = new List<string>();
rebuilt.AddRange(header);
rebuilt.AddRange(newTable);
rebuilt.AddRange(tail);
File.WriteAllText(mdFile, string.Join(Environment.NewLine, rebuilt) + Environment.NewLine, Encoding.UTF8);
Console.WriteLine($"Updated DLC_DEPENDENCIES.md with {newTable.Count} feature row(s).");
return 0;
