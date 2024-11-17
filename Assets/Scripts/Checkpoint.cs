using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointReachedEvent
{
	public Util.WaypointName waypoint_name;
    public CheckpointReachedEvent(Util. WaypointName waypoint_name)
    {
        this.waypoint_name = waypoint_name;
	}
}

public class Checkpoint : MonoBehaviour
{
	public Util.WaypointName waypoint_name;
    static Dictionary<Util.WaypointName, Checkpoint> checkpoints = new();
	public static Dictionary<Util.WaypointName, Checkpoint> Checkpoints =>checkpoints;
	public GameObject red_mesh;
	public GameObject green_mesh;
	public GameObject checkpoint_goal;
	public static Checkpoint Get(Util.WaypointName waypoint_name)
    {
        Debug.Assert(checkpoints.ContainsKey(waypoint_name), $"checkpoint {waypoint_name} not set");
		return checkpoints[waypoint_name];
	}
	private void Start()
	{
		Debug.Assert(!checkpoints.ContainsKey(waypoint_name));
		checkpoints[waypoint_name] = this;
		EventBus.Subscribe<CheckpointReachedEvent>(OnOtherCheckpointReached);
	}
	void OnOtherCheckpointReached(CheckpointReachedEvent e)
	{
		red_mesh.SetActive(false);
		green_mesh.SetActive(false);
		DeactivateColliderHelper();
	}
	void DeactivateColliderHelper()
	{
		Debug.Log($"{waypoint_name} collider deactivated!");
		checkpoint_goal.SetActive(false);
	}
	private void OnDestroy()
	{
		checkpoints.Clear();
	}
	public void Activate()
	{
		Debug.Log($"{waypoint_name} activated!");
		red_mesh.SetActive(true);
		green_mesh.SetActive(false);
		checkpoint_goal.SetActive(true);
	}
	public void OnCheckpointGoalReached()
	{
		EventBus.Publish(new CheckpointReachedEvent(waypoint_name));
		red_mesh.SetActive(false);
		green_mesh.SetActive(true);
		GameSave.CurrentCheckpoint = waypoint_name;
	}
	// on checkpoint goal reached

}
