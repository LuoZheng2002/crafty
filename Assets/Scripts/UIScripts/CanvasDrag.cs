using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CanvasDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	bool first_person = true;
	float sign = 1;
	public bool active = false;
	public float rotationSpeed = 0.05f;  // Speed of rotation
	PiggyCameraPivot piggyCameraPivot;
	private void Start()
	{
		EventBus.Subscribe<FirstPersonChangedEvent>(OnFirstPersonChanged);
		piggyCameraPivot = GameObject.Find("PiggyCameraPivot").GetComponent<PiggyCameraPivot>();
		Debug.Assert(piggyCameraPivot != null);
	}
	void OnFirstPersonChanged(FirstPersonChangedEvent e)
	{
		first_person = e.first_person;
		sign = first_person ? 1 : -1;
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
		piggyCameraPivot.dragEulerAngle += new Vector3(rotationX, rotationY, 0);
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		Debug.Log("Drag end");
	}
}
