using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EraseChangedEvent
{
	public bool active = false;
    public EraseChangedEvent(bool active)
    {
        this.active = active;
    }
}

public class Eraser : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	public float minScale = 0.8f;
	public float maxScale = 1.2f;
	public float scaleSpeed = 5.0f;
	Image image;
	private void OnEnable()
	{
		image = GetComponent<Image>();
		StartCoroutine(Scale());
	}
	IEnumerator Scale()
	{
		while (!GameState.placed_a_component)
		{
			yield return null;
		}
		while (!GameState.shown_eraser)
		{
			float scale = (Mathf.Sin(Time.time * scaleSpeed) + 1.0f) / 2.0f * (maxScale - minScale) + minScale;
			// Debug.Log($"Scale: {scale}");
			image.rectTransform.localScale = new Vector3(scale, scale, scale);
			yield return null;
		}
		image.rectTransform.localScale = Vector3.one;
	}

	bool dragging = false;
	public void OnBeginDrag(PointerEventData eventData)
	{
		GameState.shown_eraser = true;
		EventBus.Publish(new EraseChangedEvent(true));
		dragging = true;
	}
	public void OnDrag(PointerEventData eventData)
	{
		transform.position = eventData.position;
	}
	public void OnEndDrag(PointerEventData eventData)
	{
		transform.localPosition = Vector3.zero;
		EventBus.Publish(new EraseChangedEvent(false));
		dragging = false;
	}
	public void OnClick()
	{
		if (!dragging)
		{
			ToastManager.Toast("Drag!");
			GameState.shown_eraser = true;
		}		
	}
}
