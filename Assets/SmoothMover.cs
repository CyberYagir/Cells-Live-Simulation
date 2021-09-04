using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothMover : MonoBehaviour
{
    public Vector3 newPos;

    private void Start()
    {
        newPos = transform.position;
    }
    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, newPos, 20 * Time.deltaTime);
    }
}
