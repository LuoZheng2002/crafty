using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Probe : MonoBehaviour
{
	Rigidbody rb;
	public void MovePosition(Vector3 pos)
	{
		rb.MovePosition(pos);
	}
	public void MoveRotation(Quaternion rot)
	{
		rb.MoveRotation(rot);
	}
	private void Start()
	{
		rb = GetComponent<Rigidbody>();

	}
	void OnTriggerEnter(Collider other)
	{
		// Debug.Log("Ontriggerenter!");
		GridMatrix.Inst.CollisionCount++;
	}
	void OnTriggerExit(Collider other)
	{
		GridMatrix.Inst.CollisionCount--;
	}
}
