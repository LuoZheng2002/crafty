using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridLayer : MonoBehaviour
{
	List<GridCell> children;
	private void Start()
	{
		children = transform.GetComponentsInChildren<GridCell>().ToList();
	}
	public void SetActive()
	{
		if (children == null)
		{
			children = transform.GetComponentsInChildren<GridCell>().ToList();
		}
		foreach (var renderer in children)
		{
			renderer.SetActive(true);
		}
	}
	public void SetInactive()
	{
		foreach (var renderer in children)
		{
			renderer.SetActive(false);
		}
	}
}
