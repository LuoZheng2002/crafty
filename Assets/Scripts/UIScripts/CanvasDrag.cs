using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CanvasDragEvent
{
	public float deltaX;
	public float deltaY;
    public CanvasDragEvent(float deltaX, float deltaY)
    {
		this.deltaX = deltaX;
		this.deltaY = deltaY;
    }
}

public class CanvasDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	private void Start()
	{
		// Debug.Log("Canvas Drag Instantiated");
	}
	public void OnBeginDrag(PointerEventData eventData)
	{
		// Debug.Log("Dragged!");
	}
	public void OnDrag(PointerEventData eventData)
	{
		EventBus.Publish(new CanvasDragEvent(eventData.delta.x, eventData.delta.y));
	}

	public void OnEndDrag(PointerEventData eventData)
	{
	}
}
