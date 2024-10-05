using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GoalReached : MonoBehaviour
{
	MeshRenderer meshRenderer;
	Collider c;
	public int level_num = 1;
	private void Start()
	{
		meshRenderer = GetComponent<MeshRenderer>();
		c = GetComponent<Collider>();
		EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
		meshRenderer.enabled = false;
		c.enabled = false;
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
		ToastManager.Toast($"Received game state changed event: {e.state}, {e.level_num}");
		if (e.state == Util.GameStateType.Build || e.state == Util.GameStateType.Intro)
		{
			if (e.level_num == level_num)
			{
				meshRenderer.enabled = true;
				c.enabled = true;
			}
			else
			{
				meshRenderer.enabled = false;
				c.enabled = false;
			}
		}
	}
}
