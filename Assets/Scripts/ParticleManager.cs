using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    [SerializeField] GameObject clearPrefab;
    [SerializeField] GameObject breakPrefab;
    [SerializeField] GameObject doublePrefab;

    public void ClearPiece(int _x, int _y)
    {
        if(clearPrefab != null)
        {
            GameObject clear = Instantiate(clearPrefab, new Vector2(_x, _y), Quaternion.identity);

            Particles particle = clear.GetComponent<Particles>();

            if(particle != null)
            {
                particle.PlayParticle();
            }
        }
    }
    public void BreakTile(int _break, int _x, int _y)
    {
        GameObject breakObject = null;
        Particles particle = null;

        if(_break > 1)
        {
            if(doublePrefab != null)
            {
                breakObject = Instantiate(doublePrefab, new Vector2(_x, _y), Quaternion.identity);
            }
        }
        else
        {
            if (breakPrefab != null)
            {
                breakObject = Instantiate(breakPrefab, new Vector2(_x, _y), Quaternion.identity);
            }
        }

        if(breakObject != null)
        {
            particle = breakObject.GetComponent<Particles>();
            if(particle != null)
            {
                particle.PlayParticle();
            }
        }
    }
}
