using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentInstantiator : MonoBehaviour
{
	public ContentPreview piggyPreview;
	public ContentPreview cratePreview;
	public ContentPreview wheelPreview;
	public ContentPreview turnWheelPreview;
	public ContentPreview motorWheelPreview;
	public Dictionary<Util.Content, ContentPreview> contentToPreview;

	static ContentInstantiator instance;

	public static ContentInstantiator Instance
	{
		get
		{
			if (instance == null)
			{
				instance = GameObject.Find("ContentInstantiator").GetComponent<ContentInstantiator>();
			}
			return instance;
		}
	}

	private void Start()
	{
		contentToPreview = new()
		{
			{ Util.Content.Pig, piggyPreview },
			{ Util.Content.WoodenCrate, cratePreview },
			{ Util.Content.Wheel, wheelPreview },
			{ Util.Content.TurnWheel, turnWheelPreview },
			{ Util.Content.MotorWheel, motorWheelPreview },
		};
	}
	// set parent later
	// if has parent, then position is local
	public ContentPreview InstantiateContent(Util.Content content, Transform parent, Vector3 position, bool local, int direction)
    {
		GameObject prefab = contentToPreview[content].gameObject;
		Debug.Assert(prefab != null);
		GameObject inst = Instantiate(prefab, Vector3.zero, Quaternion.identity);
		Debug.Assert(inst != null);
		Debug.Log("Instantiated an object");
		inst.transform.parent = parent;
		inst.transform.localRotation = Quaternion.identity;
		ContentPreview contentPreview = inst.GetComponent<ContentPreview>();
		if (local)
		{
			contentPreview.MoveLocal(position, "InstantiateContent");
		}
		else
		{
			contentPreview.MoveGlobal(position, "InstantiateContent");
		}
		DirectionalPreview directionalPreview = contentPreview as DirectionalPreview;
		if (directionalPreview != null)
		{
			directionalPreview.Direction = direction;
		}
		return contentPreview;
    }
}
