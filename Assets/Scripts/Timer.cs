using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] Text timeLeft;
    [SerializeField] Image clock;

    float max = 60;
    // Flash
    int flashTime = 10;
    [SerializeField] AudioClip flashAudio;
    float flashInterval = 1f;
    Color flashColor = Color.red;
    IEnumerator flashIEnumerator;

    public bool isPaused;

    public void InitTime(int _maxTime = 60)
    {
        max = _maxTime;
        clock.type = Image.Type.Filled;
        clock.fillMethod = Image.FillMethod.Radial360;
        clock.fillOrigin = (int)Image.Origin360.Top;

        if(timeLeft != null)
        {
            timeLeft.text = _maxTime.ToString();
        }
    }

    public void UpdateTime(float _time)
    {
        if (isPaused) { return; }
        clock.fillAmount = _time / max;
        if(_time <= flashTime)
        {
            flashIEnumerator = FlashCoroutine(clock, flashColor, flashInterval);
            StartCoroutine(flashIEnumerator);
            AudioManager.Instance.PlayAudio(flashAudio);
        }

        timeLeft.text = _time.ToString();
    }

    IEnumerator FlashCoroutine(Image _image, Color _color, float _interval)
    {
        if(_image != null)
        {
            Color color = _image.color;
            _image.CrossFadeColor(_color, _interval * 0.3f, true, true);
            yield return new WaitForSeconds(_interval * 0.5f);

            _image.CrossFadeColor(color, _interval * 0.3f, true, true);
            yield return new WaitForSeconds(_interval * 0.5f);
        }
    }
    public void StopFlash()
    {
        if(flashIEnumerator != null)
        {
            StopCoroutine(flashIEnumerator);
        }
    }
}
