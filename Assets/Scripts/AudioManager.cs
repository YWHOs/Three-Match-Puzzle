using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioClip winAudio;
    [SerializeField] AudioClip LoseAudio;

    public static AudioManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public AudioSource PlayAudio(AudioClip _audio, float _volume = 1f)
    {
        if(_audio != null)
        {
            GameObject go = new GameObject(_audio.name);
            AudioSource source = go.AddComponent<AudioSource>();
            source.clip = _audio;
            source.volume = _volume;
            source.Play();
            Destroy(go, _audio.length);
            return source;
        }
        return null;
    }
    public void PlayWin()
    {
        PlayAudio(winAudio, 0.5f);
    }
    public void PlayLose()
    {
        PlayAudio(LoseAudio, 0.5f);
    }
}
