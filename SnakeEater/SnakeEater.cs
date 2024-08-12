using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

using LCSoundTool;
using UnityEngine;

namespace SnakeEater
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class SnakeEater : BaseUnityPlugin
    {
        public static SnakeEater Instance { get; private set; } = null!;
        internal new static ManualLogSource Logger { get; private set; } = null!;
        internal static Harmony? Harmony { get; set; }

        internal static AudioClip SnakeEaterAudio { get; set; } = null!;
        internal static AudioSource SnakeEaterAudioSource { get; set;} = null!;

        internal const float MAX_VOLUME = 0f;

        private void Awake()
        {
            Logger = base.Logger;
            Instance = this;
            
            SnakeEaterAudio = SoundTool.GetAudioClip("Scooxstad-SnakeEater", "Sounds", "SnakeEater.mp3");

            Patch();

            Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
        }

        internal static void Patch()
        {
            Harmony ??= new Harmony(MyPluginInfo.PLUGIN_GUID);

            Logger.LogDebug("Patching...");

            Harmony.PatchAll();

            Logger.LogDebug("Finished patching!");
        }

        internal static void Unpatch()
        {
            Logger.LogDebug("Unpatching...");

            Harmony?.UnpatchSelf();

            Logger.LogDebug("Finished unpatching!");
        }
    }
}
