using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreStar : MonoBehaviour
{
    [SerializeField] Image star;
    [SerializeField] Particles particleStar;
    [SerializeField] float delay = 0.5f;
    [SerializeField] AudioClip starSound;
    bool isActive = false;

    // Start is called before the first frame update
    void Start()
    {
        SetActive(false);
    }

    void SetActive(bool _state)
    {
        if (star != null)
        {
            star.gameObject.SetActive(false);
        }
    }
    public void Active()
    {
        if (isActive) { return; }
        StartCoroutine(ActiveCoroutine());
    }

    IEnumerator ActiveCoroutine()
    {
        isActive = true;

        if(particleStar != null)
        {
            particleStar.PlayParticle();
        }
        if(AudioManager.Instance != null && starSound != null)
        {
            AudioManager.Instance.PlayAudio(starSound);
        }
        yield return new WaitForSeconds(delay);
        SetActive(true);
    }
}
