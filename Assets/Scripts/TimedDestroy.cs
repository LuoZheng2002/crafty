using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedDestroy : MonoBehaviour
{
	private void Start()
	{
		Invoke(nameof(DestroyObject), 20.0f);
	}
	void DestroyObject()
	{
		Destroy(gameObject);
	}
}
