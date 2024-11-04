using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWheel : MonoBehaviour
{
	WheelCollider wheelCollider;
	public Transform cylinderTransform;
	public float torque_factor = 5.0f;
	Rigidbody rb;
	private void Start()
	{
		wheelCollider = GetComponent<WheelCollider>();
		rb = transform.parent.GetComponent<Rigidbody>();
	}
	private void Update()
	{
		float v_input = Input.GetAxis("Vertical");  // "Vertical" corresponds to W/S or Up/Down keys

		// Apply motor torque based on input
		float torque = v_input * torque_factor / (rb.velocity.magnitude + 1);  // Adjust multiplier for more/less power
		wheelCollider.motorTorque = torque;
		wheelCollider.GetWorldPose(out var pos, out var quat);
		cylinderTransform.position = pos;
		cylinderTransform.rotation = quat;
	}
}
