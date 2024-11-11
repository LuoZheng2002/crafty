using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Eraser : MonoBehaviour
{
	Image image;
	RectTransform rectTransform;
	ButtonScale buttonScale;
	public static Eraser Inst
	{
		get { Debug.Assert(inst != null, "Eraser not set");return inst; }
	}
	static Eraser inst;
	private void Start()
	{
		Debug.Assert(inst == null, "Eraser already set");
		inst = this;
	}
	private void OnEnable()
	{
		image = GetComponent<Image>();
		rectTransform = image.rectTransform;
		buttonScale = GetComponent<ButtonScale>();
		Util.Delay(this, () =>
		{
			if (!GameState.shown_eraser && GameState.Inst.Components.Count > 0)
			{
				buttonScale.ScaleStart();
			}
		});
	}
	private void OnDestroy()
	{
		inst = null;
	}
	public void OnPlacedAComponent()
	{
		if (!GameState.shown_eraser)
		{
			buttonScale.ScaleStart();
		}
	}
	bool dragging = false;
	public void OnClick()
	{
		if (!dragging)
		{
			ToastManager.Toast("Drag!");
			GameState.shown_eraser = true;
			buttonScale.ScaleStop();
		}		
	}
}
