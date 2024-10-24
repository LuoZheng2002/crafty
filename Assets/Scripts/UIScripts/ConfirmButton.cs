using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmButton : MonoBehaviour
{
	Image image;
	Color transparentColor;
	Color solidColor;
	ButtonScale buttonScale;
	public static ConfirmButton Inst
	{
		get { Debug.Assert(inst != null, "Confirm Button not set");return inst; }
	}
	static ConfirmButton inst;
	private void Start()
	{
		Debug.Assert(inst == null, "Confirm button already set");
		inst = this;
		image = GetComponent<Image>();
		buttonScale = GetComponent<ButtonScale>();
		EventBus.Subscribe<ResetCountEvent>(OnTrash);
		transparentColor = new Color(1, 1, 1, 0.2f);
		solidColor = Color.white;
		image.color = transparentColor;
	}
	private void OnDestroy()
	{
		inst = null;
	}
	void OnTrash(ResetCountEvent e)
	{
		image.color = transparentColor;
		buttonScale.ScaleStop();
	}
	public void OnPiggyRemoved()
	{
		image.color = transparentColor;
		buttonScale.ScaleStop();
	}
	public void OnPiggyInstantiated()
	{
		image.color = solidColor;
		if (!GameState.shown_confirm)
		{
			buttonScale.ScaleStart();
		}
	}
	public void OnConfirmClicked()
    {
		Debug.Log("OnConfirmClicked");
		if (GameState.Inst.Piggy!=null)
		{
			GameState.Inst.TransitionToPlay();
		}
		GameState.shown_confirm = true;
		buttonScale.ScaleStop();
    }
}
