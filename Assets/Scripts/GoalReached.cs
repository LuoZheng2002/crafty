using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalReached : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		Debug.Log("You win!");
		Destroy(gameObject);
	}
}
