using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackoutCanvas : MonoBehaviour
{
    Image image;
    public static BlackoutCanvas Inst
    {
        get { Debug.Assert(inst != null, "Blackout Canvas not set");return inst; }
    }
    static BlackoutCanvas inst;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(inst == null, "Blackout Canvas already set");
        image = transform.Find("Black").GetComponent<Image>();
        inst = this;
        gameObject.SetActive(false);
    }
    public void Blackout(float time, float time_stay, Action func)
    {
		gameObject.SetActive(true);
		StartCoroutine(BlackoutHelper(time, time_stay, func));
    }
    IEnumerator BlackoutHelper(float time_transition, float time_stay, Action func)
    {
        float start_time = Time.time;
        while (Time.time - start_time < time_transition)
        {
            Color color = image.color;
            color.a = Mathf.Lerp(0.0f, 1.0f, (Time.time - start_time) / time_transition);
            image.color = color;
            yield return null;
        }
        start_time = Time.time;
        while(Time.time - start_time < time_stay)
        {
            yield return null;
        }
		gameObject.SetActive(false);
		func();
    }
}
