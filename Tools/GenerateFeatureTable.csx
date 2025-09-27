// Usage: dotnet script Tools/GenerateFeatureTable.csx (requires dotnet-script) OR compile into a temporary runner.
// Scans Source/ for feature annotations and rewrites DLC_DEPENDENCIES.md section between markers.

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

// Simple attribute convention (not enforced by compiler yet):
// [FeatureTag("Name","DLC|None","Namespace.Type.Method","Fallback","Notes")]

var root = Directory.GetCurrentDirectory();
var sourceDir = Path.Combine(root, "Source");
var dlcFile = Path.Combine(root, "DLC_DEPENDENCIES.md");

// Pattern explanation:
// [FeatureTag("Name","Req","Entry","Fallback","Notes")]
// Simpler capture groups (1..5) to avoid named group escaping issues in script context.
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
            string esc(string s) => string.IsNullOrWhiteSpace(s) ? "-" : s.Replace("|", "/");
            var name = esc(m.Groups[1].Value);
            var req = esc(m.Groups[2].Value);
            var entry = esc(m.Groups[3].Value);
            var fallback = esc(m.Groups[4].Value);
            var notes = esc(m.Groups[5].Value);
            rows.Add($"| {name} | {req} | {entry} | {fallback} | {notes} |");
        }
    }
}

if (!File.Exists(dlcFile))
{
    Console.Error.WriteLine("DLC_DEPENDENCIES.md not found.");
    return;
}

var md = File.ReadAllLines(dlcFile).ToList();
var begin = md.FindIndex(l => l.Contains("<!-- AUTO-FEATURE-ROWS:BEGIN -->"));
var end = md.FindIndex(l => l.Contains("<!-- AUTO-FEATURE-ROWS:END -->"));
if (begin == -1 || end == -1 || end <= begin)
{
    Console.Error.WriteLine("Markers not found or malformed.");
    return;
}

// Preserve first line after BEGIN (existing example row) only if rows are empty.
var headerSegment = md.GetRange(0, begin + 1);
var tailSegment = md.GetRange(end, md.Count - end);

var newBlock = new List<string>();
if (rows.Count == 0)
{
    // Keep existing content between markers (do nothing)
    Console.WriteLine("No feature annotations found; leaving existing rows.");
    return;
}
else
{
    foreach (var r in rows.OrderBy(r => r)) newBlock.Add(r);
}

var rebuilt = new List<string>();
rebuilt.AddRange(headerSegment);
rebuilt.AddRange(newBlock);
rebuilt.AddRange(tailSegment);

File.WriteAllText(dlcFile, string.Join(Environment.NewLine, rebuilt) + Environment.NewLine, Encoding.UTF8);
Console.WriteLine($"Updated feature table with {rows.Count} entries.");