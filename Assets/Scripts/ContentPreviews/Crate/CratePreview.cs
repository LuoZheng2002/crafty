using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CratePreview : ContentPreview
{
	public Material transparent;
	public Material opaque;
	bool active = true;
	public float random_force = 1.0f;
	public override Util.Content Content => Util.Content.WoodenCrate;

	private void Start()
	{
		
	}
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
		Rigidbody rb = GetComponent<Rigidbody>();
		for(int i = 0;i < 5;i++)
		{
			rb.AddForce(Random.onUnitSphere * random_force, ForceMode.Impulse);
			yield return new WaitForSeconds(0.25f);
		}
	}

	public override void SetActive(bool active)
	{
		this.active = active;
	}
}
