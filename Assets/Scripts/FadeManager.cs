using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    [SerializeField] float time;
    [SerializeField] Image image;
    public bool isFade;

    public void FadeOut()
    {
        StartCoroutine(FadeOut(image));
    }
    IEnumerator FadeOut(Image _object)
    {
        isFade = true;
        Color color = _object.color;
        color.a = _object.color.a;
        while (color.a > 0f)
        {
            color.a -= time * Time.deltaTime;
            _object.color = color;
            yield return null;
        }
        color.a = 0f;
        isFade = false;
        _object.gameObject.SetActive(false);
    }
}
