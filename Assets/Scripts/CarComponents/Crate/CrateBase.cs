using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CrateBase : CrateComponent
{
	public Material transparent;
	public Material opaque;
	bool active = true;
	public float random_force = 1.0f;

	public override void Build()
	{
		Rigidbody rb = GetComponent<Rigidbody>();
		Collider c = GetComponent<Collider>();
		rb.useGravity = true;
		c.enabled = true;
		StartCoroutine(AddForce());
	}
	IEnumerator AddForce()
	{
		//for now turn this off
		Rigidbody rb = GetComponent<Rigidbody>();
		for(int i = 0;i < 5;i++)
		{
			//rb.AddForce(Random.onUnitSphere * random_force, ForceMode.Impulse);
			yield return new WaitForSeconds(0.25f);
		}
	}
}
