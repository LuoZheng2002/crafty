using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelPreview : AccessoryPreview
{
	public override Util.Content Content => Util.Content.Wheel;
	WheelCollider wheelCollider;
	Transform cylinderTransform;
	bool built = false;
	int current_rotation = 0;
	public override int Direction
	{
		get
		{
			return current_rotation;
		}
		set
		{
			current_rotation = value;
			transform.localRotation = Util.WheelRotations[current_rotation].Item1;
		}
	}
	public override void Build()
	{
		Rigidbody rb = GetComponent<Rigidbody>();
		Collider c = GetComponent<Collider>();
		rb.useGravity = true;
		c.enabled = true;
		cylinderTransform = transform.Find("Cylinder");
		Debug.Assert(cylinderTransform != null);
		wheelCollider = transform.Find("Collider").GetComponent<WheelCollider>();
		Debug.Assert(wheelCollider != null);
		wheelCollider.enabled = true;
		built = true;
	}

	public override void ChangeDirection(bool forward)
	{
		Debug.Log("Direction Changed");
		if (forward)
		{
			Direction = (Direction + 1) % Util.WheelRotations.Count;
		}
		else
		{
			Direction = (Direction + Util.WheelRotations.Count - 1) % Util.WheelRotations.Count;
		}		
	}

	public override void SetActive(bool active)
	{
		throw new System.NotImplementedException();
	}
	private void Update()
	{
		if (built)
		{
			wheelCollider.GetWorldPose(out var pos, out var quat);
			cylinderTransform.position = pos;
			cylinderTransform.rotation = quat;
		}
	}

	public override (int h_delta, int w_delta, int l_delta) AttachDir()
	{
		return Util.WheelRotations[current_rotation].Item2;
	}
	public override (bool wa, bool sd) GetWASD()
	{
		return (false, false);
	}
}
