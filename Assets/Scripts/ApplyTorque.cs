using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyTorque : MonoBehaviour
{
    WheelCollider wheelCollider;
    // Start is called before the first frame update
    void Start()
    {
        wheelCollider = GetComponent<WheelCollider>();
		wheelCollider.motorTorque = 0.01f;
	}

    // Update is called once per frame
    void Update()
    {
		float input = Input.GetAxis("Vertical");  // "Vertical" corresponds to W/S or Up/Down keys

		// Apply motor torque based on input
		float torque = input * 5f;  // Adjust multiplier for more/less power
        if (input != 0)
        {
            Debug.Log("Torque applied");
        }
		wheelCollider.motorTorque = torque;
	}
}
