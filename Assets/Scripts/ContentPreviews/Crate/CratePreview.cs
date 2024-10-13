using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CratePreview : ContentPreview
{
	public Material transparent;
	public Material opaque;
	bool active = true;

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
	}

	public override void SetActive(bool active)
	{
		this.active = active;
	}
}
