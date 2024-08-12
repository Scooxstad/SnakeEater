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

        static Coroutine? FadeInCoroutine;

        [HarmonyPatch("SetUsingLadderOnLocalClient")]
        [HarmonyPrefix]
        internal static void PrintLadderStatus(InteractTrigger __instance, bool isUsing)
        {
            float height = Vector3.Distance(__instance.bottomOfLadderPosition.position, __instance.topOfLadderPosition.position);

            SnakeEater.Logger.LogInfo("Ladder height: " + height);

            if (isUsing)
            {
                FadeInCoroutine = __instance.StartCoroutine(MusicFadeIn());
            } else
            {
                __instance.StopCoroutine(FadeInCoroutine);
                SnakeEater.SnakeEaterAudioSource.Pause();
            }
        }

        internal static IEnumerator MusicFadeIn()
        {
            yield return (object)new WaitForSeconds(1f);

            AudioSource audioSource = SnakeEater.SnakeEaterAudioSource;

            audioSource.volume = 0f;
            audioSource.Play();

            for (float i = 0; i < 3f; i += Time.deltaTime)
            {
                yield return null;
                audioSource.volume += Time.deltaTime / 3f;
                if (audioSource.volume >= SnakeEater.MAX_VOLUME)
                {
                    break;
                }
            }
        }
    }
}
