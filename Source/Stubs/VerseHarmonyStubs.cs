#if STUB_VERSE || STUB_HARMONY || REFERENCE_STUBS
// Minimal stub definitions to allow compilation without actual RimWorld / Harmony assemblies.
// These are excluded when real assemblies are present. DO NOT implement real logic here.
namespace Verse {
    public interface IExposable { void ExposeData(); }
    public class Mod { public Mod(ModContentPack content) {} public virtual string SettingsCategory() => "Stub"; public virtual void DoSettingsWindowContents(object rect){} protected T GetSettings<T>() where T:new()=> new T(); }
    public class ModContentPack {}
    public class ModSettings { public virtual void ExposeData(){} public void Write(){} }
    public class Listing_Standard { public void Begin(object r){} public void CheckboxLabeled(string lbl, ref bool val, string? tip=null){} public void End(){} }
    public static class Scribe_Values { public static void Look(ref bool v, string name, bool def){} public static void Look(ref int v,string name,int def){} public static void Look(ref string? v,string name,string? def){} }
    public static class Scribe { public static LoadSaveMode mode => LoadSaveMode.PostLoadInit; }
    public enum LoadSaveMode { PostLoadInit }
    public static class Log { public static void Message(string m){} public static void Warning(string m){} public static void Error(string m){} }
    public class Pawn { public bool Spawned => false; public bool Destroyed => false; public object? Map => null; public string LabelShort => "pawn"; public bool IsHashIntervalTick(int i)=>false; public void Tick(){} }
    public class Map { public int uniqueID => 0; }
    public class StaticConstructorOnStartupAttribute : System.Attribute {}
}
namespace UnityEngine {
    public struct Rect { public float x,y,width,height; public Rect(float x,float y,float w,float h){ this.x=x; this.y=y; this.width=w; this.height=h; } }
}
namespace HarmonyLib {
    public class HarmonyPatchAttribute : System.Attribute { public HarmonyPatchAttribute(System.Type t,string methodName){} }
    public class Harmony { public Harmony(string id){} public void PatchAll(System.Reflection.Assembly asm){} public System.Collections.Generic.IEnumerable<System.Reflection.MethodBase> GetPatchedMethods(){ yield break; } }
}
#endif
