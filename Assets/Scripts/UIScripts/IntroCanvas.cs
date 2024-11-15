using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

public class IntroCanvas : MonoBehaviour
{
    static IntroCanvas inst;
    Text crafty_text;
    Text _3d_text;
    public static IntroCanvas Inst
    {
        get { Debug.Assert(inst != null); return inst; }
    }
    private void Start()
    {
        Debug.Assert(inst == null);
        inst = this;
        gameObject.SetActive(false);
        crafty_text = transform.Find("CraftyPiggies").GetComponent<Text>();
        _3d_text = transform.Find("3D").GetComponent<Text>();
        Debug.Assert(crafty_text != null);
        Debug.Assert(_3d_text != null);
		UnityEngine.Color color = crafty_text.color;
		color.a = 0;
		crafty_text.color = color;
		color = _3d_text.color;
		color.a = 0;
		_3d_text.color = color;
	}
    public void Play(float delay)
    {
        gameObject.SetActive(true);
        StartCoroutine(PlayHelper(delay));
    }
    IEnumerator PlayHelper(float delay)
    {
        yield return new WaitForSeconds(delay);
		UnityEngine.Color color = crafty_text.color;
		color.a = 0;
		crafty_text.color = color;
        color = _3d_text.color;
        color.a = 0;
        _3d_text.color= color;
		float start_time = Time.time;
        float fade_in_time1 = 1.2f;
        float fade_in_time2 = 1.2f;
        float stay_time = 2.0f;
        float fade_out_time = 1.5f;
        while(Time.time - start_time <fade_in_time1)
        {
            float alpha = (Time.time - start_time) / fade_in_time1;
            color = crafty_text.color;
            color.a = alpha;
            crafty_text.color = color;
            yield return null;
        }
        start_time = Time.time;
		while (Time.time - start_time < fade_in_time2)
		{
			float alpha = (Time.time - start_time) / fade_in_time2;
			color = _3d_text.color;
			color.a = alpha;
			_3d_text.color = color;
			yield return null;
		}
		start_time = Time.time;
		while (Time.time - start_time < stay_time)
        {
            yield return null;
        }
        start_time = Time.time;
		while (Time.time - start_time < fade_out_time)
		{
			float alpha = 1.0f - (Time.time - start_time) / fade_out_time;
			color = crafty_text.color;
			color.a = alpha;
			crafty_text.color = color;
			color = _3d_text.color;
			color.a = alpha;
			_3d_text.color = color;
			yield return null;
		}
        gameObject.SetActive(false);
	}
}
