using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using BepInEx.Configuration;
namespace Custom_Focus_Value;

[BepInPlugin(PluginInfo.GUID, PluginInfo.NAME, PluginInfo.VERSION)]
public class Plugin : BaseUnityPlugin
{

    internal static new ManualLogSource Logger;
    internal static ConfigEntry<float> FocusScale;

    public void Awake()
    {
        Logger = base.Logger;
        FocusScale = Config.Bind(
                "General",                
                "FocusScale",             
                0.25f,                    
                "Focus slowdown multiplier, lower number means more slowdown. Negative numbers behave as if focus is 1, until natural game slowdown occurs, in which case you keep that slowdown until you release focus." +
                " WARNING: Use high focus at own risk lol."
            );

        Logger.LogInfo($"Plugin {PluginInfo.GUID} loaded! Using FocusScale = {FocusScale.Value}");

        Harmony harmony = new(PluginInfo.GUID);
        harmony.Patch(
            original: AccessTools.Method(typeof(PlayerController), nameof(PlayerController.FocusON)),
            prefix: new HarmonyMethod(typeof(Patch), nameof(Patch.Custom_Focus_Value))
        );
    }

    public class Patch
    {
        public static bool Custom_Focus_Value(PlayerController __instance)
        {
            // Prevent game from updating scores to leaderboards
            Game.mod = true;
            float scale = Plugin.FocusScale.Value;
            var Focus_scale = AccessTools.Field(__instance.GetType(), "focusScale");
            Focus_scale.SetValue(__instance, scale);
            return true;
        }
    }
}