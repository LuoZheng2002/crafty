using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentInstantiator : MonoBehaviour
{
	public VehicleComponent piggyPreview;
	public VehicleComponent cratePreview;
	public VehicleComponent wheelPreview;
	public VehicleComponent turnWheelPreview;
	public VehicleComponent motorWheelPreview;
	public Dictionary<Util.Content, VehicleComponent> contentToPreview;

	static ContentInstantiator inst;

	public static ContentInstantiator Inst
	{
		get
		{
			Debug.Assert(inst != null, "Content Instantiator not set"); return inst;
		}
	}

	private void Start()
	{
		Debug.Assert(inst == null, "Content Instantiator is already created");
		inst = this;
		contentToPreview = new()
		{
			{ Util.Content.Pig, piggyPreview },
			{ Util.Content.WoodenCrate, cratePreview },
			{ Util.Content.Wheel, wheelPreview },
			{ Util.Content.TurnWheel, turnWheelPreview },
			{ Util.Content.MotorWheel, motorWheelPreview },
		};
	}
	private void OnDestroy()
	{
		inst = null;
	}
	// set parent later
	// if has parent, then position is local
	public VehicleComponent InstantiateContent(Util.Content content, Transform parent, Vector3 position, bool local, int direction)
    {
		GameObject prefab = contentToPreview[content].gameObject;
		Debug.Assert(prefab != null);
		GameObject inst;
		if (parent != null)
		{
			inst = Instantiate(prefab, parent);
		}
        else
        {
			inst = Instantiate(prefab, Vector3.zero, Quaternion.identity);
		}
        Debug.Assert(inst != null);
		Debug.Log("Instantiated an object");
		VehicleComponent contentPreview = inst.GetComponent<VehicleComponent>();
		if (local)
		{
			contentPreview.MoveLocal(position);
		}
		else
		{
			contentPreview.MoveGlobal(position, "InstantiateContent");
		}
		DirectionalComponent directionalPreview = contentPreview as DirectionalComponent;
		if (directionalPreview != null)
		{
			directionalPreview.Direction = direction;
		}
		return contentPreview;
    }
}
