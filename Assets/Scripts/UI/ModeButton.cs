using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ModeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform size;
    public bool view;

    float height;
    public void OnPointerEnter(PointerEventData eventData)
    {
        view = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        view = false;
    }

    private void LateUpdate()
    {
        size.sizeDelta = Vector2.Lerp(size.sizeDelta, view ? new Vector2(200, size.sizeDelta.y) : new Vector2(0, size.sizeDelta.y), 10 * Time.unscaledDeltaTime);
    }
}
