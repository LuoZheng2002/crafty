using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
	public Util.WaypointName waypoint_name;
    static Dictionary<Util.WaypointName, Checkpoint> checkpoints = new();
	public static Dictionary<Util.WaypointName, Checkpoint> Checkpoints =>checkpoints;
	public static Checkpoint Get(Util.WaypointName waypoint_name)
    {
        Debug.Assert(checkpoints.ContainsKey(waypoint_name), $"checkpoint {waypoint_name} not set");
		return checkpoints[waypoint_name];
	}
	private void Start()
	{
		Debug.Assert(!checkpoints.ContainsKey(waypoint_name));
		checkpoints[waypoint_name] = this;
	}
	private void OnDestroy()
	{
		checkpoints.Clear();
	}
}
