using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        yield return new WaitForSecondsRealtime(waitTime/2f);
        GetComponent<Image>().raycastTarget = false;
        yield return new WaitForSecondsRealtime(waitTime/2f);
        waitEnd = true;
    }
}
