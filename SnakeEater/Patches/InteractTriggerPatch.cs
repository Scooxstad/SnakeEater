using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;
using LCSoundTool;
using System.Runtime.CompilerServices;
using System.Collections;

namespace SnakeEater.Patches
{
    [HarmonyPatch(typeof(InteractTrigger))]
    internal class InteractTriggerPatch
    {

        static Coroutine PlaybackCoroutine;

        [HarmonyPatch("SetUsingLadderOnLocalClient")]
        [HarmonyPrefix]
        internal static void PrintLadderStatus(InteractTrigger __instance, bool isUsing)
        {
            float height = Vector3.Distance(__instance.bottomOfLadderPosition.position, __instance.topOfLadderPosition.position);

            if (isUsing)
            {
                if (SnakeEater.Restrict.Value && SnakeEater.HeightThreshold.Value <= height)
                    PlaybackCoroutine = __instance.StartCoroutine(BeginPlayback());
            } else
            {
                if (SnakeEater.SnakeEaterAudioSource.isPlaying)
                {
                    __instance.StopCoroutine(PlaybackCoroutine);
                    SnakeEater.SnakeEaterAudioSource.Pause();
                }
            }
        }

        internal static IEnumerator BeginPlayback()
        {
            yield return (object)new WaitForSeconds(SnakeEater.AudioDelay.Value);

            AudioSource audioSource = SnakeEater.SnakeEaterAudioSource;

            if (SnakeEater.Restart.Value)
                audioSource.time = 0;

            audioSource.volume = 0f;
            audioSource.Play();

            float fadeInDuration = SnakeEater.AudioFadeIn.Value;

            while (audioSource.volume < SnakeEater.AudioVolume.Value)
            {
                yield return null;
                audioSource.volume += Time.deltaTime / fadeInDuration;
            }
        }
    }
}
