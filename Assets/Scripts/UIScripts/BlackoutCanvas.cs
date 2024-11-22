using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackoutCanvas : MonoBehaviour
{
    Image image;
    public Text sub_text;
    public AnimationCurve animationCurve;
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
        SetTextAlpha(0.0f);
        gameObject.SetActive(false);
	}
    void SetImageAlpha(float alpha)
    {
		Color color = image.color;
		color.a = alpha;
		image.color = color;
	}
    void SetTextAlpha(float alpha)
    {
        Color color = sub_text.color;
        color.a = alpha;
		sub_text.color = color;
    }
    public void BlackoutAsync(float time, float start_alpha, float end_alpha)
    {
		gameObject.SetActive(true);
        StartCoroutine(Blackout(time, start_alpha, end_alpha));
	}
    public void DisplaySubAsync(string sub, float time, float start_alpha, float end_alpha)
    {
        gameObject.SetActive(true);
        StartCoroutine(DisplaySub(sub, time, start_alpha, end_alpha));
    }
    public IEnumerator DisplaySub(string sub, float time, float start_alpha, float end_alpha)
    {
		gameObject.SetActive(true);
        if (sub != null)
        {
            sub_text.text = sub;
        }
		float start_time = Time.time;
		while (Time.time - start_time < time)
		{
			float progress = (Time.time - start_time) / time;
			float x_val = Mathf.Lerp(start_alpha, end_alpha, progress);
			float y_val = animationCurve.Evaluate(x_val);
			SetTextAlpha(y_val);
			yield return null;
		}
		SetTextAlpha(end_alpha);
	}
    public IEnumerator Blackout(float time, float start_alpha, float end_alpha)
    {
        gameObject.SetActive(true);
		float start_time = Time.time;
		while (Time.time - start_time < time)
		{
            float progress = (Time.time - start_time) / time;
            float x_val = Mathf.Lerp(start_alpha, end_alpha, progress);
			float y_val = animationCurve.Evaluate(x_val);
			SetImageAlpha(y_val);
			yield return null;
		}
        SetImageAlpha (end_alpha);
        if (end_alpha == 0.0f)
        {
            gameObject.SetActive(false);
        }
	}
}
