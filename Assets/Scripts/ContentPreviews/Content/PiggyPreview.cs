using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiggyPreview : LoadPreview
{
	GameObject mesh;
	private void Start()
	{
		mesh = transform.GetChild(0).gameObject;
		Debug.Assert(mesh != null);
	}
	public override void Build()
	{
		Rigidbody rb = GetComponent<Rigidbody>();
		Collider c = GetComponent<Collider>();
		rb.useGravity = true;
		c.enabled = true;
	}
	public void SetInvisible(bool invisible)
	{
		if (invisible)
		{
			mesh.SetActive(false);
		}
		else
		{
			mesh.SetActive(true);
		}
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
