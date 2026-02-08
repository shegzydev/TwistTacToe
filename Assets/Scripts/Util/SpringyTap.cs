using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class SpringyTapButton : Button, IPointerClickHandler
{
    [Header("Bounce Settings")]
    public float pressedScale = 0.85f;
    public float overshootScale = 1.1f;
    public float bounceDuration = 0.15f;

    Vector3 defaultScale;
    CanvasGroup canvasGroup;
    bool isBouncing;

    new void Awake()
    {
        defaultScale = transform.localScale;

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        canvasGroup.alpha = interactable ? 1f : 0.4f;
        canvasGroup.interactable = interactable;
        canvasGroup.blocksRaycasts = interactable;
    }

    void Update()
    {
        canvasGroup.interactable = interactable;
        canvasGroup.blocksRaycasts = interactable;
        canvasGroup.alpha = interactable ? 1f : 0.4f;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (!interactable || isBouncing) return;
        StartCoroutine(BounceThenInvoke());
    }

    IEnumerator BounceThenInvoke()
    {
        isBouncing = true;

        // Step 1: Press In
        yield return ScaleTo(defaultScale * pressedScale, bounceDuration * 0.4f);

        // Step 2: Overshoot Out
        yield return ScaleTo(defaultScale * overshootScale, bounceDuration * 0.4f);

        // Step 3: Return
        yield return ScaleTo(defaultScale, bounceDuration * 0.2f);

        // Step 4: Trigger Event
        isBouncing = false;

        onClick?.Invoke();
    }

    IEnumerator ScaleTo(Vector3 target, float duration)
    {
        Vector3 start = transform.localScale;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, time / duration);
            transform.localScale = Vector3.LerpUnclamped(start, target, t);
            yield return null;
        }

        transform.localScale = target;
    }
}
