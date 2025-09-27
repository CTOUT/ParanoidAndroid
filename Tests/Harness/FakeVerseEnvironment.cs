// Exclude the test harness when building with reference stubs or when Verse is not actually available.
#if HAVE_VERSE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using ParanoidAndroid.Util;
using Xunit;

namespace ParanoidAndroid.Tests.Harness
{
    // We simulate enough of Verse's mod metadata surface for reflection-based ModDependency.
    internal class FakeModMetaData
    {
        public string? PackageIdLowerCase { get; set; }
    }

    internal static class FakeLoadedModManager
    {
        public static List<FakeModMetaData> RunningModsListForReading { get; } = new();
    }

    public class ModDependencyHarnessTests
    {
        private void InjectFakeVerse(params string[] packageIds)
        {
            // Build a dynamic assembly that contains types Verse.LoadedModManager and Verse.ModMetaData shape-like.
            // For simplicity we project our fake list by injecting into AppDomain via reflection emit substitution approach:
            // Instead of full dynamic type creation (complex for this context), we register our fake types under expected full names
            // using a lightweight fallback: create a shadow assembly only once.
            // Simpler approach: create types with correct full names via Reflection.Emit.

            // If we've already created the dynamic assembly skip.
            if (AppDomain.CurrentDomain.GetAssemblies().Any(a => a.GetName().Name == "_VerseTestShim"))
            {
                // Update running list
                UpdateList(packageIds);
                return;
            }

            var asmName = new AssemblyName("_VerseTestShim");
            var asmBuilder = AssemblyBuilder.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run);
            var module = asmBuilder.DefineDynamicModule("Main");

            // Define Verse.LoadedModManager
            var ns = "Verse";
            var typeBuilderModMeta = module.DefineType(ns + ".ModMetaData", TypeAttributes.Public | TypeAttributes.Class);
            var fieldPid = typeBuilderModMeta.DefineField("<PackageIdLowerCase>k__BackingField", typeof(string), FieldAttributes.Private);
            // Auto prop get only for our usage
            var prop = typeBuilderModMeta.DefineProperty("PackageIdLowerCase", PropertyAttributes.None, typeof(string), null);
            var getter = typeBuilderModMeta.DefineMethod("get_PackageIdLowerCase", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName, typeof(string), Type.EmptyTypes);
            var il = getter.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, fieldPid);
            il.Emit(OpCodes.Ret);
            prop.SetGetMethod(getter);
            // Ctor(string id)
            var ctor = typeBuilderModMeta.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new[] { typeof(string) });
            var ilc = ctor.GetILGenerator();
            ilc.Emit(OpCodes.Ldarg_0);
            ilc.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes)!);
            ilc.Emit(OpCodes.Ldarg_0);
            ilc.Emit(OpCodes.Ldarg_1);
            ilc.Emit(OpCodes.Stfld, fieldPid);
            ilc.Emit(OpCodes.Ret);
            var runtimeModMetaType = typeBuilderModMeta.CreateType();

            var typeBuilderLoadedMgr = module.DefineType(ns + ".LoadedModManager", TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.Abstract);
            var listType = typeof(List<>).MakeGenericType(runtimeModMetaType!);
            var fieldList = typeBuilderLoadedMgr.DefineField("_list", listType, FieldAttributes.Private | FieldAttributes.Static);
            var propList = typeBuilderLoadedMgr.DefineProperty("RunningModsListForReading", PropertyAttributes.None, typeof(IEnumerable<>).MakeGenericType(runtimeModMetaType!), null);
            var getter2 = typeBuilderLoadedMgr.DefineMethod("get_RunningModsListForReading", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.Static, propList.PropertyType, Type.EmptyTypes);
            var il2 = getter2.GetILGenerator();
            il2.Emit(OpCodes.Ldsfld, fieldList);
            il2.Emit(OpCodes.Ret);
            propList.SetGetMethod(getter2);
            // Static cctor to initialize list
            var cctor = typeBuilderLoadedMgr.DefineConstructor(MethodAttributes.Static, CallingConventions.Standard, Type.EmptyTypes);
            var ilcc = cctor.GetILGenerator();
            ilcc.Emit(OpCodes.Newobj, listType.GetConstructor(Type.EmptyTypes)!);
            ilcc.Emit(OpCodes.Stsfld, fieldList);
            ilcc.Emit(OpCodes.Ret);
            var runtimeLoadedMgrType = typeBuilderLoadedMgr.CreateType();

            // Populate initial list
            var listObj = runtimeLoadedMgrType!.GetProperty("RunningModsListForReading")!.GetValue(null) as System.Collections.IList;
            foreach (var id in packageIds)
            {
                listObj!.Add(Activator.CreateInstance(runtimeModMetaType!, id.ToLowerInvariant()));
            }
        }

        private void UpdateList(string[] packageIds)
        {
            var loaded = AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "_VerseTestShim");
            var loadedMgrType = loaded.GetType("Verse.LoadedModManager");
            var runtimeModMeta = loaded.GetType("Verse.ModMetaData");
            var list = loadedMgrType!.GetProperty("RunningModsListForReading")!.GetValue(null) as System.Collections.IList;
            list!.Clear();
            foreach (var id in packageIds)
            {
                list.Add(Activator.CreateInstance(runtimeModMeta!, id.ToLowerInvariant()));
            }
        }

        [Fact]
        public void ActiveDlcSummary_NoDlc_ReturnsNoDlc()
        {
            ModDependency.InternalResetForTests();
            InjectFakeVerse();
            Assert.Equal("(No DLC)", ModDependency.ActiveDlcSummary());
        }

        [Fact]
        public void ActiveDlcSummary_WithSomeDlc_ListsThem()
        {
            ModDependency.InternalResetForTests();
            InjectFakeVerse(ModDependency.RoyaltyId, ModDependency.OdysseyId);
            var summary = ModDependency.ActiveDlcSummary();
            Assert.Contains("Royalty", summary);
            Assert.Contains("Odyssey", summary);
            Assert.DoesNotContain("Biotech", summary);
        }

        [Fact]
        public void Has_CachesResults()
        {
            ModDependency.InternalResetForTests();
            InjectFakeVerse(ModDependency.BiotechId);
            Assert.True(ModDependency.Biotech);
            // Remove from underlying list to confirm cached value sticks
            InjectFakeVerse();
            Assert.True(ModDependency.Biotech); // still true because cached
        }
    }
}
#endif
