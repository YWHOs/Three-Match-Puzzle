using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particles : MonoBehaviour
{
    [SerializeField] ParticleSystem[] particles;
    [SerializeField] float time;
    // Start is called before the first frame update
    void Start()
    {
        particles = GetComponentsInChildren<ParticleSystem>();

        Destroy(gameObject, time);
    }

    public void PlayParticle()
    {
        foreach (ParticleSystem ps in particles)
        {
            ps.Stop();
            ps.Play();
        }
    }
}
