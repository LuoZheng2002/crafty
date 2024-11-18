using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ReadyToGoEvent { }
public class ConfirmSuccessEvent { }
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
		EventBus.Publish(new NeighborChangedEvent());
	}
	bool can_start = false;
	public void OnGridStateChanged()
	{
		if (GridMatrix.Inst.ForceDesign)
		{
			can_start = true;
			foreach (var dragImage in DragImage.DragImages)
			{
				if (dragImage.Value.Count > 0)
				{
					can_start = false;
					break;
				}
			}
		}
		else
		{
			can_start = PiggyPreview.Inst != null;
			if (DragImage.DragImages[Util.Component.Partner].Count > 0
				|| DragImage.DragImages[Util.Component.Pig].Count > 0)
			{
				can_start = false;
			}

			if (!EnableConfirm)
			{
				can_start = false;
			}
		}

		if (can_start)
		{
			EventBus.Publish(new ReadyToGoEvent());
			image.color = solidColor;
			buttonScale.ScaleStart();
		}
		else
		{
			image.color = transparentColor;
			buttonScale.ScaleStop();
		}
	}
	bool enable_confirm = true;
	public bool EnableConfirm
	{
		get { return enable_confirm; }
		set
		{
			enable_confirm = value;
			OnGridStateChanged();
		}
	}
	public void OnConfirmClicked()
    {
		if (can_start)
		{
			ForceConfirmClicked();
		}
    }
	public void ForceConfirmClicked()
	{
		EventBus.Publish(new ConfirmSuccessEvent());
		GameState.Inst.TransitionToPlay(true);
		CustomCursor.Inst.SetIdleCursor();
	}
}
