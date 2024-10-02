using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelPreview : AccessoryPreview
{
	WheelCollider wheelCollider;
	Transform cylinderTransform;
	bool built = false;
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

	public override void ChangeDirection()
	{
		throw new System.NotImplementedException();
	}

	public override void SetActive(bool active)
	{
		throw new System.NotImplementedException();
	}
	void UpdateWheel()
	{
		wheelCollider.GetWorldPose(out Vector3 pos, out Quaternion quat);
		cylinderTransform.position = pos;
		cylinderTransform.rotation = quat;
	}
	private void Update()
	{
		if (built)
		{
			UpdateWheel();
		}
	}
}
