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
		OnGridStateChanged();
	}
	bool can_start = false;
	public void OnGridStateChanged()
	{
		if (GridMatrix.Current.design_index >=0)
		{
			can_start = true;
			foreach(var dragImage in DragImage.DragImages)
			{
				if(dragImage.Value.Count > 0)
				{
					can_start = false;
					break;
				}
			}
		}
		else
		{
			// to do
			can_start = GameState.Inst.Piggy != null;
		}
		if (can_start)
		{
			image.color = solidColor;
			buttonScale.ScaleStart();
		}
		else
		{
			image.color = transparentColor;
			buttonScale.ScaleStop();
		}
	}
	public void OnConfirmClicked()
    {
		if (can_start)
		{
			GameState.Inst.TransitionToPlay();
		}
    }
}
