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

	void Start()
    {
		wheelCollider = transform.GetComponent<WheelCollider>();
	}

	// Update is called once per frame
	float current_angle = 0.0f;
    void Update()
    {
		if (allow_turn)
		{
			float h_input = Input.GetAxis("Horizontal");
			if (h_input != 0.0f)
			{
				float sign = h_input > 0 ? 1 : -1;
				float delta = Time.deltaTime * 45.0f * turn_speed * sign * max_turn_angle;
				current_angle = Mathf.Clamp(current_angle + delta, -max_turn_angle, max_turn_angle);
				wheelCollider.steerAngle = current_angle;
				shaftTransform.localRotation = Quaternion.Euler(0, current_angle, 0);
			}
			else
			{
				float delta = Time.deltaTime * 45.0f * turn_speed * max_turn_angle;
				if (current_angle > 0)
				{
					current_angle -= delta;
					if (current_angle < 0)
					{
						current_angle = 0;
					}
				}
				else
				{
					current_angle += delta;
					if (current_angle > 0)
					{
						current_angle = 0;
					}
				}
				wheelCollider.steerAngle = current_angle;
			}
		}
	}
}
