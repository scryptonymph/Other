using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The game audio fade in and out functionality.
public static class AudioFade {
    // Fades the given audio in in the given time span.
    public static IEnumerator FadeIn(AudioSource audio, float time) {
        float maxVolume = audio.volume;

        audio.volume = 0;

        audio.Play();

        while (audio.volume < maxVolume) {
            audio.volume += maxVolume * Time.deltaTime / time;

            yield return null;
        }

        audio.volume = maxVolume;
    }

    // Fades the given audio out in the given time span.
    public static IEnumerator FadeOut(AudioSource audio, float time) {
        float startVolume = audio.volume;

        while (audio.volume > 0) {
            audio.volume -= startVolume * Time.deltaTime / time;

            yield return null;
        }

        audio.Stop();

        audio.volume = startVolume;
    }

    public static IEnumerator CrossFade(AudioSource audio1, AudioSource audio2, float time) {
        float startVolume = audio1.volume;
        float maxVolume = audio2.volume;
        audio2.volume = 0;

        while ((audio1.volume > 0) && (audio2.volume < maxVolume)) {
            audio1.volume -= startVolume * Time.deltaTime / time;

            audio2.Play();

            audio2.volume += maxVolume * Time.deltaTime / time;

            yield return null;
        }

        audio1.Stop();

        audio1.volume = startVolume;
        audio2.volume = maxVolume;
    }
}
