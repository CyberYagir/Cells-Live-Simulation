using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fader : MonoBehaviour
{
    public enum Fade { FadeIn, FadeOut};
    public Fade fade;
    public float waitTime;

    bool waitEnd;

    private void Start()
    {
        StartCoroutine(wait());
    }

    private void LateUpdate()
    {
        if (waitEnd == true)
        {
            GetComponent<Animator>().Play(fade.ToString());
        }
    }
    IEnumerator wait()
    {
        yield return new WaitForSecondsRealtime(waitTime);
        waitEnd = true;
    }
}
