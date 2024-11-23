using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchSphere : MonoBehaviour
{
	private void Start()
	{
		Rigidbody rb = GetComponent<Rigidbody>();
		rb.useGravity = false;
	}
	private void OnTriggerEnter(Collider other)
	{
		// Debug.Log("Trigger enter!");
		Vector3 dir = (transform.position - other.transform.position).normalized;
		float dist = (transform.position - other.transform.position).magnitude;
		float power = 50.0f / (1.0f + dist);
		Rigidbody rb = GetComponent<Rigidbody>();
		rb.AddForce(dir * power, ForceMode.Impulse);
		rb.useGravity = true;
	}
}
