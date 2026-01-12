using HarmonyLib;
using ResoniteModLoader;
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using System.Collections.Generic;
using FrooxEngine;
using Assimp;

namespace MeshImportUnfuck;

public class MeshImportUnfuck : ResoniteMod
{
    public const string VERSION = "1.0.0";
    public override string Name => "MeshImportUnfuck";
    public override string Author => "art0007i";
    public override string Version => VERSION;
    public override string Link => "https://github.com/art0007i/MeshImportUnfuck/";
    public override void OnEngineInit()
    {
        Harmony harmony = new Harmony("me.art0007i.MeshImportUnfuck");
        harmony.PatchAll();

    }
    [HarmonyPatch(typeof(ModelImporter), "PreprocessScene")]
    class MeshImportUnfuckPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
        {
            var badFunc = AccessTools.Property(typeof(MeshAnimationAttachment), nameof(MeshAnimationAttachment.Vertices)).GetMethod;
            var goodFunc = AccessTools.Property(typeof(MeshAnimationAttachment), nameof(MeshAnimationAttachment.Tangents)).GetMethod;
            var lookFor = AccessTools.Property(typeof(MeshAnimationAttachment), nameof(MeshAnimationAttachment.HasTangentBasis)).GetMethod;
            var fixNext = false;
            var found = false;
            foreach(var code in codes) {
                if(code.Calls(lookFor)) {
                    fixNext = true;
                }
                if(fixNext && code.Calls(badFunc)) {
                    code.operand = goodFunc;
                    fixNext = false;
                    found = true;
                }
                yield return code;
            }
            if(!found)
            {
                Error("Transpiler Failed. This means the mod did nothing.");
            }
        }
    }
}
