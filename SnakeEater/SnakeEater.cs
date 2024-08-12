using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

using LethalConfig;
using LCSoundTool;
using UnityEngine;
using LethalConfig.ConfigItems;
using LethalConfig.ConfigItems.Options;
using BepInEx.Configuration;
using System.Dynamic;

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

        // Configurable General Parameters
        internal static ConfigEntry<bool> Restart;
        internal static ConfigEntry<bool> Restrict;
        internal static ConfigEntry<float> HeightThreshold;



        // Configurable Audio Parameters
        internal static ConfigEntry<float> AudioVolume;
        internal static ConfigEntry<float> AudioFadeIn;
        internal static ConfigEntry<float> AudioDelay;

        private void Awake()
        {
            Logger = base.Logger;
            Instance = this;
            
            SnakeEaterAudio = SoundTool.GetAudioClip("Scooxstad-SnakeEater", "Audio", "SnakeEater.mp3");

            Patch();
            InitializeConfig();

            Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
        }

        internal void InitializeConfig()
        {
            LethalConfigManager.SetModDescription("SnakeEater");

            Restart = Config.Bind("General", "Restart audio", true, "Restarts the audio each time an applicable ladder is mounted");
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(Restart, requiresRestart: false));
            Restrict = Config.Bind("General", "Restrict ladders", true, "Only allow ladders past a certain height threshold to begin playback");
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(Restrict, requiresRestart: false));
            HeightThreshold = Config.Bind("General", "Height threshold", 12f, "Minimum height which will allow playback when \"Restrict ladders\" is enabled");
            LethalConfigManager.AddConfigItem(new FloatInputFieldConfigItem(HeightThreshold, requiresRestart: false));


            AudioVolume = Config.Bind("Audio Properties", "Volume", 0.5f, "Playback volume relative to source audio file");
            LethalConfigManager.AddConfigItem(new FloatSliderConfigItem(AudioVolume, new FloatSliderOptions { Max = 1.5f, Min = 0f, RequiresRestart = false }));

            AudioVolume.SettingChanged += delegate
            {
                SnakeEaterAudioSource.volume = AudioVolume.Value;
            };

            AudioFadeIn = Config.Bind("Audio Properties", "Fade-in duration", 0.5f, "Duration of fade-in to playback in seconds");
            LethalConfigManager.AddConfigItem(new FloatInputFieldConfigItem(AudioFadeIn, requiresRestart: false));
            AudioDelay = Config.Bind("Audio Properties", "Delay", 2f, "Time after mounting a ladder before playback beings in seconds");
            LethalConfigManager.AddConfigItem(new FloatInputFieldConfigItem(AudioDelay, requiresRestart: false));

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
