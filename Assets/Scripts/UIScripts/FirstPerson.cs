using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstPerson : MonoBehaviour
{
	static FirstPerson inst;
	public static FirstPerson Inst
	{
		get { Debug.Assert(inst != null); return inst; }
	}
	private void Start()
	{
		buttonScale = GetComponent<ButtonScale>();
		Debug.Assert(inst == null);
		inst = this;
	}
	private void OnEnable()
	{
		buttonScale = GetComponent<ButtonScale>();
		if (!GameState.shown_third_person)
		{
			buttonScale.ScaleStart();
		}
	}
	private void OnDisable()
	{
		buttonScale.ScaleStop();
	}
	public void Show()
	{
		gameObject.SetActive(true);
	}
	public void Hide()
	{
		gameObject.SetActive(false);
	}
	ButtonScale buttonScale;
	public void ToggleFirstPerson()
    {
        GameState.Inst.IsFirstPerson = !GameState.Inst.IsFirstPerson;
		// PlayCanvasDrag.Inst.OnFirstPersonChanged();
        GameState.shown_third_person = true;
		buttonScale.ScaleStop();
    }
}
