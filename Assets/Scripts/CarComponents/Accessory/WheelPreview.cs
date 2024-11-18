using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelPreview : AccessoryComponent
{
	public override Util.Component Component => Util.Component.Wheel;
	protected WheelCollider wheelCollider;
	public Transform cylinderTransform;
	protected bool built = false;
	int current_rotation = 0;
    Collider c;
    private void Start()
    {
		c = GetComponent<Collider>();
		Init();
    }
    public override void Build()
	{
		RB.useGravity = true;
		if (c != null)
		{
			c.enabled = true;
		}
		Debug.Assert(cylinderTransform != null);
		wheelCollider = GetComponent<WheelCollider>();
		Debug.Assert(wheelCollider != null);
		wheelCollider.enabled = true;
		built = true;
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

	public override (bool wa, bool sd) GetWASD()
	{
		return (false, false);
	}
	public override void Stick()
	{
		StickUmbrellaOrWheel(true);
	}
	public override List<(Quaternion, RotationInfo)> Rotations => Util.WheelRotations;
	public override bool[] GetDirectionMask()
	{
		return GetDirectionMaskWheelOrUmbrella();
	}
}
