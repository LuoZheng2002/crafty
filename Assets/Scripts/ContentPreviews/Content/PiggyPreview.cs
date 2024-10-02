using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiggyPreview : LoadPreview
{
	public override void Build()
	{
		Rigidbody rb = GetComponent<Rigidbody>();
		Collider c = GetComponent<Collider>();
		rb.useGravity = true;
		c.enabled = true;
	}
	public override void ChangeDirection()
	{
		throw new System.NotImplementedException();
	}

	public override void SetActive(bool active)
	{
		throw new System.NotImplementedException();
	}
}
