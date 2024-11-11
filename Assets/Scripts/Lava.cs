using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchLavaEvent
{

}
public class Lava : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (can_retry)
		{	
			can_retry = false;
			EventBus.Publish(new TouchLavaEvent());
			StartCoroutine(ResetRetry());
		}

	}
	bool can_retry = true;
	IEnumerator ResetRetry()
	{
		yield return new WaitForSeconds(10.0f);
		can_retry = true;
	}
}
