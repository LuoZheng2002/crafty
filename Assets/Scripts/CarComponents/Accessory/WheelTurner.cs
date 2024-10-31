using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelTurner : MonoBehaviour
{
	public float max_turn_angle = 15.0f;
	WheelCollider wheelCollider;
	public Transform shaftTransform;
	public bool allow_turn = true;
	void Start()
    {
		wheelCollider = transform.Find("Collider").GetComponent<WheelCollider>();
	}

    // Update is called once per frame
    void Update()
    {
		if (allow_turn)
		{
			float h_input = Input.GetAxis("Horizontal");
			float steerAngle = h_input * max_turn_angle;
			wheelCollider.steerAngle = steerAngle;
			shaftTransform.localRotation = Quaternion.Euler(0, steerAngle, 0);
		}
	}
}
