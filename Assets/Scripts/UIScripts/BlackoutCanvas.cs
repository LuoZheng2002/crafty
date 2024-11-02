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
        SetImageAlpha(0.0f);
	}
    void SetImageAlpha(float alpha)
    {
		Color color = image.color;
		color.a = alpha;
		image.color = color;
	}
    public void Blackout(float time, bool turns_black)
    {
		StartCoroutine(BlackoutHelper(time, turns_black));
    }
    IEnumerator BlackoutHelper(float time_transition, bool turns_black)
    {
        float start_time = Time.time;
        float initial_alpha = turns_black ? 0.0f : 1.0f;
        float end_alpha = turns_black ? 1.0f: 0.0f;
        while (Time.time - start_time < time_transition)
        {
            SetImageAlpha(Mathf.Lerp(initial_alpha, end_alpha, (Time.time - start_time) / time_transition));
            yield return null;
        }
    }
}
