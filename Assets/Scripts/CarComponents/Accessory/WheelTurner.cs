using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelTurner : MonoBehaviour
{
	public float max_turn_angle = 15.0f;
	public float turn_speed = 1.0f;
	WheelCollider wheelCollider;
	public Transform shaftTransform;
	public bool allow_turn = true;
	Rigidbody rb;
	void Start()
    {
		wheelCollider = transform.GetComponent<WheelCollider>();
		rb = GetComponent<Rigidbody>();
	}

	// Update is called once per frame
	float current_angle = 0.0f;
	public float correction_coef = 2.0f;
    void Update()
    {
		// float real_max_turn_angle = Mathf.Clamp(max_turn_angle - rb.velocity.magnitude*correction_coef, 10, max_turn_angle);
		float real_max_turn_angle = max_turn_angle;
		if (allow_turn)
		{
			float h_input = Input.GetAxisRaw("Horizontal");
			if (h_input != 0.0f)
			{
				float sign = h_input > 0 ? 1 : -1;
				float delta = Time.deltaTime * 45.0f * turn_speed * sign * real_max_turn_angle;
				// current_angle = Mathf.Clamp(current_angle + delta, -real_max_turn_angle, real_max_turn_angle);
				current_angle = real_max_turn_angle * sign;
				wheelCollider.steerAngle = current_angle;
				shaftTransform.localRotation = Quaternion.Euler(0, current_angle, 0);
			}
			else
			{
				//float delta = Time.deltaTime * 45.0f * turn_speed * real_max_turn_angle;
				//if (current_angle > 0)
				//{
				//	current_angle -= delta;
				//	if (current_angle < 0)
				//	{
				//		current_angle = 0;
				//	}
				//}
				//else
				//{

				//	current_angle = 0;

				//}
				current_angle = 0.0f;
				wheelCollider.steerAngle = current_angle;
			}
		}
	}
}
