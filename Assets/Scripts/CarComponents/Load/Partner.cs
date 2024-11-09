using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Partner : LoadComponent
{
	public override Util.Component Component => Util.Component.Partner;
	GameObject mesh;
	void Start()
	{
		mesh = transform.GetChild(0).gameObject;
		Debug.Assert(mesh != null);
	}
	public override void Build()
	{
		Collider c = GetComponent<Collider>();
		RB.useGravity = true;
		c.enabled = true;
	}
}
