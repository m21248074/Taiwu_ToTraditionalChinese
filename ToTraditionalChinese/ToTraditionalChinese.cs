using HarmonyLib;
using TaiwuModdingLib.Core.Plugin;
using TMPro;
using UnityEngine.UI;

namespace ToTraditionalChinese
{
    [PluginConfig("ToTraditionalChinese","CaveyCoding","1.0.0")]
    public class ToTraditionalChinese : TaiwuRemakePlugin
    {
        Harmony harmony;

        public override void Dispose()
        {
            harmony?.UnpatchSelf();
        }

        public override void Initialize()
        {
            harmony = Harmony.CreateAndPatchAll(typeof(ToTraditionalChinese));
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TMP_Text), "text", MethodType.Setter)]
        public static void TMPPrefix(Text __instance, ref string value)
        {
            value = Util.ToTraditionalChinese(value ?? "");
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(TMP_Text), "text", MethodType.Getter)]
        public static void TMPPostfix(ref string __result)
        {
            __result = Util.ToTraditionalChinese(__result);
        }
    }
}
