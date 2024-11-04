using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class StarCollectedEvent{ }
public class Starbox : MonoBehaviour
{
	public static int CollectCount{get; private set;}
	private void OnTriggerEnter(Collider other)
	{
		Destroy(gameObject);
		CollectCount++;
		EventBus.Publish(new StarCollectedEvent());
	}
}
