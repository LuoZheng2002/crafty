using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CanvasDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	float sign = 1;
	public bool active = false;
	public float rotationSpeed = 0.05f;  // Speed of rotation
	public static CanvasDrag Inst
	{
		get { Debug.Assert(inst != null); return inst; }
	}
	static CanvasDrag inst;
	private void Start()
	{
		Debug.Assert(inst == null, "Canvas Drag already set");
		inst = this;
	}
	private void OnDestroy()
	{
		inst = null;
	}
	public void OnFirstPersonChanged()
	{
		sign = GameState.Inst.FirstPerson ? 1 : -1;
	}
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
		float rotationX = eventData.delta.y * rotationSpeed * sign;  // Vertical rotation
		float rotationY = -eventData.delta.x * rotationSpeed * sign;  // Horizontal rotation
		// Rotate the camera accordingly
		PiggyCameraPivot.Inst.dragEulerAngle += new Vector3(rotationX, rotationY, 0);
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		Debug.Log("Drag end");
	}
}
