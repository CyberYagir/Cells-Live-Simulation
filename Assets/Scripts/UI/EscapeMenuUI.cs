using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeMenuUI : MonoBehaviour
{
    public void SetIsCanTrue()
    {
        GetComponentInParent<UIManager>().canEscape = true;
    }
    public void SetIsCanFalse()
    {
        GetComponentInParent<UIManager>().canEscape = false;
    }
}
