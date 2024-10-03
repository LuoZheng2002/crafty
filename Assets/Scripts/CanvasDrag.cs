using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CanvasDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	public bool active = false;
	public float rotationSpeed = 0.05f;  // Speed of rotation
	public void OnBeginDrag(PointerEventData eventData)
	{
		Debug.Log("Drag begin");
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (!active)
		{
			return;
		}
		float rotationX = eventData.delta.y * rotationSpeed;  // Vertical rotation
		float rotationY = -eventData.delta.x * rotationSpeed;  // Horizontal rotation
		// Rotate the camera accordingly
		Camera.main.transform.localEulerAngles += new Vector3(rotationX, rotationY, 0);
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		Debug.Log("Drag end");
	}
}
