using HarmonyLib;
using UnityEngine;
using System.Collections;

namespace SnakeEater.Patches
{
    [HarmonyPatch(typeof(InteractTrigger))]
    internal class InteractTriggerPatch
    {
        static Coroutine PlaybackCoroutine;

        [HarmonyPatch("SetUsingLadderOnLocalClient")]
        [HarmonyPrefix]
        internal static void TogglePlayback(InteractTrigger __instance, bool isUsing)
        {
            float height = Vector3.Distance(__instance.bottomOfLadderPosition.position, __instance.topOfLadderPosition.position);

            if (isUsing)
            {
                if (!SnakeEater.Restrict.Value || (SnakeEater.Restrict.Value && SnakeEater.HeightThreshold.Value <= height))
                    PlaybackCoroutine = __instance.StartCoroutine(BeginPlayback());
            } else
            {
                if (PlaybackCoroutine != null)
                    __instance.StopCoroutine(PlaybackCoroutine);

                if (SnakeEater.SnakeEaterAudioSource.isPlaying)
                    SnakeEater.SnakeEaterAudioSource.Pause();
            }
        }

        internal static IEnumerator BeginPlayback()
        {
            yield return new WaitForSeconds(SnakeEater.AudioDelay.Value);

            AudioSource audioSource = SnakeEater.SnakeEaterAudioSource;

            if (SnakeEater.Restart.Value)
                audioSource.time = 0;

            audioSource.volume = 0f;
            audioSource.Play();

            float fadeInDuration = SnakeEater.AudioFadeIn.Value;
            float targetVolume = SnakeEater.AudioVolume.Value / 2;
            float timeElapsed = 0;

            if (fadeInDuration > 0)
            {
                while (timeElapsed < fadeInDuration)
                {
                    audioSource.volume = Mathf.Lerp(0, targetVolume, timeElapsed / fadeInDuration);
                    timeElapsed += Time.deltaTime;
                    yield return null;
                }

            } else
            {
                audioSource.volume = SnakeEater.AudioVolume.Value / 2;
            }

        }
    }
}
