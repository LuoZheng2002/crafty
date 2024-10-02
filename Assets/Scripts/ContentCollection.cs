using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentCollection : MonoBehaviour
{
	public GameObject woodenCratePrefab;
	Dictionary<Util.Content, GameObject> getContent;
	public Dictionary<Util.Content, GameObject> GetContent { get
		{
			getContent ??= new()
				{
					{ Util.Content.WoodenCrate, woodenCratePrefab },
				};
			return getContent;
		} }
	private void Start()
	{
		getContent = new()
		{
			{ Util.Content.WoodenCrate, woodenCratePrefab },
		};
	}
}
