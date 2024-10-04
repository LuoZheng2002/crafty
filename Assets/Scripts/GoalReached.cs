using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GoalReached : MonoBehaviour
{
	MeshRenderer meshRenderer;
	Collider c;
	private void Start()
	{
		meshRenderer = GetComponent<MeshRenderer>();
		c = GetComponent<Collider>();
		EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
	}
	private void OnTriggerEnter(Collider other)
	{
		Debug.Log("You win!");
		EventBus.Publish(new GameStateChangedEvent(Util.GameStateType.Outro, 0));
		meshRenderer.enabled = false;
		c.enabled = false;
	}
	void OnGameStateChanged(GameStateChangedEvent e)
	{
		if (e.state == Util.GameStateType.Build)
		{
			meshRenderer.enabled = true;
			c.enabled = true;
		}
	}
}
