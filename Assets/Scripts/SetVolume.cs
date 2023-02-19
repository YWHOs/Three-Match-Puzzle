using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
public class SetVolume : MonoBehaviour
{
    public AudioMixer mixer;
    public void SetBGMLevel(float _sliderValue)
    {
        mixer.SetFloat("BGM", Mathf.Log10(_sliderValue) * 20);
    }
    public void SetSFXLevel(float _sliderValue)
    {
        mixer.SetFloat("SFX", Mathf.Log10(_sliderValue) * 20);
    }

}
