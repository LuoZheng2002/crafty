using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Partner : LoadComponent
{
	public override int Direction { get { return 0; }set { } }

	public override Util.Component Component => Util.Component.Partner;
	GameObject mesh;
	Rigidbody rb;
	void Start()
	{
		mesh = transform.GetChild(0).gameObject;
		Debug.Assert(mesh != null);
		rb = GetComponent<Rigidbody>();
	}
	public override void Build()
	{
		Rigidbody rb = GetComponent<Rigidbody>();
		Collider c = GetComponent<Collider>();
		rb.useGravity = true;
		c.enabled = true;
	}

	public override void ChangeDirection(bool forward = true)
	{
		// throw new System.NotImplementedException();
	}

	public override void SetActive(bool active)
	{
		// throw new System.NotImplementedException();
	}
}
