using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{
    public AudioMixer mixer;

    private void Start()
    {
        setMixerVolume("VolumeMusic", GameSettings.MusicVolume);
        setMixerVolume("VolumeSFX", GameSettings.SfxVolume);

        GameSettings.MusicVolumeChanged += (float volume) =>
        {
            setMixerVolume("VolumeMusic", volume);
        };

        GameSettings.SfxVolumeChanged += (volume) =>
        {
            setMixerVolume("VolumeSFX", volume);
        };
    }

    private void setMixerVolume(string mixerName, float volume)
    {
        if (!(volume > 0))
            volume = 0.0000025f;
        float dbVol = 20 * Mathf.Log10(volume);
        mixer.SetFloat(mixerName, dbVol);
    }
}
