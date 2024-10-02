using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelPreview : AccessoryPreview
{
	public override void Build()
	{
		Rigidbody rb = GetComponent<Rigidbody>();
		Collider c = GetComponent<Collider>();
		rb.useGravity = true;
		c.enabled = true;
		WheelCollider wheelCollider = transform.Find("Cylinder").GetComponent<WheelCollider>();
		Debug.Assert(wheelCollider != null);
		wheelCollider.enabled = true;
	}

	public override void ChangeDirection()
	{
		throw new System.NotImplementedException();
	}

	public override void SetActive(bool active)
	{
		throw new System.NotImplementedException();
	}
}
