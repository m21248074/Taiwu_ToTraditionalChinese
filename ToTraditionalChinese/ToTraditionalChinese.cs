using HarmonyLib;
using TaiwuModdingLib.Core.Plugin;
using TMPro;
using UnityEngine.UI;

namespace ToTraditionalChinese
{
    [PluginConfig("ToTraditionalChinese", "CaveyCoding", "1.0.0")]
    public class ToTraditionalChinese : TaiwuRemakePlugin
    {
        private static Harmony harmony;
        private static int ToTCFunctionType = 0;

        public enum OpenccConfig
        {
            s2t = 1,
            s2tw = 2,
            s2twp = 3,
            s2hk = 4
        }

        public override void Dispose()
        {
            harmony?.UnpatchSelf();
            OpenCCHelper.DisposeOpenCC();
        }

        public override void Initialize()
        {
            // OpenCCHelper.ExtractOpenCCFiles();
            ReadSettings();
            if (ToTCFunctionType != 0)
            {
                OpenccConfig config = (OpenccConfig)ToTCFunctionType;
                OpenCCHelper.InitialOpenCC(config.ToString());
            }
            harmony = Harmony.CreateAndPatchAll(typeof(ToTraditionalChinese));
        }

        public void ReadSettings()
        {
            ModManager.GetSetting(this.ModIdStr, "ToTCFunctionType", ref ToTCFunctionType);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TMP_Text), "text", MethodType.Setter)]
        public static void TMP_TextPrefix(Text __instance, ref string value)
        {
            if (value == null) return;
            switch (ToTCFunctionType)
            {
                case 0:
                    value = Util.ToTraditionalChinese(value);
                    break;
                case 1:
                case 2:
                case 3:
                case 4:
                    value = OpenCCHelper.OpenCC(value);
                    break;
                default:
                    value = Util.ToTraditionalChinese(value);
                    break;
            }
        }

    }
}
