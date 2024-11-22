using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChapterCanvas : MonoBehaviour
{
    static ChapterCanvas inst;
	public static ChapterCanvas Inst
	{
		get
		{
			Debug.Assert(inst != null);
			return inst;
		}
	}
	private void Start()
	{
		Debug.Assert(inst == null);
		inst = this;
		Hide();
	}
	private void OnDestroy()
	{
		inst = null;
	}
	public void Show()
	{
		gameObject.SetActive(true);
	}
	public void Hide()
	{
		gameObject.SetActive(false);
	}
	public Text type;
	public Text text;
	public Text state;
	public void DisplayTextAsync(string type_str, string text_str, string state_str, float force_time)
	{
		GameState.Inst.StartCoroutine(DisplayText(type_str, text_str, state_str, force_time));
	}
	public IEnumerator DisplayText(string type_str, string text_str, string state_str, float force_time)
	{
		gameObject.SetActive(true);
		type.text = type_str;
		text.text = text_str;
		state.text = state_str;
		yield return new WaitForSeconds(force_time);
		while (!Input.GetMouseButtonDown(0))
		{
			yield return null;
		}
		gameObject.SetActive(false);
	}
}
