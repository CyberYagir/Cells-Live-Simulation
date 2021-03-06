using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayType { Forward, Rewind };
public enum MoveType { Move, Lerp };
public class UIAnimate : MonoBehaviour
{
    public PlayType playType;
    public MoveType moveType;
    [SerializeField] Vector2 startPoint, endPoint;
    public float speed = 6;
    new RectTransform transform;
    bool plaing;
    public bool playOnStart ;
    public float startWaitTime = 0.5f;
    float time;

    private void Start()
    {
        Init();
        if (playOnStart)
        {
            Play();
        }
    }
    public void Play()
    {
        transform.anchoredPosition = playType == PlayType.Forward ? startPoint : endPoint;
        plaing = true;
    }
    public void Stop()
    {
        plaing = false;
    }
    private void Update()
    {
        if (Time.unscaledDeltaTime > 0.1f) return;
        time += Time.unscaledDeltaTime;
        
        if (plaing && time > startWaitTime)
        {
            if (playType == PlayType.Forward)
            {
                Move(endPoint);
            }
            else
            {
                Move(startPoint);
            }
        }        
    }

    public void PinPong()
    {
        plaing = true;
        if (playType == PlayType.Forward)
        {
            playType = PlayType.Rewind;
        }
        else
        {
            playType = PlayType.Forward;
        }
    }

    public void Move(Vector3 point)
    {
        if (moveType == MoveType.Move)
        {
            transform.anchoredPosition = Vector3.LerpUnclamped(transform.anchoredPosition, point, speed * Time.unscaledDeltaTime);
        }
        if (moveType == MoveType.Lerp)
        {
            transform.anchoredPosition = Vector3.Lerp(transform.anchoredPosition, point, speed * Time.unscaledDeltaTime);
        }
    }

    public void Init()
    {
        transform = GetComponent<RectTransform>();
    }

}
