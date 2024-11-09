using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelMotor : MonoBehaviour
{
	WheelCollider wheelCollider;
	public float torque_factor = 5.0f;
	// Start is called before the first frame update
	void Start()
    {
		wheelCollider = GetComponent<WheelCollider>();
	}

    // Update is called once per frame
    void Update()
    {
		float v_input = Input.GetAxis("Vertical");  // "Vertical" corresponds to W/S or Up/Down keys

		// Apply motor torque based on input
		float torque = v_input * torque_factor;  // Adjust multiplier for more/less power
		wheelCollider.motorTorque = torque;
	}
}
