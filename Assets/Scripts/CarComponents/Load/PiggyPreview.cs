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
	GameObject mesh;
	public float scream_velocity = 5.0f;
	bool screaming = false;
	float frame_1_vel = 0.0f;
	float frame_2_vel = 0.0f;
	float frame_3_vel = 0.0f;
	float frame_4_vel = 0.0f;

	public override Util.Component Component => Util.Component.Pig;

	private void Start()
	{
		mesh = transform.GetChild(0).gameObject;
		Debug.Assert(mesh != null);
		EventBus.Subscribe<InvisibleStateUpdateEvent>(OnFIrstPersonChanged);
	}
	private void Update()
	{
		frame_1_vel = frame_2_vel;
		frame_2_vel = frame_3_vel;
		frame_3_vel = frame_4_vel;
		frame_4_vel = Mathf.Abs(RB.velocity.z);
		float average_vel = (frame_1_vel + frame_2_vel + frame_3_vel + frame_4_vel) / 4.0f;
		if (Mathf.Abs(average_vel) > scream_velocity && !screaming)
		{
			screaming = true;
			EventBus.Publish(new ScreamEvent());
		}
		else if (Mathf.Abs(RB.velocity.z) < scream_velocity - 1.0f && screaming) 
		{
			screaming = false;
		}
	}
	public override void Build()
	{
		Collider c = GetComponent<Collider>();
		RB.useGravity = true;
		c.enabled = true;
	}
	void OnFIrstPersonChanged(InvisibleStateUpdateEvent e)
	{
		if (GameState.Inst.IsFirstPerson && GameState.Inst.PiggyPermitInvisible)
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
}
