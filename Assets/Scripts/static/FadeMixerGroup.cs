using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

public static class FadeMixerGroup
{

    public static IEnumerator StartFade(AudioMixer audioMixer, string exposedParamFadeOut, string exposedParamFadeIn, float duration, float targetVolumeFadIn)
    {
        float currentTime = 0;
        float currentVol;
        float targetVolumeFadeOut = 0f;
        float targetValue;

        if (exposedParamFadeOut != "")
        {
            audioMixer.GetFloat(exposedParamFadeOut, out currentVol);
            currentVol = Mathf.Pow(10, currentVol / 20);
            targetValue = Mathf.Clamp(targetVolumeFadeOut, 0.0001f, 1);

            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                float newVol = Mathf.Lerp(currentVol, targetVolumeFadeOut, currentTime / duration);
                audioMixer.SetFloat(exposedParamFadeOut, Mathf.Log10(newVol) * 20);
                yield return null;
            }
        }

        float currentTimeFadeIn = 0;
        float currentVolFadeIn;
        float targetValueFadeIn;

        audioMixer.GetFloat(exposedParamFadeIn, out currentVolFadeIn);
        currentVolFadeIn = Mathf.Pow(10, currentVolFadeIn / 20);
        targetValueFadeIn = Mathf.Clamp(targetVolumeFadIn, 0.0001f, 1);

        while (currentTimeFadeIn < duration)
        {
            currentTimeFadeIn += Time.deltaTime;
            float newVol = Mathf.Lerp(currentVolFadeIn, targetValueFadeIn, currentTimeFadeIn / duration);
            audioMixer.SetFloat(exposedParamFadeIn, Mathf.Log10(newVol) * 20);
            yield return null;
        }

    }
}