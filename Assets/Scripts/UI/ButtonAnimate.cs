using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonAnimate : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float width;
    bool over;
    RectTransform rect;
    public void OnPointerEnter(PointerEventData eventData)
    {
        over = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        over = false;
    }

    private void Start()
    {
        rect = GetComponent<RectTransform>();
        width = rect.sizeDelta.x;
    }

    private void LateUpdate()
    {
        if (over)
        {
            rect.sizeDelta = Vector2.Lerp(rect.sizeDelta, new Vector2(width * 1.2f, rect.sizeDelta.y), 5 * Time.unscaledDeltaTime);
        }
        else
        {
            rect.sizeDelta = Vector2.Lerp(rect.sizeDelta, new Vector2(width, rect.sizeDelta.y), 5 * Time.unscaledDeltaTime);
        }
    }
}
