﻿using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;


namespace SnakeEater.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerBPatch
    {
        [HarmonyPatch("ConnectClientToPlayerObject")]
        [HarmonyPostfix]
        private static void AttachAudioSource(PlayerControllerB __instance)
        {
            AudioSource? audioSource = __instance.gameObject.AddComponent<AudioSource>();

            if (audioSource != null) {
                audioSource.playOnAwake = false;
                audioSource.clip = SnakeEater.SnakeEaterAudio;
                SnakeEater.SnakeEaterAudioSource = audioSource;
                SnakeEater.Logger.LogInfo("Audio source attached");
            }
            else
            {
                SnakeEater.Logger.LogError("Failed to attach Snake Eater audio source");
            }
        }
    }
}
