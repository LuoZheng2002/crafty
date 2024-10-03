using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelPreview : AccessoryPreview
{
	WheelCollider wheelCollider;
	Transform cylinderTransform;
	bool built = false;
	int current_rotation = 0;
	public override void Build()
	{
		Rigidbody rb = GetComponent<Rigidbody>();
		Collider c = GetComponent<Collider>();
		rb.useGravity = true;
		// c.enabled = true;
		cylinderTransform = transform.Find("Cylinder");
		Debug.Assert(cylinderTransform != null);
		wheelCollider = transform.Find("Collider").GetComponent<WheelCollider>();
		Debug.Assert(wheelCollider != null);
		wheelCollider.enabled = true;
		built = true;
		wheelCollider.motorTorque = 0.01f;
	}

	public override void ChangeDirection()
	{
		Debug.Log("Direction Changed");
		current_rotation = ++current_rotation % Util.WheelRotations.Count;
		transform.rotation = Util.WheelRotations[current_rotation].Item1;
		(var h, var w, var l) =AttachDir();
		Debug.Log($"Attach dir: (h{h}, w{w}, l{l})");
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

	public override (int h_delta, int w_delta, int l_delta) AttachDir()
	{
		return Util.WheelRotations[current_rotation].Item2;
	}
}
