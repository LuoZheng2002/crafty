using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Eraser : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
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
	public void OnBeginDrag(PointerEventData eventData)
	{
		GameState.shown_eraser = true;
		buttonScale.ScaleStop();
		DragImage.OnEraseStart();
		dragging = true;
	}
	public void OnDrag(PointerEventData eventData)
	{
		RectTransformUtility.ScreenPointToWorldPointInRectangle(
			rectTransform,
			eventData.position,
			Camera.main,
			out Vector3 worldPoint
		);

		// Update the position of the image to follow the mouse
		rectTransform.position = worldPoint;
	}
	public void OnEndDrag(PointerEventData eventData)
	{
		transform.localPosition = Vector3.zero;
		DragImage.OnEraseEnd();
		GridMatrix.Current.OnEraseEnd();
		dragging = false;
	}
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
