using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldSizeChanger : MonoBehaviour
{
    void Start()
    {
        var size = GameManager.Instance.world.fieldSize;
        transform.localScale = (Vector2.one * size) + Vector2.one;
        transform.position = (Vector2.one * size)/2;
    }
}
