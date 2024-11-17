using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigMapCamera : MonoBehaviour
{
    static BigMapCamera inst;
	public static BigMapCamera Inst
	{
		get { Debug.Assert(inst != null, "Big Map Camera Not Set"); return inst; }
	}
	public Camera Camera { get; private set; }
	bool active = false;
	public void Activate()
	{
		active = true;
	}
	public void Deactivate()
	{
		active = false;
	}
	private void Start()
	{
		Debug.Assert(inst == null, "Big Map Camera Already Set");
		inst = this;
		Camera = GetComponent<Camera>();
		Camera.enabled = false;
		Debug.Assert(Camera != null);
		EventBus.Subscribe<CanvasDragEvent>(OnCanvasDrag);
	}
	public float drag_speed = 0.001f;
	void OnCanvasDrag(CanvasDragEvent e)
	{
		if (active)
		{
			Vector3 pos = transform.position;
			pos.x += e.deltaY * drag_speed * height;
			pos.z -= e.deltaX * drag_speed * height;
			pos.x = Mathf.Clamp(pos.x, -300.0f, 300.0f);
			pos.z = Mathf.Clamp(pos.z, -300.0f, 300.0f);
			transform.position = pos;
		}
	}
	private void OnDestroy()
	{
		inst = null;
	}
	float height = 500.0f;
	float min_height = 200.0f;
	float max_height = 800.0f;
	public float zoom_speed = 100.0f;
	void SetY(float y)
	{
		Vector3 pos = transform.position;
		pos.y = y;
		transform.position = pos;
	}
	private void Update()
	{
		if (!active)
		{
			return;
		}
		if (Input.mouseScrollDelta.y != 0)
		{
			height = Mathf.Clamp(height - Input.mouseScrollDelta.y * zoom_speed, min_height, max_height);
		}
		SetY(height);
	}

}
