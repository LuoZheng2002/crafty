using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PiggyDestroyEvent
{

}
public class ScreamEvent
{
}

public class PiggyPreview : LoadComponent
{
	int current_direction;
	public override int Direction
	{
		get { return current_direction; }
		set { current_direction = value; }
	}
	GameObject mesh;
	Rigidbody rb;
	public float scream_velocity = 5.0f;
	bool screaming = false;
	float frame_1_vel = 0.0f;
	float frame_2_vel = 0.0f;
	float frame_3_vel = 0.0f;
	float frame_4_vel = 0.0f;

	public override Util.Content Content => Util.Content.Pig;

	private void Start()
	{
		mesh = transform.GetChild(0).gameObject;
		Debug.Assert(mesh != null);
		rb = GetComponent<Rigidbody>();
		EventBus.Subscribe<InvisibleChangedEvent>(SetInvisible);
	}
	private void Update()
	{
		frame_1_vel = frame_2_vel;
		frame_2_vel = frame_3_vel;
		frame_3_vel = frame_4_vel;
		frame_4_vel = Mathf.Abs(rb.velocity.z);
		float average_vel = (frame_1_vel + frame_2_vel + frame_3_vel + frame_4_vel) / 4.0f;
		if (Mathf.Abs(average_vel) > scream_velocity && !screaming)
		{
			screaming = true;
			EventBus.Publish(new ScreamEvent());
		}
		else if (Mathf.Abs(rb.velocity.z) < scream_velocity - 1.0f && screaming) 
		{
			screaming = false;
		}
	}
	public override void Build()
	{
		Rigidbody rb = GetComponent<Rigidbody>();
		Collider c = GetComponent<Collider>();
		rb.useGravity = true;
		c.enabled = true;
	}
	void SetInvisible(InvisibleChangedEvent e)
	{
		if (e.invisible)
		{
			mesh.SetActive(false);
		}
		else
		{
			mesh.SetActive(true);
		}
	}
	private void OnDestroy()
	{
		EventBus.Publish(new PiggyDestroyEvent());
	}
	public override void ChangeDirection(bool forward = true)
	{
		// throw new System.NotImplementedException();
	}

	public override void SetActive(bool active)
	{
		// throw new System.NotImplementedException();
	}
}
