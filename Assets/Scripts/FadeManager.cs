using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MaskableGraphic))]
public class FadeManager : MonoBehaviour
{
    [SerializeField] float time;
    public bool isFade;
    // Start is called before the first frame update
    void Start()
    {


    }
    public void FadeOut()
    {
        StartCoroutine(FadeOut(gameObject.GetComponent<Image>()));
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
