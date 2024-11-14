using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CarCore : MonoBehaviour
{
	static CarCore inst;
	public static CarCore Inst { get { Debug.Assert(inst != null); return inst; } }
	Rigidbody rb;
	FixedJoint joint;
	FixedJoint fix_joint;
	private void Start()
	{
		Debug.Assert(inst == null);
		inst = this;
		rb = GetComponent<Rigidbody>();
	}
	private void OnDestroy()
	{
		inst = null;
	}
	public void AttachPiggy()
	{
		Debug.Assert(PiggyPreview.Inst != null);
		joint = transform.AddComponent<FixedJoint>();
		joint.connectedBody = PiggyPreview.Inst.RB;
	}
	public void Fix()
	{
		Debug.Assert(fix_joint == null);
		fix_joint = transform.AddComponent<FixedJoint>();
	}
	public void Unfix()
	{
		Debug.Assert(fix_joint != null);
		Destroy(fix_joint);
		fix_joint = null;
	}
	public void Show()
	{
		gameObject.SetActive(true);
	}
	public void Move()
	{
		rb.velocity = new Vector3(0.5f, 0.0f, 0.0f);
	}
	public void Build()
	{
		gameObject.SetActive(true);
		rb.MovePosition(GridMatrix.Inst.Probe.transform.position);
		foreach (Transform child in transform)
		{
			Destroy(child.gameObject);
		}
	}
	public void Hide()
	{
		gameObject.SetActive(false);
	}
}
