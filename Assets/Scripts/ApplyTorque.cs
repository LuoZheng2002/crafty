using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyTorque : MonoBehaviour
{
    public float torque_factor = 5.0f;
    public float max_turn_angle = 15.0f;
    WheelCollider wheelCollider;
    public bool allow_turn = false;
    // Start is called before the first frame update
    void Start()
    {
        wheelCollider = GetComponent<WheelCollider>();
		wheelCollider.motorTorque = 0.01f;
	}

    // Update is called once per frame
    void Update()
    {
		float v_input = Input.GetAxis("Vertical");  // "Vertical" corresponds to W/S or Up/Down keys

		// Apply motor torque based on input
		float torque = v_input * torque_factor;  // Adjust multiplier for more/less power
		wheelCollider.motorTorque = torque;
        if (allow_turn)
        {
			float h_input = Input.GetAxis("Horizontal");
			wheelCollider.steerAngle = h_input * max_turn_angle;
		}
	}
}
