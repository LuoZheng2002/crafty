using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridMatrixDragEvent
{
	public float deltaX;
	public float deltaY;
    public GridMatrixDragEvent(float deltaX, float deltaY)
    {
		this.deltaX = deltaX;
		this.deltaY = deltaY;
    }
}

public class CanvasDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public event Action DragBegin;
    public event Action<float, float> Drag;
    public event Action DragEnd;
	public bool is_grid_matrix = false;
	private void Start()
	{
		Debug.Log("Canvas Drag Instantiated");
	}
	public void OnBeginDrag(PointerEventData eventData)
	{
		DragBegin?.Invoke();
		Debug.Log("Dragged!");
	}

	public void OnDrag(PointerEventData eventData)
	{
		Drag?.Invoke(eventData.delta.x, eventData.delta.y);
		if (is_grid_matrix)
		{
			EventBus.Publish(new GridMatrixDragEvent(eventData.delta.x, eventData.delta.y));
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		DragEnd?.Invoke();
	}
}
