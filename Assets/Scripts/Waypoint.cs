using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
	public Material yellow;
	public Material red;
	public Material green;
	public Util.WaypointName waypoint_name;
	public static Dictionary<Util.WaypointName, Waypoint> Waypoints { get; private set; } = new();
	MeshRenderer meshRenderer;
	private void Start()
	{
		Debug.Assert(!Waypoints.ContainsKey(waypoint_name));
		Waypoints[waypoint_name] = this;
		meshRenderer = transform.Find("beacon").GetComponent<MeshRenderer>();
	}
	public void ChangeToGreen()
	{
		meshRenderer.material = green;
	}
}
