using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

using LCSoundTool;
using UnityEngine;
using LethalConfig;
using LethalConfig.ConfigItems;
using LethalConfig.ConfigItems.Options;
using BepInEx.Configuration;
using System.IO;

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
            try
            {
                InitializeLethalConfigIntegration();
            } catch (FileNotFoundException)
            {
                Logger.LogInfo("LethalConfig.dll not found");
            }

            Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
        }

        internal void InitializeConfig()
        {
            AudioVolume = Config.Bind("Audio Properties", "Volume", 0.5f, "Playback volume relative to source audio file");
            AudioFadeIn = Config.Bind("Audio Properties", "Fade-in duration", 0.5f, "Duration of fade-in to playback in seconds");
            AudioDelay = Config.Bind("Audio Properties", "Delay", 2f, "Time after mounting a ladder before playback beings in seconds");
            
            Restart = Config.Bind("General", "Restart audio", true, "Restarts the audio each time an applicable ladder is mounted");
            Restrict = Config.Bind("General", "Restrict ladders", true, "Only allow ladders past a certain height threshold to begin playback");
            HeightThreshold = Config.Bind("General", "Height threshold", 12f, "Minimum height which will allow playback when \"Restrict ladders\" is enabled");
        }

        internal void InitializeLethalConfigIntegration()
        {
            LethalConfigManager.SetModDescription("SnakeEater");

            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(Restart, requiresRestart: false));
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(Restrict, requiresRestart: false));
            LethalConfigManager.AddConfigItem(new FloatInputFieldConfigItem(HeightThreshold, requiresRestart: false));

            LethalConfigManager.AddConfigItem(new FloatSliderConfigItem(AudioVolume, new FloatSliderOptions { Max = 1.5f, Min = 0f, RequiresRestart = false }));
            LethalConfigManager.AddConfigItem(new FloatInputFieldConfigItem(AudioFadeIn, requiresRestart: false));
            LethalConfigManager.AddConfigItem(new FloatInputFieldConfigItem(AudioDelay, requiresRestart: false));

            AudioVolume.SettingChanged += delegate
            {
                SnakeEaterAudioSource.volume = AudioVolume.Value;
            };
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
