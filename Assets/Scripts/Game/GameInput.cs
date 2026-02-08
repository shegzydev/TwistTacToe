using System;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class GameInput : MonoBehaviour, IPointerDownHandler
{
    public static Action<(int column, int row)> OnInputDetected;

    int rows = 3;
    int columns = 3;
    RectTransform rectTransform;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition))
        {
            Vector2 localPointInRect;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, null, out localPointInRect);

            localPointInRect.x += rectTransform.rect.width / 2;

            localPointInRect.y *= -1;
            localPointInRect.y += rectTransform.rect.height / 2;

            int tappedColumn = (int)(localPointInRect.x / (rectTransform.rect.width / columns));
            int tappedRow = (int)(localPointInRect.y / (rectTransform.rect.height / rows));

            OnInputDetected?.Invoke((tappedColumn, tappedRow));
        }
    }

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        /*if (Input.GetMouseButtonDown(0))
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition))
            {
                Vector2 localPoint;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, null, out localPoint);

                localPoint.x += rectTransform.rect.width / 2;

                localPoint.y *= -1;
                localPoint.y += rectTransform.rect.height / 2;

                int tappedColumn = (int)(localPoint.x / (rectTransform.rect.width / columns));
                int tappedRow = (int)(localPoint.y / (rectTransform.rect.height / rows));

                OnInputDetected?.Invoke((tappedColumn, tappedRow));
            }
        }*/
    }
}
