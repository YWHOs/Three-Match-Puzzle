using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeBonus : MonoBehaviour
{
    [Range(0, 5)]
    public int bonusTime = 5;

    [Range(0f, 1f)]
    public float chanceBonus = 0.1f;

    public GameObject glow;
    public Material[] bonusMaterial;

    private void Start()
    {
        float random = Random.Range(0f, 1f);
        if(random > chanceBonus)
        {
            bonusTime = 0;
        }
        if(GameManager.Instance.LevelTime == null)
        {
            bonusTime = 0;
        }
        SetActive(bonusTime != 0);

        if(bonusTime != 0)
        {
            SetupMaterial(bonusTime - 1, glow);
        }
    }
    void SetActive(bool _state)
    {
        glow.SetActive(_state);
    }

    void SetupMaterial(int _value, GameObject _bonusObject)
    {
        int value = Mathf.Clamp(_value, 0, bonusMaterial.Length - 1);

        if(bonusMaterial[value] != null)
        {
            ParticleSystemRenderer renderer = _bonusObject.GetComponent<ParticleSystemRenderer>();
            renderer.material = bonusMaterial[value];
        }
    }
}
