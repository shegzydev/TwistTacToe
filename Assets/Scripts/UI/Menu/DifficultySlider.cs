using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DifficultySlider : Slider, IPointerUpHandler, IPointerDownHandler, IPointerMoveHandler
{
    public bool isLerping;
    bool isPressed;

    public new void OnPointerUp(PointerEventData eventData)
    {
        if (!isLerping)
        {
            LerpSlider(Mathf.RoundToInt(value));
        }
        else
        {
            StopLerp();

            var rect = fillRect.parent.GetComponent<RectTransform>();
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, eventData.position, null, out Vector2 point))
            {
                float xPos = point.x + rect.rect.width / 2;
                float frac = xPos / rect.rect.width;
                LerpSlider(Mathf.RoundToInt(frac * maxValue));
            }
        }
        isPressed = false;
    }

    public new void OnPointerDown(PointerEventData eventData)
    {
        StopLerp();

        var rect = fillRect.parent.GetComponent<RectTransform>();
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, eventData.position, null, out Vector2 point))
        {
            float xPos = point.x + rect.rect.width / 2;
            float frac = xPos / rect.rect.width;
            LerpSlider(frac * maxValue);
        }

        isPressed = true;
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (isLerping && isPressed)
        {
            StopLerp();
        }
    }

    void LerpSlider(float target)
    {
        StartCoroutine(LerpSliderRoutine(target));
    }

    IEnumerator LerpSliderRoutine(float target)
    {
        isLerping = true;

        float lerpTime = 0.1f;
        float timeElapsed = 0;
        float startValue = value;

        while (timeElapsed < lerpTime)
        {
            timeElapsed += Time.deltaTime;
            value = Mathf.Lerp(startValue, target, timeElapsed / lerpTime);
            yield return null;
        }

        value = target;
        isLerping = false;
    }

    void StopLerp()
    {
        StopAllCoroutines();
        isLerping = false;
    }
}
