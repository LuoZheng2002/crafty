using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayCanvasDraggedEvent { }
public class PlayCanvasDrag : MonoBehaviour
{
	float sign = 1;
	public bool active = false;
	public float rotationSpeed = 0.05f;  // Speed of rotation
	CanvasDrag canvasDrag;
	public static PlayCanvasDrag Inst
	{
		get { Debug.Assert(inst != null); return inst; }
	}
	static PlayCanvasDrag inst;
	private void Start()
	{
		Debug.Assert(inst == null, "Play Canvas Drag already set");
		inst = this;
		canvasDrag = GetComponent<CanvasDrag>();
		canvasDrag.Drag += OnDrag;
	}
	private void OnDestroy()
	{
		inst = null;
	}
	public void OnFirstPersonChanged()
	{
		sign = GameState.Inst.FirstPerson ? 1 : -1;
	}
	void OnDrag(float deltaX, float deltaY)
	{
		if (!active)
		{
			return;
		}
		float rotationX = deltaY * rotationSpeed * sign;  // Vertical rotation
		float rotationY = -deltaX * rotationSpeed * sign;  // Horizontal rotation
		// Rotate the camera accordingly
		PiggyCameraPivot.Inst.dragEulerAngle += new Vector3(rotationX, rotationY, 0);
		EventBus.Publish(new PlayCanvasDraggedEvent());
	}
}
