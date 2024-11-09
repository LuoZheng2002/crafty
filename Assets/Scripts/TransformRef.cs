using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformRef : MonoBehaviour
{
	public Util.TransformRefName transform_ref_name;
    static Dictionary<Util.TransformRefName, TransformRef> transform_refs = new();
	public static TransformRef Get( Util.TransformRefName transform_ref_name)
	{
		Debug.Assert(transform_refs.ContainsKey(transform_ref_name), $"transform ref {transform_ref_name} not set");
		return transform_refs[transform_ref_name];
	}
	private void Start()
	{
		Debug.Assert(!transform_refs.ContainsKey(transform_ref_name), $"Duplicate transform ref {transform_ref_name}");
		transform_refs[transform_ref_name] = this;
		for (int i = 0; i < transform.childCount; i++)
		{
			transform.GetChild(i).gameObject.SetActive(false);
		}
	}
	private void OnDestroy()
	{
		transform_refs.Clear();
	}


}
