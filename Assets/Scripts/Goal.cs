using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Goal : MonoBehaviour
{
	MeshRenderer meshRenderer;
	Collider c;
	public int level_num = 0;
	static Dictionary<int, Goal> goals = new();
	/// <summary>
	/// Show the goal specified by level_num and hide the previous goal
	/// </summary>
	static Goal current = null;
	public static void Select(int level_num)
	{
		if (current!=null)
		{
			current.Hide();
		}
		Debug.Assert(goals.ContainsKey(level_num));
		current = goals[level_num];
		current.Show();
	}
	public static void Deselect()
	{
		Debug.Assert(current!=null);
		current.Hide();
		current = null;
	}
	private void Start()
	{
		Debug.Assert(!goals.ContainsKey(level_num));
		goals[level_num] = this;
		meshRenderer = GetComponent<MeshRenderer>();
		c = GetComponent<Collider>();
		meshRenderer.enabled = false;
		c.enabled = false;
	}
	private void OnTriggerEnter(Collider other)
	{
		Debug.Log("You win!");
		GameState.Inst.TransitionToOutro();
		meshRenderer.enabled = false;
		c.enabled = false;
	}

	void Show()
	{
		meshRenderer.enabled = true;
		c.enabled = true;
	}
	void Hide()
	{
		meshRenderer.enabled = false;
		c.enabled = false;
	}
}
