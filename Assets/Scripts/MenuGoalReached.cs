using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuGoalReached : MonoBehaviour
{
	public float recover_time = 0.5f;
	MeshRenderer meshRenderer;
	Collider c;
	private void Start()
	{
		meshRenderer = GetComponent<MeshRenderer>();
		c = GetComponent<Collider>();
	}
	IEnumerator Recover()
	{
		yield return new WaitForSeconds(0.5f);
		meshRenderer.enabled = true;
		c.enabled = true;
	}
	private void OnTriggerEnter(Collider other)
	{
		Debug.Log("You win!");
		EventBus.Publish(new GameStateChangedEvent(Util.GameStateType.Outro, 0));
		meshRenderer.enabled = false;
		c.enabled = false;
		StartCoroutine(Recover());
	}
}

