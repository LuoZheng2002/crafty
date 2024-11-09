using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelPreview : AccessoryComponent
{
	public override Util.Component Component => Util.Component.Wheel;
	protected WheelCollider wheelCollider;
	public Transform cylinderTransform;
	protected bool built = false;
	int current_rotation = 0;
	public override int Direction
	{
		get
		{
			return current_rotation;
		}
		set
		{
			current_rotation = value;
			transform.localRotation = Util.WheelRotations[current_rotation].Item1;
		}
	}
    Rigidbody rb ;
    Collider c;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
		c = GetComponent<Collider>();
		EventBus.Subscribe<NeighborChangedEvent>(OnNeighborChanged);
    }
    public override void Build()
	{
		rb.useGravity = true;
		if (c != null)
		{
			c.enabled = true;
		}
		Debug.Assert(cylinderTransform != null);
		wheelCollider = GetComponent<WheelCollider>();
		Debug.Assert(wheelCollider != null);
		wheelCollider.enabled = true;
		built = true;
	}

	public override void ChangeDirection(bool forward)
	{
		Debug.Log("Direction Changed");
		if (forward)
		{
			Direction = (Direction + 1) % Util.WheelRotations.Count;
		}
		else
		{
			Direction = (Direction + Util.WheelRotations.Count - 1) % Util.WheelRotations.Count;
		}		
	}

	public override void SetActive(bool active)
	{
		throw new System.NotImplementedException();
	}
	private void Update()
	{
		if (built)
		{
			wheelCollider.GetWorldPose(out var pos, out var quat);
			cylinderTransform.position = pos;
			cylinderTransform.rotation = quat;
		}
	}

	public override (int h_delta, int w_delta, int l_delta) AttachDir()
	{
		return Util.WheelRotations[current_rotation].Item2;
	}
	public override (bool wa, bool sd) GetWASD()
	{
		return (false, false);
	}
	public override void Stick(GridMatrix gridMatrix, int h, int w, int l)
	{
		(int dh, int dw, int dl) = AttachDir();
		(int new_h, int new_w, int new_l) = (h + dh, w + dw, l + dl);
		if (gridMatrix.InGrid(new_h, new_w, new_l) && gridMatrix.crates[new_h, new_w, new_l] != null)
		{
			Util.CreateJoint(this, gridMatrix.crates[new_h, new_w, new_l], gridMatrix.position_spring, gridMatrix.position_damper);
		}
	}
	void OnNeighborChanged(NeighborChangedEvent e)
	{

	}
}
