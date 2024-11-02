using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineCanvas : MonoBehaviour
{
	public float seconds_per_char = 0.2f;
	static LineCanvas topCanvas;
	static LineCanvas bottomCanvas;
	public bool top;
	public static LineCanvas Top
	{
		get { Debug.Assert(topCanvas != null, "Top Canvas not set"); return topCanvas; }
	}
	public static LineCanvas Bottom
	{
		get { Debug.Assert(bottomCanvas != null, "Bottom Canvas not set"); return bottomCanvas; }
	}
	Text line;
	Text n;
	ButtonScale buttonScale;
	private void Start()
	{
		if (top)
		{
			Debug.Assert(topCanvas == null, "Top canvas already set");
			topCanvas = this;
		}
		else
		{
			Debug.Assert(bottomCanvas == null, "Bottom canvas already set");
			bottomCanvas = this;
		}
		line = transform.Find("Panel").Find("Line").GetComponent<Text>();
		n = transform.Find("Panel").Find("Name").GetComponent<Text>();
		buttonScale = transform.Find("Panel").Find("Continue").GetComponent<ButtonScale>();
		Debug.Assert(line != null);
		Debug.Assert(n != null);
		Debug.Assert(buttonScale != null);
		buttonScale.gameObject.SetActive(false);
		Hide();
	}
	private void OnDestroy()
	{
		topCanvas = null;
	}
	string line_string;
	public string Name
	{
		set {
			gameObject.SetActive(true);
			n.text = value; }
	}
	IEnumerator ShowText()
	{
		line.text = "";
		for (int i = 0; i < line_string.Length; i++)
		{
			line.text += line_string[i];
			yield return new WaitForSeconds(seconds_per_char);
		}
	}
	IEnumerator coroutine;
	public string Line
	{
		set 
		{
			gameObject.SetActive(true);
			line_string = value;
			if (coroutine != null)
			{
				StopCoroutine(coroutine);
				coroutine = null;
			}
			coroutine = ShowText();
			StartCoroutine(coroutine);
		}
	}
	public void SetString(string str)
	{

	}
	public void Hide()
	{
		if (coroutine != null)
		{
			StopCoroutine(coroutine);
			coroutine = null;
		}
		gameObject.SetActive(false);
	}
	public void ShowContinue()
	{
		buttonScale.gameObject.SetActive(true);
		buttonScale.ScaleStart();
	}
	public void HideContinue()
	{
		buttonScale.gameObject.SetActive(false);
	}
}
