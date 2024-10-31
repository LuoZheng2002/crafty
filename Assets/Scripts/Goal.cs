using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalReachedEvent
{
	public Util.GoalName goal_name;
    public GoalReachedEvent(Util.GoalName goal_name)
    {
		this.goal_name = goal_name;
    }
}

public class Goal : MonoBehaviour
{
	MeshRenderer meshRenderer;
	Collider c;
	public Util.GoalName goal_name; 
	static Dictionary<Util.GoalName, Goal> goals = new();
	/// <summary>
	/// Show the goal specified by level_num and hide the previous goal
	/// </summary>
	static Goal current = null;
	public static void Select(Util.GoalName goal_name)
	{
		if (current!=null)
		{
			current.Hide();
		}
		Debug.Assert(goals.ContainsKey(goal_name));
		current = goals[goal_name];
		current.Show();
	}
	public static void Deselect()
	{
		if (current != null)
		{
			current.Hide();
			current = null;
		}
	}
	private void Start()
	{
		Debug.Assert(!goals.ContainsKey(goal_name));
		goals[goal_name] = this;
		meshRenderer = GetComponent<MeshRenderer>();
		c = GetComponent<Collider>();
		meshRenderer.enabled = false;
		c.enabled = false;
	}
	private void OnDestroy()
	{
		goals.Clear();
	}
	private void OnTriggerEnter(Collider other)
	{
		Debug.Log("You win!");
		EventBus.Publish(new GoalReachedEvent(goal_name));
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
