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
	public MeshRenderer meshRenderer;
	Collider c;
	public Util.GoalName goal_name; 
	static Dictionary<Util.GoalName, Goal> goals = new();
	public bool show_goal = false;
	/// <summary>
	/// Show the goal specified by level_num and hide the previous goal
	/// </summary>
	static Goal current = null;
	public static Goal Get(Util.GoalName goal_name)
	{
		Debug.Assert(goals.ContainsKey(goal_name));
		return goals[goal_name];
	}
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
	public static void Activate(Util.GoalName goal_name)
	{
		Debug.Assert(goals.ContainsKey(goal_name));
		goals[goal_name].Show();
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
		if (meshRenderer == null)
		{
			meshRenderer = GetComponent<MeshRenderer>();
			Debug.Assert(meshRenderer != null);
		}
		meshRenderer.enabled = false;
		c = GetComponent<Collider>();
		
		c.enabled = false;
	}
	private void OnDestroy()
	{
		goals.Clear();
	}
	private void OnTriggerEnter(Collider other)
	{
		// Debug.Log("You win!");
		EventBus.Publish(new GoalReachedEvent(goal_name));
		meshRenderer.enabled = false;
		c.enabled = false;
	}

	void Show()
	{
		if (show_goal)
		{
			meshRenderer.enabled = true;
		}
		c.enabled = true;
	}
	void Hide()
	{
		meshRenderer.enabled = false;
		c.enabled = false;
	}
}
